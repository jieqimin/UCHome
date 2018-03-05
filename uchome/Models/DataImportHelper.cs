using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Data;
using System.IO;
namespace UCHome.Models {
	public class DataImportHelper
    {
        public static HSSFWorkbook hssfworkbook;
        public static XSSFWorkbook xssfworkbook;
        public static DataTable ImportExcelFile(string filePath, string extension, int sheetnum = 0, int columnnum = 0)
        {
            if (extension == ".xls")
                return ImportExcelXlsFile(filePath, extension,sheetnum,columnnum);
            else
                return ImportExcelXlsXFile(filePath, extension, sheetnum, columnnum);
        }
        public static DataTable ImportExcelXlsFile(string filePath, string extension, int sheetnum = 0, int columnnum = 0)
        {
            try
            {
                using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    hssfworkbook = new HSSFWorkbook(file);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            NPOI.SS.UserModel.ISheet sheet;
            sheet = hssfworkbook.GetSheetAt(0);
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();
            DataTable dt = new DataTable();
            //给DdataTable添加表头
            if (sheet.GetRow(sheetnum) != null)
            {
                for (int j = 0; j < (sheet.GetRow(columnnum).LastCellNum); j++)
                {
                    dt.Columns.Add(sheet.GetRow(columnnum).Cells[j].ToString());
                }
                //读取数据  
                while (rows.MoveNext())
                {
                    HSSFRow row1 = null;

                    row1 = (HSSFRow)rows.Current;
                    int j = row1.LastCellNum;

                    DataRow dr = dt.NewRow();
                    if (j > dt.Columns.Count)
                    {
                        j = dt.Columns.Count;
                    }
                    for (int i = 0; i < j; i++)
                    {
                        NPOI.SS.UserModel.ICell cell = row1.GetCell(i);

                        if (cell == null)
                        {
                            dr[i] = null;
                        }
                        else
                        {
                            dr[i] = cell.ToString();
                        }
                    }
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }
        public static DataTable ImportExcelXlsXFile(string filePath, string extension, int sheetnum = 0, int columnnum = 0)
        {
            try
            {
                using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    xssfworkbook = new XSSFWorkbook(file);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            NPOI.SS.UserModel.ISheet sheet;
            sheet = xssfworkbook.GetSheetAt(0);
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();
            DataTable dt = new DataTable();
            //给DdataTable添加表头
            if (sheet.GetRow(sheetnum) != null)
            {
                for (int j = 0; j < (sheet.GetRow(columnnum).LastCellNum); j++)
                {
                    dt.Columns.Add(sheet.GetRow(columnnum).Cells[j].ToString());
                }
                //读取数据  
                while (rows.MoveNext())
                {
                    XSSFRow row2 = null;
                    row2 = (XSSFRow)rows.Current;
                    int j = row2.LastCellNum;

                    DataRow dr = dt.NewRow();
                    if (j > dt.Columns.Count)
                    {
                        j = dt.Columns.Count;
                    }
                    for (int i = 0; i < j; i++)
                    {
                        NPOI.SS.UserModel.ICell cell = row2.GetCell(i);

                        if (cell == null)
                        {
                            dr[i] = null;
                        }
                        else
                        {
                            dr[i] = cell.ToString();
                        }
                    }
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }
    }
}
