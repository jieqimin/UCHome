using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
namespace UCHome.Models {
    public class DataConvertHelper<T>
    {
        /// <summary>
        /// 泛型集合类导出成excel
        /// </summary>
        /// <param name="list">泛型集合类</param>
        /// <param name="fileName">生成的excel文件名</param>
        /// <param name="propertyName">excel的字段列表</param>
        public static void ListToExcel(IList<T> list, string fileName, string[] showName, params string[] propertyName)
        {
            RenderMemorStream(ListToExcel<T>(list, showName, propertyName),fileName);
        }
        private static void RenderMemorStream(MemoryStream ms,string fileName)
        {
            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel;charset=UTF-8";
            HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName));
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.BinaryWrite(ms.GetBuffer());
            HttpContext.Current.Response.End();
        }

        public static MemoryStream ListToExcel<T>(IList<T> list, string[] showName, params string[] propertyName)
        {
            //创建流对象
            using (MemoryStream ms = new MemoryStream())
            {
                //将参数写入到一个临时集合中
                List<string> propertyNameList = new List<string>();
                if (propertyName != null)
                    propertyNameList.AddRange(propertyName);
                List<string> showNameList = new List<string>();
                if (showName != null)
                    showNameList.AddRange(showName);
                //床NOPI的相关对象
                IWorkbook workbook = new HSSFWorkbook();
                ISheet sheet = workbook.CreateSheet();
                IRow headerRow = sheet.CreateRow(0);
                int showcount = 0;

                if (list.Count > 0)
                {
                    //通过反射得到对象的属性集合
                    PropertyInfo[] propertys = list[0].GetType().GetProperties();
                    //遍历属性集合生成excel的表头标题
                    for (int i = 0; i < propertys.Count(); i++)
                    {
                        //判断此属性是否是用户定义属性
                        if (propertyNameList.Count == 0)
                        {
                            headerRow.CreateCell(i).SetCellValue(propertys[i].Name);
                        }
                        else
                        {
                            if (propertyNameList.Contains(propertys[i].Name))
                            {
                                headerRow.CreateCell(showcount).SetCellValue(showNameList[showcount]);
                                showcount++;
                            }
                        }
                    }

                    int rowIndex = 1;
                    //遍历集合生成excel的行集数据
                    for (int i = 0; i < list.Count; i++)
                    {
                        int datacount = 0;
                        IRow dataRow = sheet.CreateRow(rowIndex);
                        if (propertyNameList.Count == 0)
                        {
                            for (int j = 0; j < propertys.Count(); j++)
                            {
                                //如果没有限定就导出所有
                                object obj = propertys[j].GetValue(list[i], null);
                                dataRow.CreateCell(j).SetCellValue(obj == null ? "" : obj.ToString());
                            }
                        }
                        else
                        {
                            for (int k = 0; k < propertyNameList.Count; k++)
                            {
                                for (int j = 0; j < propertys.Count(); j++)
                                {
                                    if (propertyNameList[k] == propertys[j].Name)
                                    {
                                        object obj = propertys[j].GetValue(list[i], null);
                                        dataRow.CreateCell(datacount).SetCellValue(obj == null ? "" : obj.ToString());
                                        datacount++;
                                    }
                                }
                            }
                        }
                        rowIndex++;
                    }
                }
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;
                return ms;
            }
        }
        public static void ListToExcel(IList<OutExcelModel> lEM,string fileName)
        {
            RenderMemorStream(ListToExcel(lEM), fileName);
        }
      
        private static MemoryStream ListToExcel(IList<OutExcelModel> lEM)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                //床NOPI的相关对象
                HSSFWorkbook workbook = new HSSFWorkbook();
                for (int i = 0; i < lEM.Count(); i++)
                {
                    workbook = ListToExcel(lEM[i], workbook);
                }
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;
                return ms;
            }
        }
        private static HSSFWorkbook ListToExcel(OutExcelModel OEM, HSSFWorkbook workbook)
        {
            //将参数写入到一个临时集合中
            List<string> propertyNameList = new List<string>();
            if (OEM.propertyName != null)
                propertyNameList.AddRange(OEM.propertyName);
            List<string> showNameList = new List<string>();
            if (OEM.showName != null)
                showNameList.AddRange(OEM.showName);

            ISheet sheet = workbook.CreateSheet(OEM.sheetName);
            IRow headerRow = sheet.CreateRow(0);
            int showcount = 0;
            if (OEM.list.Count > 0)
            {
                //通过反射得到对象的属性集合
                PropertyInfo[] propertys = OEM.list[0].GetType().GetProperties();
                //遍历属性集合生成excel的表头标题
                for (int i = 0; i < propertys.Count(); i++)
                {
                    //判断此属性是否是用户定义属性
                    if (propertyNameList.Count == 0)
                    {
                        headerRow.CreateCell(i).SetCellValue(propertys[i].Name);
                    }
                    else
                    {
                        if (propertyNameList.Contains(propertys[i].Name))
                        {
                            headerRow.CreateCell(showcount).SetCellValue(showNameList[showcount]);
                            showcount++;
                        }
                    }
                }

                int rowIndex = 1;
                //遍历集合生成excel的行集数据
                for (int i = 0; i < OEM.list.Count; i++)
                {
                    int datacount = 0;
                    IRow dataRow = sheet.CreateRow(rowIndex);
                    if (propertyNameList.Count == 0)
                    {
                        for (int j = 0; j < propertys.Count(); j++)
                        {
                            //如果没有限定就导出所有
                            object obj = propertys[j].GetValue(OEM.list[i], null);
                            dataRow.CreateCell(j).SetCellValue(obj == null ? "" : obj.ToString());
                        }
                    }
                    else
                    {
                        for (int k = 0; k < propertyNameList.Count; k++)
                        {
                            for (int j = 0; j < propertys.Count(); j++)
                            {
                                if (propertyNameList[k] == propertys[j].Name)
                                {
                                    object obj = propertys[j].GetValue(OEM.list[i], null);
                                    dataRow.CreateCell(datacount).SetCellValue(obj == null ? "" : obj.ToString());
                                    datacount++;
                                }
                            }
                        }
                    }
                    rowIndex++;
                }
            }
            return workbook;
        }
    }

    public class OutExcelModel
    {
        public IList list { get; set; }
        public string[] showName { get; set; }
        public string[] propertyName { get; set; }
        public string sheetName { get; set; }
    }
}