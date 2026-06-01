namespace Satluj_Latest.Models
{
    public class CircularList
    {

        public long schoolId { get; set; }
        public bool IsAdmin { get; set; }
        public List<TbCircular> CircularData { get; set; }
            = new List<TbCircular>();
    }
}
