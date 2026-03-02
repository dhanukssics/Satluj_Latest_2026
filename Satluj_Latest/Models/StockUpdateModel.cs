
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Satluj_Latest.Models
{
    public class StockUpdateModel
    {
        public long CategoryId { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Item { get; set; }
        [Required(ErrorMessage = "Required")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Required")]
        public string PurchaseId { get; set; }
        [Required(ErrorMessage = "Required")]
        public string SupplirName { get; set; }
        [Required(ErrorMessage = "Required")]
        public StockStatus Status { get; set; }
        [Required(ErrorMessage = "Required")]
        public long SchoolId { get; set; }
        public long StockId { get; set; }

        public List<SelectListItem> LabCategories { get; set; }

    }
}