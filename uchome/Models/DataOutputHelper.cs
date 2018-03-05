using System;
using System.Diagnostics;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data.OleDb;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;
namespace UCHome.Models {
    /// <summary>
    /// 操作EXCEL导出数据报表的类
    /// Copyright (C) Maticsoft
    /// </summary>
    public class DataOutputHelper
    {
        public DataOutputHelper()
        {
        }
        public string DataTableToExcel(DataTable dt, string excelPath)
        {
            int num3;
            if (dt == null)
            {
                return "DataTable不能为空";
            }
            dt.TableName = "Sheet1";
            int count = dt.Rows.Count;
            int num2 = dt.Columns.Count;
            if (count == 0)
            {
                return "没有数据";
            }
            StringBuilder builder = new StringBuilder();
            string connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=Excel 8.0;", excelPath);
            builder.Append("CREATE TABLE ");
            builder.Append(dt.TableName + " ( ");
            for (num3 = 0; num3 < num2; num3++)
            {
                if (num3 < (num2 - 1))
                {
                    builder.Append(string.Format("{0} nvarchar,", dt.Columns[num3].ColumnName));
                }
                else
                {
                    builder.Append(string.Format("{0} nvarchar)", dt.Columns[num3].ColumnName));
                }
            }
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand();
                command.Connection = connection;
                command.CommandText = builder.ToString();
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception exception)
                {
                    return ("在Excel中创建表失败，错误信息：" + exception.Message);
                }
                builder.Remove(0, builder.Length);
                builder.Append("INSERT INTO ");
                builder.Append(dt.TableName + " ( ");
                for (num3 = 0; num3 < num2; num3++)
                {
                    if (num3 < (num2 - 1))
                    {
                        builder.Append(dt.Columns[num3].ColumnName + ",");
                    }
                    else
                    {
                        builder.Append(dt.Columns[num3].ColumnName + ") values (");
                    }
                }
                for (num3 = 0; num3 < num2; num3++)
                {
                    if (num3 < (num2 - 1))
                    {
                        builder.Append("@" + dt.Columns[num3].ColumnName + ",");
                    }
                    else
                    {
                        builder.Append("@" + dt.Columns[num3].ColumnName + ")");
                    }
                }
                command.CommandText = builder.ToString();
                OleDbParameterCollection parameters = command.Parameters;
                num3 = 0;
                while (num3 < num2)
                {
                    parameters.Add(new OleDbParameter("@" + dt.Columns[num3].ColumnName, OleDbType.VarChar));
                    num3++;
                }
                foreach (DataRow row in dt.Rows)
                {
                    for (num3 = 0; num3 < parameters.Count; num3++)
                    {
                        parameters[num3].Value = row[num3];
                    }
                    command.ExecuteNonQuery();
                }
                return "数据已成功导入Excel";
            }
        }
        public DataSet ExcelToDS(string Path)
        {
            string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Path + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\";";
            OleDbConnection connection = new OleDbConnection(connectionString);
            connection.Open();
            string selectCommandText = "";
            OleDbDataAdapter adapter = null;
            DataSet dataSet = null;
            selectCommandText = "select * from [Sheet1$]";
            adapter = new OleDbDataAdapter(selectCommandText, connectionString);
            dataSet = new DataSet();
            adapter.Fill(dataSet, "table1");
            connection.Close();
            return dataSet;
        }
    }
    /// <summary>
    /// 提供将泛型集合数据导出Excel文档。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExcelResult<T> : ActionResult where T : new()
    {
        public ExcelResult(IList<T> entity, string fileName, string[] showName, params string[] DataName)
        {
            this.Entity = entity;
            this.showName = showName;
            this.DataName = DataName;
            this.FileName = fileName;
        }

        public ExcelResult(IList<T> entity, string[] showName, params string[] DataName)
        {
            this.Entity = entity;
            this.showName = showName;
            this.DataName = DataName;
            DateTime time = DateTime.Now;
            this.FileName = string.Format("{0}_{1}_{2}_{3}",
                time.Month, time.Day, time.Hour, time.Minute);
        }

        public IList<T> Entity
        {
            get;
            set;
        }

        public string FileName
        {
            get;
            set;
        }
        public string[] showName
        {
            get;
            set;
        }
        public string[] DataName
        {
            get;
            set;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            if (Entity == null)
            {
                new EmptyResult().ExecuteResult(context);
                return;
            }

            SetResponse(context, showName, DataName);
        }

        /// <summary>
        /// 设置并向客户端发送请求响应。
        /// </summary>
        /// <param name="context"></param>
        private void SetResponse(ControllerContext context, string[] showName, params string[] DataName)
        {
            StringBuilder sBuilder = ConvertEntity(showName, DataName);
            byte[] bytestr = Encoding.Unicode.GetBytes(sBuilder.ToString());

            context.HttpContext.Response.Clear();
            context.HttpContext.Response.ClearContent();
            context.HttpContext.Response.Buffer = true;
            context.HttpContext.Response.Charset = "GB2312";
            context.HttpContext.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            context.HttpContext.Response.ContentType = "application/ms-excel";
            context.HttpContext.Response.AddHeader("Content-Disposition", "attachment; filename=" + FileName + ".xls");
            context.HttpContext.Response.AddHeader("Content-Length", bytestr.Length.ToString());
            context.HttpContext.Response.Write(sBuilder);
            context.HttpContext.Response.Flush();
            context.HttpContext.Response.Close();
            //context.HttpContext.Response.End();
        }

        /// <summary>
        /// 把泛型集合转换成组合Excel表格的字符串。
        /// </summary>
        /// <returns></returns>
        private StringBuilder ConvertEntity(string[] showName, params string[] DataName)
        {
            StringBuilder sb = new StringBuilder();

            AddTableHead(sb, showName, DataName);
            AddTableBody(sb, showName, DataName);

            return sb;
        }

        /// <summary>
        /// 根据IList泛型集合中的每项的属性值来组合Excel表格。
        /// </summary>
        /// <param name="sb"></param>
        private void AddTableBody(StringBuilder sb, string[] showName, params string[] DataName)
        {
            List<string> DataNameList = new List<string>();
            if (DataName != null)
                DataNameList.AddRange(DataName);
            List<string> showNameList = new List<string>();
            if (showName != null)
                showNameList.AddRange(showName);
            if (Entity == null || Entity.Count <= 0)
            {
                return;
            }

            PropertyDescriptorCollection properties = FindProperties();

            if (properties.Count <= 0)
            {
                return;
            }

            for (int i = 0; i < Entity.Count; i++)
            {
                int datacount = 0;
                for (int j = 0; j < properties.Count; j++)
                {
                    if (DataNameList.Count == 0)
                    {
                        return;
                    }
                    else
                    {
                        if (DataNameList.Contains(properties[j].Name))
                        {
                            string sign = "\t";
                            if (datacount == DataNameList.Count - 1)
                                sign = "\n";
                            object obj = properties[j].GetValue(Entity[i]);
                            obj = obj == null ? string.Empty : obj.ToString();
                            sb.Append(obj + sign);
                            datacount++;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 根据指定类型T的所有属性名称来组合Excel表头。
        /// </summary>
        /// <param name="sb"></param>
        private void AddTableHead(StringBuilder sb, string[] showName, params string[] DataName)
        {
            List<string> DataNameList = new List<string>();
            if (DataName != null)
                DataNameList.AddRange(DataName);
            List<string> showNameList = new List<string>();
            if (showName != null)
                showNameList.AddRange(showName);

            PropertyDescriptorCollection properties = FindProperties();

            if (properties.Count <= 0)
            {
                return;
            }
            int showcount = 0;
            for (int i = 0; i < properties.Count; i++)
            {
                if (DataNameList.Count == 0)
                {
                    return;
                }
                else
                {
                    if (DataNameList.Contains(properties[i].Name))
                    {
                        string sign = "\t";
                        if (showcount == DataNameList.Count - 1)
                            sign = "\n";
                        sb.Append(showNameList[showcount] + sign);
                        //headerRow.CreateCell(showcount).SetCellValue(showNameList[showcount].ToString());
                        showcount++;
                    }
                }
            }
        }

        /// <summary>
        /// 返回指定类型T的属性集合。
        /// </summary>
        /// <returns></returns>
        private static PropertyDescriptorCollection FindProperties()
        {
            return TypeDescriptor.GetProperties(typeof(T));
        }
    }
}
