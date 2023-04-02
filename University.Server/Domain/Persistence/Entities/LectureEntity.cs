namespace University.Server.Domain.Persistence.Entities
{
    public class LectureEntity
    {
        public int Duration { get; set; }
        public DateTime Date { get; set; }

        public Guid LocationId { get; set; }
    }
}