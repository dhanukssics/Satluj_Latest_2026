using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;



namespace Satluj_Latest.PostModel
{
  public  class ParentMessageSendPostModel
    {
        public string ParentId { get; set; }
        public string StudentId { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        //public HttpPostedFile PostFile { get; set; }
        public IFormFile PostFile { get; set; }

  }
}
