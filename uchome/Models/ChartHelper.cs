
using System.Collections.Generic;
using System.Data;
using dotnetCHARTING;
using System.Web;
using System.IO;

namespace UCHome.Models
{
    public class ChartHelper
    {
        private string _phaysicalimagepath;//图片存放路径
        private string _title; //图片标题
        private string _xtitle;//图片x座标名称
        private string _ytitle;//图片y座标名称
        private int _picwidth = 0;//图片宽度
        private int _pichight = 0;//图片高度
        private List<string> _lstSql; //用于存放sql语句的List
        private List<string> _lstName; //用于存放曲线名称的List
        private DataTable _dt;

        ///// <summary>
        ///// 图片存放路径
        /////图片存放路径默认为temp文件夹，可通过设置这个属性来修改
        ///// </summary>
        public string PhaysicalImagePath
        {
            set { _phaysicalimagepath = value; }
            get { return _phaysicalimagepath; }
        }

        /// <summary>
        /// 图片标题
        /// </summary>
        public string Title
        {
            set { _title = value; }
            get { return _title; }
        }

        /// <summary>
        /// 图片X轴标题
        /// </summary>
        public string XTitle
        {
            set { _xtitle = value; }
            get { return _xtitle; }
        }

        /// <summary>
        /// 图片Y轴标题
        /// </summary>
        public string YTitle
        {
            set { _ytitle = value; }
            get { return _ytitle; }
        }

        /// <summary>
        /// 图片宽度
        /// </summary>
        public int PicWidth
        {
            set { _picwidth = value; }
            get { return _picwidth; }
        }

        /// <summary>
        /// 图片高度
        /// </summary>
        public int PicHight
        {
            set { _pichight = value; }
            get { return _pichight; }
        }

        /// <summary>
        /// 用于存放sql语句的List
        /// </summary>
        public List<string> LstSql
        {
            set { _lstSql = value; }
            get { return _lstSql; }
        }

        /// <summary>
        /// 用于存放曲线名称的List
        /// </summary>
        public List<string> LstName
        {
            set { _lstName = value; }
            get { return _lstName; }
        }

        public DataTable Dt
        {
            set { _dt = value; }
            get { return _dt; }
        }


        //默认构造函数
        public ChartHelper()
        {
            string path = HttpContext.Current.Server.MapPath(
                HttpContext.Current.Request.ApplicationPath) + "temp";

            _phaysicalimagepath = path;
        }

        /// <summary>
        /// 构造函数1
        /// </summary>
        /// <param name="Title">图片标题</param>
        /// <param name="XTitle">图片X轴标题</param>
        /// <param name="YTitle">图片Y轴标题</param>
        /// <param name="PicWidth">宽</param>
        /// <param name="PicHight">高</param>
        /// <param name="LstSql">用于存放sql语句的List</param>
        /// <param name="LstName">用于存放曲线名称的List</param>
        public ChartHelper(string Title, string XTitle, string YTitle,
            int PicWidth, int PicHight, List<string> LstSql, List<string> LstName)
        {
            string path = HttpContext.Current.Server.MapPath(
                HttpContext.Current.Request.ApplicationPath) + "temp";

            _phaysicalimagepath = path;

            _title = Title;
            _xtitle = XTitle;
            _ytitle = YTitle;
            _picwidth = PicWidth;
            _pichight = PicHight;
            _lstSql = LstSql;
            _lstName = LstName;
        }

        /// <summary>
        /// 构造函数2
        /// </summary>
        /// <param name="Title">图片标题</param>
        /// <param name="XTitle">图片X轴标题</param>
        /// <param name="YTitle">图片Y轴标题</param>
        /// <param name="LstSql">用于存放sql语句的List</param>
        /// <param name="LstName">用于存放曲线名称的List</param>
        public ChartHelper(string Title, string XTitle, string YTitle,
            List<string> LstSql, List<string> LstName)
        {
            string path = HttpContext.Current.Server.MapPath(
                HttpContext.Current.Request.ApplicationPath) + "temp";

            _phaysicalimagepath = path;

            _title = Title;
            _xtitle = XTitle;
            _ytitle = YTitle;
            _lstSql = LstSql;
            _lstName = LstName;
        }

