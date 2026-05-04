namespace Biolife.Domain.Entities
{
    public class Session
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [MaxLength(64)]
        public string SessionKey { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime ExpiresAt { get; set; }

        public User User { get; set; } = null!;
    }
}
