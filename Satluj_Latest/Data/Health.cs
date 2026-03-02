using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class Health : BaseReference
    {
        private TbHealth h;
        public Health(TbHealth obj) { h = obj; }
        public Health(long id) { h = _Entities.TbHealths.FirstOrDefault(z => z.Id == id); }
        public long Id { get { return h.Id; } }
        public long ClassId { get { return h.ClassId; } }
        public long DivisionId { get { return h.DivisionId; } }
        public long PeriodsId { get { return h.PeriodsId; } }
        public long StudentId { get { return h.StudentId; } }
        public decimal Height { get { return h.Height; } }
        public decimal Weight { get { return h.Weight; } }
        public bool IsActive { get { return h.IsActive; } }
        public System.DateTime TimeStamp { get { return h.TimeStamp; } }
    }
}
