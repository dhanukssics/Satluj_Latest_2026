namespace Satluj_Latest.Models
{
    public class OptionalStudentDto
    {
        public long StudentId { get; set; }
        public long SchoolId { get; set; }

        public string? StudentSpecialId { get; set; }
        public string? StundentName { get; set; }
        public string? ClasssNumber { get; set; }
        public string? FilePath { get; set; }
        public string? ParentName { get; set; }
        public string? Address { get; set; }
        public string? ContactNumber { get; set; }
    }
}
