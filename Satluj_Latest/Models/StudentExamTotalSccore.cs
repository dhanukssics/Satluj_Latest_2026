namespace Satluj_Latest.Models
{
    public class StudentExamTotalSccore
    {
        public long SchoolIdModel { get; set; }
        public long ClassIdModel { get; set; }
        public long DivisionIdModel { get; set; }
        public long ExamIdModel { get; set; }

        public bool IsAdmin { get; set; }

        public List<StudentExamScoreItem> list { get; set; } = new();
    }

    public class StudentExamScoreItem
    {
        public long StudentId { get; set; }
        public string StudentName { get; set; }
        public decimal MarkObtained { get; set; }
        public decimal MarkPercentage { get; set; }
    }
}
