using Satluj_Latest.Data;
using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Repository
{
    public class StateRepository
    {
        private readonly SchoolDbContext _Entity;

        //public tb_Satluj_LatestEntities _Entity = new tb_Satluj_LatestEntities();

        public Tuple<bool, string, List<State>> AllStateList()
        {
            var status = false;
            var msg = "failed";
            var stateData = _Entity.TbStates.Where(x => x.IsActive == true).ToList().Select(x => new State(x)).ToList();
            //var stateData = StaticData.fetchState();
            if (stateData.Count>0)
            {
                status = true;
                msg = "Success";
                return new Tuple<bool, string, List<State>>(status, msg, stateData);
            }
            else
            {
                status = false;
                msg = "failed";
                return new Tuple<bool, string, List<State>>(status, msg, stateData);
            }
        }
    }
}
