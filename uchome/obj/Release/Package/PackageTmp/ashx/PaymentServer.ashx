<%@ WebHandler Language="C#" Class="PaymentServer" %>

using System.Web;
using UCHome.PayService;

public class PaymentServer : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "text/plain";
        EcloudSevice serverSevice=new EcloudSevice();
          string method = context.Request["Method"];
        string userID = context.Request["UserID"];
        //userID = "DC86E691-1D53-486B-8E20-74F30F008557";
        switch (method)
        {
            case "getOrder":
                context.Response.Write(serverSevice.getOrder(userID));
                break;
            case "getDealByCount":
                string count = context.Request["Count"];
                context.Response.Write(serverSevice.getDealByCount(userID,count));
                break;
        }
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}