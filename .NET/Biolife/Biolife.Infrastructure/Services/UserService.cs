namespace Biolife.Infrastructure.Services
{
    public class UserService
    {
        public const string RememberMeCookieName = "pustok_session_key";
        public const string RememberSessionClaimType = "RememberSessionKey";
        public const string ExternalCookieScheme = "BiolifeExternal";
        public const int RememberMeLifetimeDays = 15;
        public const int MaxUserNameLength = 25;
        public const string MaxUserNameLengthError = "Max 25 characters.";
        private static DateTime BakuNow => DateTime.UtcNow.AddHours(4);

        private readonly AppDbContext _db;

        public UserService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<(bool success, string error, User? user)> RegisterAsync(string name, string email, string password)
        {
            name = name.Trim();
            email = email.Trim();

            if (name.Length > MaxUserNameLength)
                return (false, MaxUserNameLengthError, null);

            bool emailExists = await _db.Users.AnyAsync(u => u.Email == email);
            if (emailExists)
                return (false, "This email is already registered.", null);

            var hash = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                Name = name,
                Email = email,
                PasswordHash = hash,
                CreatedAt = BakuNow,
                EmailConfirmed = false,
                UniqueKey = hash + "pustok"
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return (true, string.Empty, user);
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
            if (user is null) return null;

            bool valid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            return valid ? user : null;
        }

        public async Task<(bool success, string error, User? user)> FindOrCreateGoogleUserAsync(string name, string email)
        {
            name = NormalizeGoogleName(name, email);
            email = email.Trim();

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user is not null)
            {
                if (!user.IsActive)
                    return (false, "This account is inactive.", null);

                if (!user.EmailConfirmed)
                    user.EmailConfirmed = true;

                await _db.SaveChangesAsync();
                return (true, string.Empty, user);
            }

            var randomPassword = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
            var hash = BCrypt.Net.BCrypt.HashPassword(randomPassword);

            user = new User
            {
                Name = name,
                Email = email,
                PasswordHash = hash,
                CreatedAt = BakuNow,
                EmailConfirmed = true,
                UniqueKey = $"google:{email}"
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return (true, string.Empty, user);
        }

        public async Task<string> CreateEmailConfirmationTokenAsync(int userId)
        {
            var now = DateTime.UtcNow;
            var activeTokens = await _db.EmailConfirmationTokens
                .Where(t => t.UserId == userId && t.UsedAt == null)
                .ToListAsync();

            foreach (var activeToken in activeTokens)
                activeToken.UsedAt = now;

            var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
            _db.EmailConfirmationTokens.Add(new EmailConfirmationToken
            {
                UserId = userId,
                Token = token,
                CreatedAt = now,
                ExpiresAt = now.AddHours(24)
            });

            await _db.SaveChangesAsync();
            return token;
        }

        public async Task<bool> ConfirmEmailAsync(int userId, string token)
        {
            var now = DateTime.UtcNow;
            var confirmationToken = await _db.EmailConfirmationTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t =>
                    t.UserId == userId &&
                    t.Token == token &&
                    t.UsedAt == null &&
                    t.ExpiresAt > now);

            if (confirmationToken is null)
                return false;

            confirmationToken.UsedAt = now;
            confirmationToken.User.EmailConfirmed = true;
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<User?> FindPasswordResetUserByEmailAsync(string email)
        {
            email = email.Trim();

            return await _db.Users.FirstOrDefaultAsync(u =>
                u.Email == email &&
                u.IsActive &&
                u.EmailConfirmed);
        }

        public async Task<string> CreatePasswordResetTokenAsync(int userId)
        {
            var now = DateTime.UtcNow;
            var activeTokens = await _db.PasswordResetTokens
                .Where(t => t.UserId == userId && t.UsedAt == null)
                .ToListAsync();

            foreach (var activeToken in activeTokens)
                activeToken.UsedAt = now;

            var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
            _db.PasswordResetTokens.Add(new PasswordResetToken
            {
                UserId = userId,
                Token = token,
                CreatedAt = now,
                ExpiresAt = now.AddHours(1)
            });

            await _db.SaveChangesAsync();
            return token;
        }

        public async Task<bool> IsPasswordResetTokenValidAsync(int userId, string token)
        {
            var now = DateTime.UtcNow;

            return await _db.PasswordResetTokens.AnyAsync(t =>
                t.UserId == userId &&
                t.Token == token &&
                t.UsedAt == null &&
                t.ExpiresAt > now &&
                t.User.IsActive &&
                t.User.EmailConfirmed);
        }

        public async Task<bool> ResetPasswordAsync(int userId, string token, string password)
        {
            var now = DateTime.UtcNow;
            var resetToken = await _db.PasswordResetTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t =>
                    t.UserId == userId &&
                    t.Token == token &&
                    t.UsedAt == null &&
                    t.ExpiresAt > now &&
                    t.User.IsActive &&
                    t.User.EmailConfirmed);

            if (resetToken is null)
                return false;

            resetToken.UsedAt = now;
            resetToken.User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            resetToken.User.UniqueKey = resetToken.User.PasswordHash + "pustok";

            var userSessions = await _db.Sessions
                .Where(s => s.UserId == userId)
                .ToListAsync();

            if (userSessions.Count > 0)
                _db.Sessions.RemoveRange(userSessions);

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task UpdateLastLoginAsync(int userId)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user is null) return;
            user.LastLogin = BakuNow;
            await _db.SaveChangesAsync();
        }

        public async Task<Session> CreateRememberSessionAsync(int userId)
        {
            await DeleteExpiredSessionsAsync();

            var utcNow = DateTime.UtcNow;
            var session = new Session
            {
                UserId = userId,
                SessionKey = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)),
                CreatedAt = utcNow,
                ExpiresAt = utcNow.AddDays(RememberMeLifetimeDays)
            };

            _db.Sessions.Add(session);
            await _db.SaveChangesAsync();
            return session;
        }

        public async Task<User?> LoginBySessionKeyAsync(string sessionKey)
        {
            if (string.IsNullOrWhiteSpace(sessionKey))
                return null;

            var session = await _db.Sessions
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.SessionKey == sessionKey);

            if (session is null)
                return null;

            if (session.ExpiresAt <= DateTime.UtcNow || !session.User.IsActive || !session.User.EmailConfirmed)
            {
                _db.Sessions.Remove(session);
                await _db.SaveChangesAsync();
                return null;
            }

            return session.User;
        }

        public async Task RemoveSessionByKeyAsync(string? sessionKey)
        {
            if (string.IsNullOrWhiteSpace(sessionKey))
                return;

            var session = await _db.Sessions.FirstOrDefaultAsync(s => s.SessionKey == sessionKey);
            if (session is null)
                return;

            _db.Sessions.Remove(session);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteUserByEmailAsync(string email)
        {
            email = email.Trim();

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user is null)
                return;

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteExpiredSessionsAsync()
        {
            var utcNow = DateTime.UtcNow;
            var expiredSessions = await _db.Sessions
                .Where(s => s.ExpiresAt <= utcNow)
                .ToListAsync();

            if (expiredSessions.Count == 0)
                return;

            _db.Sessions.RemoveRange(expiredSessions);
            await _db.SaveChangesAsync();
        }

        private static string NormalizeGoogleName(string name, string email)
        {
            name = name.Trim();
            if (string.IsNullOrWhiteSpace(name))
                name = email.Split('@')[0];

            return name.Length <= MaxUserNameLength
                ? name
                : name[..MaxUserNameLength];
        }
    }
}
