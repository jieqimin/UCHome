using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Mvc;

namespace UCHome.Models {
	public class HttpClientHelper : AsyncController
    {
        public static string GETMethod(string apiurl)
        {
            var handler = new HttpClientHandler();
            using (var httpClient = new HttpClient(handler))
            {
                //添加cors跨域
                httpClient.DefaultRequestHeaders.Add("KeepAlive", "false");
                var response = httpClient.GetAsync(apiurl);
                return response.Result.Content.ReadAsStringAsync().Result;
            }
        }
        public static string POSTMethod<T>(string apiurl, [FromBody]T model)
        {
            var handler = new HttpClientHandler();
            using (var httpClient = new HttpClient(handler))
            {
                //添加cors跨域
                httpClient.DefaultRequestHeaders.Add("KeepAlive", "false");
                var requestJson = JsonConvert.SerializeObject(model);
                HttpContent httpcontent = new StringContent(requestJson);
                httpcontent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = httpClient.PostAsync(apiurl, httpcontent);
                return response.Result.Content.ReadAsStringAsync().Result;
            }
        }
        public static string POSTMethod(string apiurl, params object[] param)
        {
            var handler = new HttpClientHandler();
            using (var httpClient = new HttpClient(handler))
            {
                httpClient.DefaultRequestHeaders.Add("KeepAlive", "false");
                var requestJson = JsonConvert.SerializeObject(param);
                HttpContent httpcontent = new StringContent(requestJson);
                httpcontent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = httpClient.PostAsync(apiurl, httpcontent);
                return response.Result.Content.ReadAsStringAsync().Result;
            }
        }

        public static string PUTMethod(string apiurl, params object[] param)
        {
            var handler = new HttpClientHandler();
            using (var httpClient = new HttpClient(handler))
            {
                httpClient.DefaultRequestHeaders.Add("KeepAlive", "false");
                var requestJson = JsonConvert.SerializeObject(param);
                HttpContent httpcontent = new StringContent(requestJson);
                httpcontent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = httpClient.PostAsync(apiurl, httpcontent);
                return response.Result.Content.ReadAsStringAsync().Result;
            }
        }
    }
}