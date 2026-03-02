using Satluj_Latest.Data;
using Satluj_Latest.Models;

namespace Satluj_Latest.Models.ViewModels
{
    public class StaffListViewModel
    {
        public long SchoolId { get; set; }
        public List<Staff> Staffs { get; set; }
        public bool IsAdmin { get; set; }
    }

}
