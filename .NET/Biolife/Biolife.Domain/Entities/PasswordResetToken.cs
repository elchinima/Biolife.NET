namespace Biolife.Domain.Entities
{
    public class PasswordResetToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [MaxLength(64)]
        public string Token { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }
        public DateTime? UsedAt { get; set; }
        public User User { get; set; } = null!;
    }
}
