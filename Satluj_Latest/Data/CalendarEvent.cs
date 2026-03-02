using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class CalendarEvent : BaseReference
    {
        private TbCalenderEvent calendarEvent;
        public CalendarEvent(TbCalenderEvent obj) { calendarEvent = obj; }
        public CalendarEvent(long id) { calendarEvent = _Entities.TbCalenderEvents.FirstOrDefault(z => z.EventId == id); }
        public long eventId { get { return calendarEvent.EventId; } }
        public string eventHead { get { return calendarEvent.EventHead; } }
        public string eventDetail { get { return calendarEvent.EventDetails; } }
        public System.DateTime eventDate { get { return calendarEvent.EventDate; } }
        public System.DateTime TimeStamp { get { return calendarEvent.TimeStamp; } }
        public bool IsActive { get { return calendarEvent.IsActive; } }
      
    }
}

