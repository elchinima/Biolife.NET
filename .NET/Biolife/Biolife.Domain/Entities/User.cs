namespace Biolife.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        [MaxLength(25)]
        public string Name { get; set; } = null!;
        [MaxLength(120)]
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(4);
        public bool IsActive { get; set; } = true;
        public bool EmailConfirmed { get; set; } = true;
        public string UniqueKey { get; set; } = null!;
        public DateTime? LastLogin { get; set; }
        public int? RoleId { get; set; }
        public Role? Role { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<Session> Sessions { get; set; } = new List<Session>();
        public ICollection<Note> Notes { get; set; } = new List<Note>();
        public ICollection<EmailConfirmationToken> EmailConfirmationTokens { get; set; } = new List<EmailConfirmationToken>();
        public ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = new List<PasswordResetToken>();
    }
}
