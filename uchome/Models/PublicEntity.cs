using System;
using System.Collections.Generic;

namespace UCHome.Model
{
    public class MyClass
    {
        public Guid xxid { get; set; }
        public Guid bjid { get; set; }
        public string bjmc { get; set; }
    }

    public class UserInfo
    {
        public string areacode { get; set; }
        public Guid xxid { get; set; }
        public string xxmc { get; set; }
        public Guid userid { get; set; }
        public string username { get; set; }
        public string loginname { get; set; }
        public Guid? orgid { get; set; }
        public string orgname { get; set; }
        public string usertype { get; set; }
        public string status { get; set; }
        public string headpic { get; set; }
        public string subject { get; set; }
        public string skintheme { get; set; }
        public string skincss { get; set; }
        public ChildInfo childinfo { get; set; }
    }

    public class ChildInfo
    {
        public string childareacode { get; set; }
        public Guid childxxid { get; set; }
        public string childxxmc { get; set; }
        public Guid childbjid { get; set; }
        public string childbjmc { get; set; }
        public Guid childuserid { get; set; }
        public string childusername { get; set; }
    }
    public class RedisUser
    {
        public string AreaCode { get; set; }
        public Guid XXID { get; set; }
        public string XXMC { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public Guid? OrgId { get; set; }
        public string OrgName { get; set; }
        public string UserType { get; set; }
        public string SFZJH { get; set; }
        public string Email { get; set; }
        public string Tel { get; set; }
    }
    public class Article
    {
        public Guid PKID { get; set; }
        public string Title { get; set; }
        public string DeployTime { get; set; }

        public Article(Guid pkid, string title, DateTime deploTime)
        {
            PKID = pkid;
            DeployTime = deploTime.ToString("yyyy/MM/dd");
            if (title.Length > 25)
            {
                title = title.Substring(0, 25) + "...";
            }
            Title = title;
        }
    }
    public class NewSchoolMessage  //zy  学校资源_学校通知list
    {
        public string MsgTitle { get; set; }
        public DateTime MsgDate { get; set; }
        public Guid xxid { get; set; }
        public Guid ID { get; set; }
        public string FileUrl { get; set; }
        public string MsgSendUserName { get; set; }
        public string MsgContent { get; set; }
        public string FileID { get; set; }
        public NewSchoolMessage() { }



        public NewSchoolMessage(Guid id, string msgtitle, string msgsendusername, DateTime msgdate, string msgcontent, string fileid, string fileUrl)
        {
            ID = id;
            MsgTitle = msgtitle;
            MsgSendUserName = msgsendusername;
            MsgDate = msgdate;
            MsgContent = msgcontent;
            FileID = fileid;
            FileUrl = fileUrl;
        }


    }

    //public class ArticleHits
    //{
    //    public Guid PKID { get; set; }
    //    public string Title { get; set; }
    //    public string DisplayName { get; set; }
    //    public int Hits { get; set; }
    //}

    public class NewArticle
    {
        public Guid PKID { get; set; }
        public string Title { get; set; }
        public DateTime DeployTime { get; set; }
        public string Author { get; set; }
        public bool IsNew { get; set; }
        public NewArticle()
        {

        }
        public Guid xxid { get; set; }

        public NewArticle(Guid pkid, string title, DateTime deploTime, string author)
        {
            PKID = pkid;
            DeployTime = deploTime;
            if (string.IsNullOrWhiteSpace(author))
            {
                author = "匿名";
            }
            Author = author;
            string newTitle = author + "：" + title;
            if (newTitle.Length > 15)
            {
                newTitle = newTitle.Substring(0, 15) + "...";
            }
            Title = newTitle;
            if (DateTime.Compare(deploTime, DateTime.Now.AddDays(-3)) > 0)
            {
                IsNew = true;
            }
            else
            {
                IsNew = false;
            }
        }
    }
    public class HotArticle
    {
        public Guid PKID { get; set; }
        public string Title { get; set; }
        public DateTime DeployTime { get; set; }
        public string Author { get; set; }
        public HotArticle()
        {

        }
        public HotArticle(Guid pkid, string title, DateTime deploTime, string author)
        {
            PKID = pkid;
            DeployTime = deploTime;
            if (string.IsNullOrWhiteSpace(author))
            {
                author = "匿名";
            }
            Author = author;
            if (title.Length > 15)
            {
                title = title.Substring(0, 15) + "...";
            }

            Title = title;
        }
    }
    public class HotArticles
    {
        public Guid PKID { get; set; }
        public string Title { get; set; }
        public DateTime DeployTime { get; set; }
        public string Author { get; set; }
        public HotArticles(Guid pkid, string title, DateTime deploTime, string author)
        {
            this.PKID = pkid;
            this.DeployTime = deploTime;
            if (string.IsNullOrWhiteSpace(author))
            {
                author = "匿名";
            }
            this.Author = author;
            if (title.Length > 10)
            {
                title = title.Substring(0, 10) + "...";
            }

            this.Title = title;
        }
    }