        /// <summary>
        /// 构造函数3
        /// </summary>
        /// <param name="Title">图片标题</param>
        /// <param name="XTitle">X轴标题</param>
        /// <param name="YTitle">Y轴标题</param>
        /// <param name="PicWidth">宽</param>
        /// <param name="PicHight">高</param>
        /// <param name="dt">数据源</param>
        /// <param name="LstName">用于存放曲线名称的List</param>
        public ChartHelper(string Title, string XTitle, string YTitle,
        int PicWidth, int PicHight, DataTable dt, List<string> LstName)
        {
            string path = HttpContext.Current.Server.MapPath(
                HttpContext.Current.Request.ApplicationPath) + "temp";

            _phaysicalimagepath = path;

            _title = Title;
            _xtitle = XTitle;
            _ytitle = YTitle;
            _picwidth = PicWidth;
            _pichight = PicHight;
            _dt = dt;
            _lstName = LstName;
        }

        /// <summary>
        /// 构造函数4
        /// </summary>
        /// <param name="Title">图片标题</param>
        /// <param name="XTitle">X轴标题</param>
        /// <param name="YTitle">Y轴标题</param>
        /// <param name="dt">数据源</param>
        /// <param name="LstName">用于存放曲线名称的List</param>
        public ChartHelper(string Title, string XTitle, string YTitle,
            DataTable dt, List<string> LstName)
        {
            string path = HttpContext.Current.Server.MapPath(
                HttpContext.Current.Request.ApplicationPath) + "temp";

            _phaysicalimagepath = path;

            _title = Title;
            _xtitle = XTitle;
            _ytitle = YTitle;
            _dt = dt;
            _lstName = LstName;
        }

        /// <summary>
        /// 根据sql 语句获取各条曲线
        /// </summary>
        /// <param name="chart">dotNetChart</param>
        /// <param name="strLst">用于存放sql语句的List</param>
        public void getSeries(dotnetCHARTING.Chart chart, string strConn)
        {
            DeletetongjiPNG(this._phaysicalimagepath);
            chart.TempDirectory = this._phaysicalimagepath;

            chart.SeriesCollection.Clear();

            DataEngine _de = new DataEngine();//图片数据源
            _de.ConnectionString = strConn;

            for (int i = 0; i < this._lstSql.Count; i++)
            {
                _de.SqlStatement = this._lstSql[i];
                chart.SeriesCollection.Add(_de.GetSeries());
            }
        }

        /// <summary>
        /// 根据dataTable获取各条曲线
        /// </summary>
        /// <param name="chart">dotNetChart</param>
        /// <param name="lstFieldsY">Y轴字段</param>
        /// <param name="fieldX">X轴字段</param>
        public void getSeries(dotnetCHARTING.Chart chart, List<string> lstFieldsY, string fieldX)
        {
            DeletetongjiPNG(this._phaysicalimagepath);
            chart.TempDirectory = this._phaysicalimagepath;

            chart.SeriesCollection.Clear();

            for (int i = 0; i < lstFieldsY.Count; i++)
            {
                Series series = new Series();

                for (int j = 0; j < _dt.Rows.Count; j++)
                {
                    Element element = new Element();
                    double y = 0;
                    if (_dt.Rows[j][lstFieldsY[i]].ToString() != "")
                        y = double.Parse(_dt.Rows[j][lstFieldsY[i]].ToString());
                    element.YValue = y;
                    element.Name = _dt.Rows[j][fieldX].ToString();
                    series.Elements.Add(element);
                }
                chart.SeriesCollection.Add(series);
            }
        }

