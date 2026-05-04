namespace Biolife.Domain.Entities
{
    public class Role
    {
        public int Id { get; set; }

        [MaxLength(60)]
        public string Name { get; set; } = null!;

        [MaxLength(7)]
        public string Color { get; set; } = "#888888";
        public int SortOrder { get; set; }

        public bool AdminPanel { get; set; }
        public bool Products { get; set; }
        public bool Slider { get; set; }
        public bool Author { get; set; }
        public bool Genres { get; set; }
        public bool Users { get; set; }
        public bool Roles { get; set; }
        public bool CreateNotes { get; set; }

        public ICollection<User> AppUsers { get; set; } = new List<User>();
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
