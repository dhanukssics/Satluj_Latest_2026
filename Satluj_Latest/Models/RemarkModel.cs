using System.ComponentModel.DataAnnotations;

namespace Satluj_Latest.Models
{
    public class RemarkModel
    {
        public long SchoolId { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Remark { get; set; }
        public long RemarkId { get; set; }
    }
}
