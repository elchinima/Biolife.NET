namespace Biolife.Persistence.Contexts
{
    public class AppDbContext : DbContext
    {
        public DbSet<Carousel> Carousels { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<EmailConfirmationToken> EmailConfirmationTokens { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
        public DbSet<TwoFactorToken> TwoFactorTokens { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Product>()
                .Property(p => p.CostPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Product>()
                .Property(p => p.DiscountPercent)
                .HasColumnType("decimal(5,2)");

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Author)
                .WithMany(a => a.Products)
                .HasForeignKey(p => p.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Genre)
                .WithMany(g => g.Products)
                .HasForeignKey(p => p.GenreId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.AppUsers)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Session>()
                .Property(s => s.SessionKey)
                .HasMaxLength(64);

            modelBuilder.Entity<Session>()
                .HasIndex(s => s.SessionKey)
                .IsUnique();

            modelBuilder.Entity<Session>()
                .HasIndex(s => s.ExpiresAt);

            modelBuilder.Entity<Session>()
                .HasOne(s => s.User)
                .WithMany(u => u.Sessions)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CartItem>()
                .Property(c => c.UnitPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<CartItem>()
                .HasIndex(c => new { c.UserId, c.ProductId });

            modelBuilder.Entity<CartItem>()
                .HasOne(c => c.User)
                .WithMany(u => u.CartItems)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CartItem>()
                .HasOne(c => c.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(c => c.ProductId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Note>()
                .Property(n => n.Type)
                .HasConversion<int>();

            modelBuilder.Entity<Note>()
                .HasOne(n => n.CreatedByUser)
                .WithMany(u => u.Notes)
                .HasForeignKey(n => n.CreatedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<EmailConfirmationToken>()
                .Property(t => t.Token)
                .HasMaxLength(64);

            modelBuilder.Entity<EmailConfirmationToken>()
                .HasIndex(t => t.Token)
                .IsUnique();

            modelBuilder.Entity<EmailConfirmationToken>()
                .HasIndex(t => new { t.UserId, t.ExpiresAt });

            modelBuilder.Entity<EmailConfirmationToken>()
                .HasOne(t => t.User)
                .WithMany(u => u.EmailConfirmationTokens)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PasswordResetToken>()
                .Property(t => t.Token)
                .HasMaxLength(64);

            modelBuilder.Entity<PasswordResetToken>()
                .HasIndex(t => t.Token)
                .IsUnique();

            modelBuilder.Entity<PasswordResetToken>()
                .HasIndex(t => new { t.UserId, t.ExpiresAt });

            modelBuilder.Entity<PasswordResetToken>()
                .HasOne(t => t.User)
                .WithMany(u => u.PasswordResetTokens)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TwoFactorToken>()
                .Property(t => t.CodeHash)
                .HasMaxLength(64);

            modelBuilder.Entity<TwoFactorToken>()
                .Property(t => t.Purpose)
                .HasMaxLength(32);

            modelBuilder.Entity<TwoFactorToken>()
                .HasIndex(t => new { t.UserId, t.Purpose, t.ExpiresAt });

            modelBuilder.Entity<TwoFactorToken>()
                .HasOne(t => t.User)
                .WithMany(u => u.TwoFactorTokens)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
