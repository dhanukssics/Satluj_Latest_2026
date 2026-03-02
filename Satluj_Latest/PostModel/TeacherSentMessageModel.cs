using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;


namespace Satluj_Latest.PostModel
{
    public class TeacherSentMessageModel
    {
        public string TeacherId { get; set; }
        public string ToSentId { get; set; }
        public string Subject { get; set; }
        public string Descritpion { get; set; }
        public Satluj_Latest.MessageType MessageType { get; set; }
        public IFormFile PostFile { get; set; }
    }
}
