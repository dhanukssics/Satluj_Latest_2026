using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Satluj_Latest;

namespace Satluj_Latest.Models
{
    public class UserModuleModel
    {
        public long SchoolId { get; set; }
        public List<MainModuleList> mainList { get; set; }
        public UserTypeModule userType {get;set;}
        public string UserTypeName { get; set; }
        public List<SubModuleList> subListOnly { get; set; }
        public long UserTypeId { get; set; }
        public bool IsAdmin { get; set; }
    }
    public class MainModuleList
    {
        public long Id { get; set; }
        public string ModuleName { get; set; }
        public List<SubModuleList> subList { get; set; }
        public string subIdListString { get; set; }
        public bool IsExistsMain { get; set; }
    }
    public class SubModuleList
    {
        public long Id { get; set; }
        public long MainId { get; set; }
        public string SubMosule { get; set; }
        public bool IsExists { get; set; }
    }
}