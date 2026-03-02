namespace Satluj_Latest.Models
{

    public partial class sp_UnDeclaredExamSubjects_Result
    {
        public sp_UnDeclaredExamSubjects_Result()
        {
            
        }
        public long SubId { get; set; }
        public long SchoolI { get; set; }
        public string SubjectName { get; set; }
        public bool IsActive { get; set; }
        public DateTime TmeStamp { get; set; }
        public bool IsOptonal { get; set; }
        public string Abbreviation { get; set; }
        public string Code { get; set; }
        public int? EnumTypeId { get; set; }
        public bool? HavePractical { get; set; }
    }
}
