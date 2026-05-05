namespace Biolife.Domain.Entities
{
    public class TwoFactorToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        [MaxLength(64)]
        public string CodeHash { get; set; } = null!;
        [MaxLength(32)]
        public string Purpose { get; set; } = null!;
        public bool? PendingTwoFactorEnabled { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime? UsedAt { get; set; }
    }
}
