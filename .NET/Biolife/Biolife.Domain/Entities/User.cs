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
        [MaxLength(260)]
        public string? ProfileImagePath { get; set; }
        [MaxLength(120)]
        public string? FullName { get; set; }
        [MaxLength(30)]
        public string? PhoneNumber { get; set; }
        [MaxLength(180)]
        public string? AddressLine1 { get; set; }
        [MaxLength(180)]
        public string? AddressLine2 { get; set; }
        [MaxLength(80)]
        public string? Country { get; set; }
        [MaxLength(80)]
        public string? City { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public int? RoleId { get; set; }
        public Role? Role { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<Session> Sessions { get; set; } = new List<Session>();
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Note> Notes { get; set; } = new List<Note>();
        public ICollection<EmailConfirmationToken> EmailConfirmationTokens { get; set; } = new List<EmailConfirmationToken>();
        public ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = new List<PasswordResetToken>();
        public ICollection<TwoFactorToken> TwoFactorTokens { get; set; } = new List<TwoFactorToken>();
    }
}
