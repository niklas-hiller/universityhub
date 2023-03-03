namespace University.Server.Resources
{
    public class SaveSemesterResource
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<int> Modules { get; set; } = new List<int>();
    }
}