    public class ArticleInfo
    {
        public Guid PKID { get; set; }
        public string Title { get; set; }
        public DateTime DeployTime { get; set; }
        public string Author { get; set; }
        public int Hits { get; set; }
        public string Content { get; set; }
        public Guid? AddUser { get; set; }
        public string UserType { get; set; }
        public ArticleInfo()
        { }
        public ArticleInfo(Guid pkid, string title, DateTime deploTime, string author, int hits, string content, Guid? adduser, string usertype = "t")
        {
            PKID = pkid;
            DeployTime = deploTime;
            if (string.IsNullOrWhiteSpace(author))
            {
                author = "匿名";
            }
            Author = author;
            Hits = hits;
            Title = title;
            Content = content;
            AddUser = adduser;
            UserType = usertype;
        }
    }
    public class Room
    {
        public Guid? PKID { get; set; }
        public Guid? UserId { get; set; }
        public string UserName { get; set; }
        public string UserType { get; set; }
        public Guid? XXID { get; set; }
        public string XZQHM { get; set; }
        public string headphoto { get; set; }
        public int Count { get; set; }
    }
    public class MyResource
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }
        public int Hits { get; set; }
        public string Subject { get; set; }
        public string CreatorName { get; set; }
        public string SchoolName { get; set; }
        public string LinkUrl { get; set; }
        public MyResource(string id, string name, DateTime createTime, int hits, string grade, string subject, string XXMC, string creatorName, string linkUrl)
        {
            Id = id;
            if (name.Length > 15)
            {
                name = name.Substring(0, 15) + "...";
            }
            Name = name;
            CreateTime = createTime;
            Hits = hits;
            CreatorName = creatorName;
            Subject = grade + subject;
            LinkUrl = linkUrl;
            UCHomeEntities _uc = new UCHomeEntities();

            if (string.IsNullOrWhiteSpace(XXMC))
            {
                SchoolName = "管理员";
            }
            else
            {
                SchoolName = XXMC;
            }
        }
    }
    public class Resources
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }
        public int Hits { get; set; }
        public string Subject { get; set; }
        public string CreatorName { get; set; }
        public string SchoolName { get; set; }
        public Resources(string id, string name, DateTime createTime, int hits, string grade, string subject, string XXMC, string creatorName)
        {
            this.Id = id;
            if (name.Length > 10)
            {
                name = name.Substring(0, 10) + "...";
            }
            this.Name = name;
            this.CreateTime = createTime;
            this.Hits = hits;
            this.CreatorName = creatorName;
            this.Subject = grade + subject;
            UCHomeEntities _uc = new UCHomeEntities();

            if (string.IsNullOrWhiteSpace(XXMC))
            {
                this.SchoolName = "管理员";
            }
            else
            {
                this.SchoolName = XXMC;
            }
        }
    }
    public class School
    {
        public Guid XXID { get; set; }
        public string XXMC { get; set; }
        public string NewMC { get; set; }
    }
    public class NewSchool
    {
        public Guid XXID { get; set; }
        public string XXMC { get; set; }
        public string NewMC { get; set; }

        public NewSchool(Guid xxid, string xxmc)
        {
            XXID = xxid;
            XXMC = xxmc;
            if (xxmc.Length > 12)
            {
                NewMC = xxmc.Substring(0, 12) + "...";
            }
            else
            {
                NewMC = xxmc;
            }
        }
    }
    public class NewSchool8
    {
        public Guid XXID { get; set; }
        public string XXMC { get; set; }
        public string NewMC { get; set; }

        public NewSchool8(Guid xxid, string xxmc)
        {
            XXID = xxid;
            XXMC = xxmc;
            if (xxmc.Length > 8)
            {
                NewMC = xxmc.Substring(0, 8) + "...";
            }
            else
            {
                NewMC = xxmc;
            }
        }
    }
    public class SchoolInfo
    {
        public Guid xxid { get; set; }
        public string xxmc { get; set; }
        public string address { get; set; }
        public string schoolInfo { get; set; }

        public SchoolInfo(Guid xxid, string xxmc, string xxdz, string clbj)
        {
            this.xxid = xxid;
            this.xxmc = xxmc;
            address = string.IsNullOrWhiteSpace(xxdz) ? "暂无" : xxdz;
            if (string.IsNullOrWhiteSpace(clbj))
            {
                schoolInfo = "暂无";
            }
            else
            {
                if (clbj.Length > 180)
                {
                    schoolInfo = clbj.Substring(0, 180) + "...";
                }
                else
                {
                    schoolInfo = clbj;
                }
            }

        }
    }
    public class SchoolRank
    {
        public Guid? XXID { get; set; }
        public string XXMC { get; set; }
        public int Hits { get; set; }
        public string Title { get; set; }
    }
    public class ArticleHits
    {
        public Guid PKID { get; set; }
        public string Title { get; set; }
        public string DisplayName { get; set; }
        public int Hits { get; set; }
    }
    public class SClass
    {
        public Guid classid { get; set; }
        public string classname { get; set; }

        public SClass(Guid bjid, string xzbmc, int? year)
        {
            classid = bjid;
            string yearName;
            switch (year)
            {
                case 1:
                    yearName = "一年级";
                    break;
                case 2:
                    yearName = "二年级";
                    break;
                case 3:
                    yearName = "三年级";
                    break;
                case 4:
                    yearName = "四年级";
                    break;
                case 5:
                    yearName = "五年级";
                    break;
                case 6:
                    yearName = "六年级";
                    break;
                default:
                    yearName = "";
                    break;
            }
            classname = yearName + xzbmc;
        }
    }
    public class TempClass
    {
        public Guid bjid { get; set; }
        public string xzbmc { get; set; }
        public int? year { get; set; }
        public string njdm { get; set; }
        public Guid xxid { get; set; }
    }
    public class Data
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
    public class Count
    {
        public string Key { get; set; }
        public int Value { get; set; }
    }

    public class ArticlesBySch
    {
        public Guid xxid { get; set; }
        public string xxmc { get; set; }
        public int ucount { get; set; }
    }
}