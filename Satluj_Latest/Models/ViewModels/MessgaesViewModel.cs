namespace Satluj_Latest.Models.ViewModels
{
    public class MessagesViewModel
    {
        public long SchoolId { get; set; }
        public List<TbClass> Classes { get; set; }
        public Dictionary<long, List<TbDivision>> Divisions { get; set; }
        public Dictionary<(long classId, long divisionId), List<TbStudent>> Students { get; set; }
    }

}
