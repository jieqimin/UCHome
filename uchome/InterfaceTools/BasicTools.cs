using ResourceLibrary.Search;
using ResourceLibrary.Services;
using SimplifyDDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace UCHome.InterfaceTools
{
    public class BasicTools
    {
        public List<ResourceInfo> GetResourceByUser(string elementName, string elementValue, string userid, int top)
        {
            var service = new BasicToolsModel();

            var ResourcesList = service.GetResourceByUser(elementName, elementValue, userid, top);

            return ResourcesList;
        }
        public List<ResourceInfo> GetResourceByBJ(string bjid, int top)
        {
            var service = new BasicToolsModel();

            var ResourcesList = service.getResourceByBJ(bjid, top);

            return ResourcesList;
        }
        public List<ResourceInfo> GetResourceBySchool(string xxid, int top)
        {
            var service = new BasicToolsModel();

            var ResourcesList = service.getResourceBySchool(xxid, top);

            return ResourcesList;
        }
    }
}