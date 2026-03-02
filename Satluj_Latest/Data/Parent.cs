using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class Parent : BaseReference
    {
        private TbParent parent;
        public Parent(TbParent obj) { parent = obj; }
        public Parent(long id) { parent = _Entities.TbParents.FirstOrDefault(z => z.ParentId == id); }
        public long ParentId { get { return parent.ParentId; } }
        public string ParentName { get { return parent.ParentName; } }
        public string Address { get { return parent.Address; } }
        public string City { get { return parent.City; } }
        public string Email { get { return parent.Email; } }
        public string ContactNumber { get { return parent.ContactNumber; } }
        public string Password { get { return parent.Password; } }
        public System.DateTime TimeStamp { get { return parent.TimeStamp; } }
        public System.Guid ParentGuid { get { return parent.ParentGuid; } }
        public bool IsActive { get { return parent.IsActive; } }
        public string State { get { return parent.State; } }
        public string FilePath { get { return parent.FilePath; } }
        public List<Student> GetChildren()
        {
            var data = _Entities.TbStudents.Where(x => x.ParentId == ParentId && x.IsActive).ToList().Select(x => new Student(x)).ToList();
            return data;
        }
        public List<EventsList> ParentEventList()
        {
            List<EventsList> list = new List<EventsList>();
            var studentList = _Entities.TbStudents.Where(x => x.ParentId == parent.ParentId && x.IsActive && x.School.IsActive).ToList();
            var schoolList = studentList.Select(x => x.SchoolId).Distinct().ToList();
            foreach (var item in schoolList)
            {
                var listOfEvents = new Satluj_Latest.Data.School(item).GetCalendarUpcomingEvent();
                foreach (var even in listOfEvents)
                {
                    EventsList one = new EventsList();
                    one.EventId = even.EventId;
                    one.EventHead = even.EventHead;
                    one.EventDate = even.EventDate;
                    one.Description = even.EventDetails;
                    list.Add(one);
                }
            }
            return list.OrderByDescending(x => x.EventDate).ToList();
        }
        public List<CircularsList> ParentCircularList()
        {
            List<CircularsList> list = new List<CircularsList>();
            var studentList = _Entities.TbStudents.Where(x => x.ParentId == parent.ParentId && x.IsActive && x.School.IsActive).ToList();
            var schoolList = studentList.Select(x => x.SchoolId).Distinct().ToList();
            foreach (var item in schoolList)
            {
                var listOfEvents = new Satluj_Latest.Data.School(item).AllCircularList();
                foreach (var cir in listOfEvents)
                {
                    CircularsList one = new CircularsList();
                    one.CircularId = cir.CircularId;
                    one.FilePath = cir.FilePath;
                    one.CircularDate = cir.CircularDate;
                    one.Description = cir.Description;
                    one.CircularHead = cir.CircularHead;
                    list.Add(one);
                }
            }
            return list.OrderByDescending(x => x.CircularDate).ToList();
        }


    }
}
