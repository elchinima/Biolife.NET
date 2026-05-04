namespace Biolife.Domain.Entities
{
    public enum NoteType
    {
        Normal = 1,
        High = 2,
        Immediate = 3
    }

    public class Note
    {
        public int Id { get; set; }

        [MaxLength(120)]
        public string Title { get; set; } = null!;

        [MaxLength(1000)]
        public string Text { get; set; } = null!;

        public NoteType Type { get; set; } = NoteType.Normal;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? CreatedByUserId { get; set; }
        public User? CreatedByUser { get; set; }
    }
}
