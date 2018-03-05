using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using UCHome.Model;

namespace App.Controllers
{
    public class EnergyController : AsyncController
    {
        //
        // GET: /Energy/
        public ActionResult IndexAsync()
        {
            UCHomeBasePage ubp = new UCHomeBasePage();
            string xxid = ubp.CurrentSch.ToString().ToUpper();
            int year = DateTime.Now.Year;
            AsyncManager.OutstandingOperations.Increment(1);
            Task.Factory.StartNew(() =>
            {
                //var url = "http://111.17.215.232:8096/Statistics/EnergyConsumption/GetBuildDayJson?year=2015&month=8&buildId=64F0BF0D-7D4C-4EF1-B6B6-96D1576FA7A6&type=01";//逐日用 电/水 状况
                //var url = "http://111.17.215.232:8096/Statistics/EnergyConsumption/GetBuildMonthJson?year=2015&buildId=64F0BF0D-7D4C-4EF1-B6B6-96D1576FA7A6&type=01";//逐月用 电/水 状况
                //var url = "http://111.17.215.232:8096/Statistics/EnergyConsumption/GetBuildCompareJson?year=2015&month=8&buildId=64F0BF0D-7D4C-4EF1-B6B6-96D1576FA7A6&type=01"; //按节点比较
                string url =
                    string.Format(
                        "http://172.25.0.108/Statistics/EnergyConsumption/GetBuildMonthJson?year={0}&buildId={1}&type=01",
                        year, xxid);
                ViewBag.xxid = xxid;
                using (HttpClient httpClient = new HttpClient())
                {
                    var response = httpClient.GetStringAsync(url).Result;
                    AsyncManager.Parameters["data"] = response;
                    AsyncManager.OutstandingOperations.Decrement();
                }
            });
          
            return View();
        }
        public ActionResult IndexCompleted(string data)
        {
            return View(JsonConvert.DeserializeObject<StatisticsViewModel>(data));
        }
    }
    public class StatisticsViewModel
    {
        public string Highcharts { get; set; }
        public string Table { get; set; }
    }
}
