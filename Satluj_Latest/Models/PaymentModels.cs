using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class PaymentModels
    {
        public string CustomGuid { get; set; }
        public string HashValue { get; set; }
        public string SecureHash { get; set; }
        public string PaymentStatus { get; set; }
        public SortedDictionary<string, string> ConfirmData { get; set; }
        public SortedDictionary<string, string> sortedDict { get; set; }
        public string Channel { get; set; }
        public string ReferenceNo { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
        public string ReturnUrl { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
        public long userId { get; set; }
        public Guid ExamGuid { get; set; }
        public string strAccessCode { get; set; }
        public string strEncRequest { get; set; }
        public string iframeSrc { get; set; }
        public string CourseName { get; set; }
        public bool paymentStatus { get; set; }
        public long BillNo { get; set; }
        public long SchoolId { get; set; }
        public long StudentId { get; set; }



    }
}