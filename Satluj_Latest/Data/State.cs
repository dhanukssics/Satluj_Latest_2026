using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class State:BaseReference
    {
        private TbState state;
        public State(TbState obj) { state = obj; }
        public State(long id) { state = _Entities.TbStates.FirstOrDefault(z => z.StateId == id); }
        public long StateId { get { return state.StateId; } }
        public string StateName { get{ return state.State; }}
        public System.Guid StateGuid { get { return state.StateGuid; }}
        public bool IsActive { get { return state.IsActive; }}
    }
}
