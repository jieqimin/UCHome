using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using ServiceStack.Common.Extensions;

namespace UCHome.Models
{
    public class NPOIExcelHelper
    {
        private HSSFWorkbook hssfworkbook;

        public NPOIExcelHelper()
        {
            hssfworkbook = new HSSFWorkbook();
        }

        public HSSFWorkbook SaveTableToExcel(IList<ExcelTable> tables)
        {
            ICellStyle cellStyle = hssfworkbook.CreateCellStyle();
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            cellStyle.Alignment=HorizontalAlignment.Center;

            var titleStyle = hssfworkbook.CreateCellStyle();
            titleStyle.CloneStyleFrom(cellStyle);
            titleStyle.FillForegroundColor = HSSFColor.Grey40Percent.Index;
            titleStyle.FillPattern = FillPattern.SolidForeground;
            titleStyle.FillBackgroundColor = HSSFColor.Grey40Percent.Index;
            var font = hssfworkbook.CreateFont();
            font.IsBold = true;
            font.FontHeightInPoints = 14;
            titleStyle.SetFont(font);
            tables.ForEach(tableItem =>
            {
                var sheet = hssfworkbook.CreateSheet(tableItem.TableName);
                var colIndex = 0;
                var rowIndex = 0;
                var rowCurrentMaxIndex = 0;
                tableItem.Tbody.ForEach(trItem =>
                {
                    var row = sheet.CreateRow(rowIndex);
                    colIndex = 0;
                    
                        trItem.ExcelTds.ForEach(tdItem =>
                        {
                            while (sheet.IsMergedRegion(new CellRangeAddress(rowIndex,rowIndex,colIndex,colIndex)))
                            {
                                colIndex ++;
                            }
                            
                            row.CreateCell(colIndex).SetCellValue(tdItem.Value);
                            if (rowIndex > 0)
                            {
                                row.GetCell(colIndex).CellStyle = cellStyle;
                            }
                            else
                            {
                                row.GetCell(colIndex).CellStyle = titleStyle;
                            }

                            rowCurrentMaxIndex = rowIndex + tdItem.RowSpan;
                            sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowCurrentMaxIndex - 1, colIndex,
                                colIndex + tdItem.ColSpan - 1));
                            colIndex = colIndex + tdItem.ColSpan;
                        });
                    
                    rowIndex++;
                });
                var maxColomn = tableItem.Tbody[0].ExcelTds.Count;
                for (var i = 0; i < maxColomn; i++)
                {
                    sheet.AutoSizeColumn(i,true);
                }
            });
            
            return hssfworkbook;
            
        }
      
    }

    public class ExcelTable
    {
        public string TableName { get; set; }
       
        public List<ExcelTr> Tbody { get; set; }
    }

    public class ExcelTr
    {
        public List<ExcelTd> ExcelTds { get; set; }

        public ExcelTr()
        {
            ExcelTds = new List<ExcelTd>();

        }

        public void AddTd(string value)
        {
            this.ExcelTds.Add(new ExcelTd(value));
        }
        public void AddTd(params string []value)
        {
            value.ForEach(AddTd);
        } 
        public void AddTd(int colspan,int rowSpan, params string []value)
        {
            value.ForEach(item =>
            {
                AddTd(new ExcelTd(item, colspan, rowSpan));
            });
        }
        public void AddTd(ExcelTd td)
        {
            this.ExcelTds.Add(td);
        }
    }

    public class ExcelTd
    {
        private int _colSpan;
        private int _rowSpan;

        public ExcelTd(string value)
        {
            this.Value = value;
        }

        public ExcelTd ()
        {
        }

        public ExcelTd(string value, int colSpan, int rowSpan)
        {
            Value = value;
            _colSpan = colSpan;
            _rowSpan = rowSpan;
        }

        public int ColSpan {
            get
            {
                if (_colSpan == 0) return 1;
                return _colSpan;
            }
            set { _colSpan = value; }
        }
        public int RowSpan
        {
            get
            {
                if (_rowSpan == 0) return 1;
                return _rowSpan;
            }
            set { _rowSpan = value; }
        }
        public string Value { get; set; }


    }

   
}