        /// <summary>
        /// 根据dataTable获取各条曲线
        /// </summary>
        /// <param name="chart">dotNetChart</param>
        /// <param name="lstFieldsY">Y轴字段</param>
        /// <param name="fieldX">X轴字段</param>
        public void getSeries(dotnetCHARTING.Chart chart, string fieldY, string fieldX)
        {
            DeletetongjiPNG(this._phaysicalimagepath);
            chart.TempDirectory = this._phaysicalimagepath;

            Series series = new Series();
            for (int i = 0; i < _dt.Rows.Count; i++)
            {
                Element element = new Element();

                element.Name = _dt.Rows[i][fieldX].ToString();
                double y = 0;
                if (_dt.Rows[i][fieldY].ToString() != "")
                    y = double.Parse(_dt.Rows[i][fieldY].ToString());
                element.YValue = y;
                series.Elements.Add(element);
            }

            chart.SeriesCollection.Add(series);
        }

        /// <summary>
        /// 设置曲线的基本属性
        /// </summary>
        /// <param name="chart">dotNetChart</param>
        /// <param name="Title">标题</param>
        /// <param name="XTitle">x座标名称</param>
        /// <param name="YTitle">y座标名称</param>
        private void setChart(dotnetCHARTING.Chart chart)
        {
            chart.Title = this._title;
            chart.XAxis.Label.Text = this._xtitle;
            chart.YAxis.Label.Text = this._ytitle;

            if (this._picwidth > 0)
                chart.Width = this._picwidth;
            if (this._pichight > 0)
                chart.Height = this._pichight;

            chart.Series.DefaultElement.ShowValue = true;
            chart.DefaultSeries.DefaultElement.ShowValue = true;
        }

        /// <summary>
        /// 设置曲线类型
        /// </summary>
        /// <param name="chart">dotNetChart</param>
        /// <param name="chartType">dotNetChart类型</param>
        /// <param name="seriesType">dotNetChart曲线类型</param>
        private void setChartType(dotnetCHARTING.Chart chart,
            dotnetCHARTING.ChartType chartType, dotnetCHARTING.SeriesType seriesType)
        {
            chart.Type = chartType;

            for (int i = 0; i < chart.SeriesCollection.Count; i++)
            {
                chart.SeriesCollection[i].Type = seriesType;
                chart.SeriesCollection[i].Name = this._lstName[i];
            }
        }

        /// <summary>
        /// 柱状图
        /// </summary>
        /// <param name="chart">dotNetChart</param>
        public void CreateBar(dotnetCHARTING.Chart chart)
        {
            setChart(chart);
            setChartType(chart, dotnetCHARTING.ChartType.Combo, dotnetCHARTING.SeriesType.Cylinder);
        }

        /// <summary>
        /// 饼状图
        /// </summary>
        /// <param name="chart">dotNetChart</param>
        public void CreatePie(dotnetCHARTING.Chart chart)
        {
            setChart(chart);
            setChartType(chart, dotnetCHARTING.ChartType.Pie, dotnetCHARTING.SeriesType.Cylinder);
        }

        /// <summary>
        /// 线形图
        /// </summary>
        /// <param name="chart">dotNetChart</param>
        public void CreateLines(dotnetCHARTING.Chart chart)
        {
            setChart(chart);
            setChartType(chart, dotnetCHARTING.ChartType.Combo, dotnetCHARTING.SeriesType.Line);
        }

        /// <summary>
        /// 删除临时文件夹里的图片
        /// </summary>
        /// <param name="Path">图片存放路径</param>
        private void DeletetongjiPNG(string Path)
        {
            string dirPath;
            dirPath = Path;
            DirectoryInfo adminDir = new DirectoryInfo(dirPath);
            foreach (FileInfo fi in adminDir.GetFiles())
            {
                if (fi.Extension.ToLower() == ".png")
                {
                    fi.Delete();
                    break;
                }
            }
        }
    }

}
