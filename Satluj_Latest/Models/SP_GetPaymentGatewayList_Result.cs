namespace Satluj_Latest.Models
{
    public class SP_GetPaymentGatewayList_Result
    {

       
            public int Id { get; set; }
            public string GatewayName { get; set; }
            public decimal Amount { get; set; }
            public long SchoolId { get; set; }
            public string SchoolName { get; set; }
            public string StundentName { get; set; }
            public string StudentSpecialId { get; set; }
            public string Class { get; set; }
            public int ClassId { get; set; }
            public string Division { get; set; }
            public int DivisionId { get; set; }
            public string BillNo { get; set; }
            public long StudentId { get; set; }
            public DateTime TimeStamp { get; set; }
            public decimal PayAmount { get; set; }
            public decimal CCAmount { get; set; }

    }
}
