using System.ComponentModel.Design;
using MetadataCenter.Services;
using ResourceLibrary.Search;
using ResourceLibrary.Services;
using SimplifyDDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UCHome.Model;
using UCModel;

namespace UCHome.InterfaceTools
{
    public class BasicToolsModel
    {
        public  readonly IResourceService _ResourceService;
        private UCHome.Model.UCHomeEntities ucdb = new UCHomeEntities();
        public  BasicToolsModel() {
            _ResourceService= IoCFactory.Resolve<IResourceService>(); 
        }
        public string ModelId
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["ResourceModelId"]; }
        }
        public List<ResourceInfo> GetResourceByUser(string elementName, string elementValue, string userid, int top)
        {
            List<ResourceInfo> resouces = new List<ResourceInfo>();
            //resouces.Add(new ResourceInfo()
            //{
            //    Title = "资源1"
            //});
            //resouces.Add(new ResourceInfo()
            //{
            //    Title = "资源2"
            //});
            //resouces.Add(new ResourceInfo()
            //{
            //    Title = "资源3"
            //});
            var searchCriterias = new List<SearchCriteria>();
            searchCriterias.Add(new SearchCriteria()
            {
                FilterType = FilterType.ElementName,
                FilterOperator = FilterOperator.Equals,
                FilterValue = elementName,
                Value = elementValue
            });
            var reourcesList =_ResourceService.GetResourcesByModelId(ModelId).Search(searchCriterias).Where(x => x.CreatorId == userid).OrderByDescending(o => o.CreateTime).Take(top);
            resouces = reourcesList.Select(s => new ResourceInfo { Id = s.Id, Title = s.DisplayName, ShowTime = s.CreateTime }).ToList();
            return resouces;
        }
        //根据班级获取前top条资源
        public List<ResourceInfo> getResourceByBJ(string bjid, int top)
        {
            Guid _bjid = Guid.Parse(bjid);
            //获取全班人员
            List<ResourceInfo> resouces = new List<ResourceInfo>();
            var userids = ucdb.View_Simple_StuInfo.Where(x=>x.BJID==_bjid).Select(s=>s.xxid.ToString());
            var reourcesList = _ResourceService.GetResourcesByModelId(ModelId).Where(x => userids.Contains(x.CreatorId)).OrderByDescending(o => o.CreateTime).Take(top);
            resouces = reourcesList.Select(s => new ResourceInfo { Id = s.Id, Title = s.DisplayName, ShowTime = s.CreateTime }).ToList();
            return resouces;
        }
        //根据学校获取前top条资源
        public List<ResourceInfo> getResourceBySchool(string xxid, int top)
        {
            Guid _xxid = Guid.Parse(xxid);
            List<ResourceInfo> resouces = new List<ResourceInfo>();
            //获取全校人员
          Wicresoft.Framework.Organization.User[] users=  Wicresoft.Framework.Organization.OrganizationHelper.GetUserInfoListByDepartment(_xxid,false);
         var userids= users.Select(s => s.ID.ToString());
         var reourcesList = _ResourceService.GetResourcesByModelId(ModelId).Where(x => userids.Contains(x.CreatorId)).OrderByDescending(o => o.CreateTime).Take(top);
         resouces = reourcesList.Select(s => new ResourceInfo { Id = s.Id, Title = s.DisplayName, ShowTime = s.CreateTime }).ToList();
         return resouces;
        }
    }
}