using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class SchoolSenderId : BaseReference
    {
        private TbSchoolSenderId senderId;
        public SchoolSenderId(TbSchoolSenderId obj) { senderId = obj; }
        public SchoolSenderId(long id) { senderId = _Entities.TbSchoolSenderIds.FirstOrDefault(z => z.Id == id); }

        

        public long Id { get { return senderId.Id; } }
        public long SchoolId { get { return senderId.SchoolId; } }
        public string SenderId { get { return senderId.SenderId; } }
        public Nullable<bool> IsActive { get { return senderId.IsActive; } }

        public TbSchoolSenderId X { get; }
    }
}
