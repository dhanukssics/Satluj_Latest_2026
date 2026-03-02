namespace Satluj_Latest.Models
{
    public partial class sp_StudentsWithNoOptionalSubjects_Result
    {
        public sp_StudentsWithNoOptionalSubjects_Result()
        {
            
        }
        public long StudentId { get; set; }
        public long SchoolId { get; set; }
        public string StudentSpecialId { get; set; }
        public string StundentName { get; set; }
        public string ClasssNumber { get; set; }
        public string FilePath { get; set; }
    }
}
