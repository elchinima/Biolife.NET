namespace Biolife.Web.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        private const long StaticImageMaxBytes = 2 * 1024 * 1024;
        private const long GifImageMaxBytes = 10 * 1024 * 1024;
        private static readonly string[] AllowedImageExtensions = [".jpg", ".jpeg", ".png", ".gif"];
        private static readonly WebpEncoder UploadWebpEncoder = new()
        {
            Quality = 75,
            Method = WebpEncodingMethod.Default
        };

        public AdminController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        private int? GetSessionUserId()
        {
            return HttpContext.Session.GetInt32("UserId");
        }

        private static Role BuildEffectiveRole(List<Role> assignedRoles)
        {
            var highest = assignedRoles
                .OrderBy(r => r.SortOrder)
                .ThenBy(r => r.Id)
                .First();

            return new Role
            {
                Id = highest.Id,
                Name = highest.Name,
                Color = highest.Color,
                SortOrder = highest.SortOrder,
                AdminPanel = assignedRoles.Any(r => r.AdminPanel),
                Products = assignedRoles.Any(r => r.Products),
                Slider = assignedRoles.Any(r => r.Slider),
                Author = assignedRoles.Any(r => r.Author),
                Genres = assignedRoles.Any(r => r.Genres),
                Users = assignedRoles.Any(r => r.Users),
                Roles = assignedRoles.Any(r => r.Roles),
                CreateNotes = assignedRoles.Any(r => r.CreateNotes)
            };
        }

        private List<Role> GetCurrentUserRoles()
        {
            var userId = GetSessionUserId();
            if (userId == null) return new List<Role>();

            var user = _db.Users
                .Include(u => u.Role)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefault(u => u.Id == userId.Value);

            if (user == null) return new List<Role>();

            var roles = user.UserRoles
                .Where(ur => ur.Role != null)
                .Select(ur => ur.Role)
                .DistinctBy(r => r.Id)
                .ToList();

            // Backward compatibility: old records may still have only RoleId.
            if (roles.Count == 0 && user.Role != null)
                roles.Add(user.Role);

            return roles
                .OrderBy(r => r.SortOrder)
                .ThenBy(r => r.Id)
                .ToList();
        }

        private Role? GetCurrentUserRole()
        {
            var roles = GetCurrentUserRoles();
            if (roles.Count == 0) return null;
            return BuildEffectiveRole(roles);
        }

        private Role? GetHighestRoleForUser(int userId)
        {
            return _db.UserRoles
                .Where(ur => ur.UserId == userId)
                .Include(ur => ur.Role)
                .Select(ur => ur.Role)
                .OrderBy(r => r.SortOrder)
                .ThenBy(r => r.Id)
                .FirstOrDefault();
        }

        private void SyncPrimaryRole(int userId)
        {
            var user = _db.Users.Find(userId);
            if (user == null) return;

            var highestRole = GetHighestRoleForUser(userId);
            user.RoleId = highestRole?.Id;
        }

        private void SyncPrimaryRoleForAllUsers()
        {
            var userIds = _db.Users.Select(u => u.Id).ToList();
            foreach (var userId in userIds)
                SyncPrimaryRole(userId);
        }

        private void NormalizeRoleOrder()
        {
            var roles = _db.Roles
                .OrderBy(r => r.SortOrder)
                .ThenBy(r => r.Id)
                .ToList();

            var changed = false;
            for (var i = 0; i < roles.Count; i++)
            {
                var expected = i + 1;
                if (roles[i].SortOrder == expected) continue;
                roles[i].SortOrder = expected;
                changed = true;
            }

            if (changed)
                _db.SaveChanges();
        }

        private bool CanManageUser(int actorUserId, int targetUserId)
        {
            if (actorUserId == targetUserId) return true;

            var actorHighestRole = GetHighestRoleForUser(actorUserId);
            if (actorHighestRole == null) return false;

            var targetHighestRole = GetHighestRoleForUser(targetUserId);
            if (targetHighestRole == null) return true;

            // Smaller SortOrder means higher privilege.
            return actorHighestRole.SortOrder < targetHighestRole.SortOrder;
        }

        private static bool CanManageRole(Role? actorHighestRole, Role targetRole)
        {
            if (actorHighestRole == null) return false;

            // Smaller SortOrder means higher privilege.
            return actorHighestRole.SortOrder < targetRole.SortOrder;
        }

        private static bool CanSwapRoleOrder(Role? actorHighestRole, Role movingRole, Role swappedRole)
        {
            return CanManageRole(actorHighestRole, movingRole)
                && CanManageRole(actorHighestRole, swappedRole);
        }

        private static void EnsureAdminPanelForSectionPermissions(
            ref bool adminPanel,
            bool products,
            bool slider,
            bool author,
            bool genres,
            bool users,
            bool roles,
            bool createNotes)
        {
            var hasSectionPermission = products || slider || author || genres || users || roles || createNotes;
            if (hasSectionPermission)
                adminPanel = true;
        }

        private static void ClampNewRolePermissionsToActor(
            Role actorRole,
            ref bool adminPanel,
            ref bool products,
            ref bool slider,
            ref bool author,
            ref bool genres,
            ref bool users,
            ref bool roles,
            ref bool createNotes)
        {
            adminPanel = adminPanel && actorRole.AdminPanel;
            products = products && actorRole.Products;
            slider = slider && actorRole.Slider;
            author = author && actorRole.Author;
            genres = genres && actorRole.Genres;
            users = users && actorRole.Users;
            roles = roles && actorRole.Roles;
            createNotes = createNotes && actorRole.CreateNotes;

            EnsureAdminPanelForSectionPermissions(ref adminPanel, products, slider, author, genres, users, roles, createNotes);
        }

        private static void ApplyEditableRolePermissionsFromActor(
            Role actorRole,
            Role targetRole,
            bool adminPanel,
            bool products,
            bool slider,
            bool author,
            bool genres,
            bool users,
            bool roles,
            bool createNotes)
        {
            var nextAdminPanel = actorRole.AdminPanel ? adminPanel : targetRole.AdminPanel;
            var nextProducts = actorRole.Products ? products : targetRole.Products;
            var nextSlider = actorRole.Slider ? slider : targetRole.Slider;
            var nextAuthor = actorRole.Author ? author : targetRole.Author;
            var nextGenres = actorRole.Genres ? genres : targetRole.Genres;
            var nextUsers = actorRole.Users ? users : targetRole.Users;
            var nextRoles = actorRole.Roles ? roles : targetRole.Roles;
            var nextCreateNotes = actorRole.CreateNotes ? createNotes : targetRole.CreateNotes;

            EnsureAdminPanelForSectionPermissions(
                ref nextAdminPanel,
                nextProducts,
                nextSlider,
                nextAuthor,
                nextGenres,
                nextUsers,
                nextRoles,
                nextCreateNotes);

            targetRole.AdminPanel = nextAdminPanel;
            targetRole.Products = nextProducts;
            targetRole.Slider = nextSlider;
            targetRole.Author = nextAuthor;
            targetRole.Genres = nextGenres;
            targetRole.Users = nextUsers;
            targetRole.Roles = nextRoles;
            targetRole.CreateNotes = nextCreateNotes;
        }

        private IActionResult BuildAccessDeniedResult(string sectionName)
        {
            return View("AccessDenied", new AccessDeniedVm
            {
                SectionName = sectionName
            });
        }

        private IActionResult? RequireAdminPanel(Role? role)
        {
            if (role == null || !role.AdminPanel)
                return BuildAccessDeniedResult("Admin Panel");
            return null;
        }

        private IActionResult? RequirePermission(Role? role, Func<Role, bool> checker, string sectionName)
        {
            var adminGuard = RequireAdminPanel(role);
            if (adminGuard != null) return adminGuard;

            if (!checker(role!))
                return BuildAccessDeniedResult(sectionName);

            return null;
        }

        private AdminVm BuildProductsVm()
        {
            return new AdminVm
            {
                Products = _db.Products.Include(p => p.Author).Include(p => p.Genre)
                             .OrderByDescending(p => p.CreatedAt).ToList(),
                Authors = _db.Authors.ToList(),
                Genres = _db.Genres.ToList()
            };
        }

        private static bool IsAllowedImageExtension(string extension)
        {
            return AllowedImageExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
        }

        private static bool IsAllowedImageFormat(IImageFormat format)
        {
            return format.Name.Equals("JPEG", StringComparison.OrdinalIgnoreCase)
                || format.Name.Equals("PNG", StringComparison.OrdinalIgnoreCase)
                || format.Name.Equals("GIF", StringComparison.OrdinalIgnoreCase);
        }

        private static bool HasUploadedFile(IFormFile? file)
        {
            return file != null && file.Length > 0;
        }

        private static string? ValidateImageUpload(IFormFile? file)
        {
            if (file == null || file.Length == 0)
                return null;

            var extension = Path.GetExtension(file.FileName);
            if (!IsAllowedImageExtension(extension))
                return "Only JPG, PNG and GIF images are allowed.";

            var maxBytes = extension.Equals(".gif", StringComparison.OrdinalIgnoreCase)
                ? GifImageMaxBytes
                : StaticImageMaxBytes;

            if (file.Length > maxBytes)
            {
                var maxMb = maxBytes / 1024 / 1024;
                return $"Maximum file size for {extension.TrimStart('.').ToUpperInvariant()} is {maxMb} MB.";
            }

            return null;
        }

        private void NormalizeProductModelStateForOptionalFields()
        {
            ModelState.Remove(nameof(Product.Author));
            ModelState.Remove(nameof(Product.Genre));
            ModelState.Remove(nameof(Product.ImageUrl));
            ModelState.Remove(nameof(Product.CostPrice));
            ModelState.Remove(nameof(Product.DiscountPercent));
        }

        private string SaveImage(IFormFile file)
        {
            var validationError = ValidateImageUpload(file);
            if (validationError != null)
                throw new InvalidOperationException(validationError);

            var uploads = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploads);
            var fileName = Guid.NewGuid() + ".webp";
            var path = Path.Combine(uploads, fileName);

            try
            {
                using var inputStream = file.OpenReadStream();
                var format = Image.DetectFormat(inputStream);
                if (!IsAllowedImageFormat(format))
                    throw new InvalidOperationException("Only JPG, PNG and GIF images are allowed.");

                inputStream.Position = 0;
                using var image = Image.Load(inputStream);
                image.SaveAsWebp(path, UploadWebpEncoder);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex) when (ex is UnknownImageFormatException
                || ex is InvalidImageContentException
                || ex is ImageFormatException)
            {
                throw new InvalidOperationException("The uploaded file is not a valid JPG, PNG or GIF image.", ex);
            }

            return "/uploads/" + fileName;
        }

        private void DeleteImage(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl)) return;
            if (!imageUrl.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase)) return;
            var fileName = Path.GetFileName(imageUrl);
            if (string.IsNullOrWhiteSpace(fileName)) return;
            var path = Path.Combine(_env.WebRootPath, "uploads", fileName);
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
        }

        private static string NormalizeHexColor(string? color)
        {
            if (string.IsNullOrWhiteSpace(color))
                return "#888888";

            var value = color.Trim();
            if (!value.StartsWith("#"))
                value = "#" + value;

            if (value.Length == 4)
                value = $"#{value[1]}{value[1]}{value[2]}{value[2]}{value[3]}{value[3]}";

            if (value.Length != 7)
                return "#888888";

            return Regex.IsMatch(value, "^#[0-9A-Fa-f]{6}$")
                ? value.ToUpperInvariant()
                : "#888888";
        }

        public IActionResult Index()
        {
            var role = GetCurrentUserRole();
            var guard = RequireAdminPanel(role);
            if (guard != null) return guard;

            var vm = new AdminVm
            {
                TotalProducts = _db.Products.Count(),
                TotalAuthors = _db.Authors.Count(),
                TotalGenres = _db.Genres.Count(),
                TotalCarousels = _db.Carousels.Count(),
                TotalRevenue = _db.Products.Any() ? _db.Products.Sum(p => p.Price) : 0,
                Users = _db.Users
                    .Include(u => u.Role)
                    .Include(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                    .OrderBy(u => u.Id)
                    .ToList(),
                CurrentUserRole = role
            };
            return View(vm);
        }

        public IActionResult Products()
        {
            var role = GetCurrentUserRole();
            var guard = RequirePermission(role, r => r.Products, "Products");
            if (guard != null) return guard;

            var vm = BuildProductsVm();
            vm.CurrentUserRole = role;
            return View(vm);
        }

        public IActionResult Carousel()
        {
            var role = GetCurrentUserRole();
            var guard = RequirePermission(role, r => r.Slider, "Slider");
            if (guard != null) return guard;

            var vm = new AdminVm
            {
                Carousels = _db.Carousels.OrderBy(c => c.Order).ToList(),
                CurrentUserRole = role
            };
            return View(vm);
        }

        public IActionResult Authors()
        {
            var role = GetCurrentUserRole();
            var guard = RequirePermission(role, r => r.Author, "Authors");
            if (guard != null) return guard;

            var vm = new AdminVm
            {
                Authors = _db.Authors.Include(a => a.Products).ToList(),
                CurrentUserRole = role
            };
            return View(vm);
        }

        public IActionResult Genres()
        {
            var role = GetCurrentUserRole();
            var guard = RequirePermission(role, r => r.Genres, "Genres");
            if (guard != null) return guard;

            var vm = new AdminVm
            {
                Genres = _db.Genres.Include(g => g.Products).ToList(),
                CurrentUserRole = role
            };
            return View(vm);
        }

        public IActionResult Users()
        {
            var role = GetCurrentUserRole();
            var guard = RequirePermission(role, r => r.Users, "Users");
            if (guard != null) return guard;

            var vm = new AdminVm
            {
                Users = _db.Users
                    .Include(u => u.Role)
                    .Include(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                    .OrderBy(u => u.Id)
                    .ToList(),
                TotalUsers = _db.Users.Count(),
                CurrentUserRole = role
            };
            return View(vm);
        }

        public IActionResult Roles()
        {
            var role = GetCurrentUserRole();
            var guard = RequirePermission(role, r => r.Roles, "Roles");
            if (guard != null) return guard;

            NormalizeRoleOrder();

            var vm = new AdminVm
            {
                Roles = _db.Roles
                    .OrderBy(r => r.SortOrder)
                    .ThenBy(r => r.Id)
                    .ToList(),
                Users = _db.Users
                    .Include(u => u.Role)
                    .Include(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                    .OrderBy(u => u.Name)
                    .ToList(),
                CurrentUserRole = role
            };
            return View(vm);
        }

        public IActionResult Notes()
        {
            var role = GetCurrentUserRole();
            var guard = RequireAdminPanel(role);
            if (guard != null) return guard;

            var vm = new AdminVm
            {
                Notes = _db.Notes
                    .Include(n => n.CreatedByUser)
                    .OrderByDescending(n => n.Type)
                    .ThenByDescending(n => n.CreatedAt)
                    .ToList(),
                CurrentUserRole = role
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult CreateRole(
            string name,
            string color,
            bool adminPanel,
            bool products,
            bool slider,
            bool author,
            bool genres,
            bool users,
            bool roles,
            bool createNotes)
        {
            var currentRole = GetCurrentUserRole();
            var guard = RequirePermission(currentRole, r => r.Roles, "Roles");
            if (guard != null) return guard;

            var normalizedName = (name ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(normalizedName) || normalizedName.Length > 60)
                return RedirectToAction("Roles");

            var exists = _db.Roles.Any(r => r.Name.ToLower() == normalizedName.ToLower());
            if (exists)
                return RedirectToAction("Roles");

            ClampNewRolePermissionsToActor(
                currentRole!,
                ref adminPanel,
                ref products,
                ref slider,
                ref author,
                ref genres,
                ref users,
                ref roles,
                ref createNotes);

            var nextOrder = (_db.Roles.Max(r => (int?)r.SortOrder) ?? 0) + 1;
            var newRole = new Role
            {
                Name = normalizedName,
                Color = NormalizeHexColor(color),
                SortOrder = nextOrder,
                AdminPanel = adminPanel,
                Products = products,
                Slider = slider,
                Author = author,
                Genres = genres,
                Users = users,
                Roles = roles,
                CreateNotes = createNotes
            };

            _db.Roles.Add(newRole);
            _db.SaveChanges();

            return RedirectToAction("Roles");
        }

        [HttpPost]
        public IActionResult EditRole(
            int id,
            string name,
            string color,
            bool adminPanel,
            bool products,
            bool slider,
            bool author,
            bool genres,
            bool users,
            bool roles,
            bool createNotes)
        {
            var currentRole = GetCurrentUserRole();
            var guard = RequirePermission(currentRole, r => r.Roles, "Roles");
            if (guard != null) return guard;

            NormalizeRoleOrder();

            var role = _db.Roles.FirstOrDefault(r => r.Id == id);
            if (role == null)
                return RedirectToAction("Roles");

            if (!CanManageRole(currentRole, role))
                return RedirectToAction("Roles");

            var normalizedName = (name ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(normalizedName) || normalizedName.Length > 60)
                return RedirectToAction("Roles");

            var exists = _db.Roles.Any(r => r.Id != id && r.Name.ToLower() == normalizedName.ToLower());
            if (exists)
                return RedirectToAction("Roles");

            role.Name = normalizedName;
            role.Color = NormalizeHexColor(color);
            ApplyEditableRolePermissionsFromActor(
                currentRole!,
                role,
                adminPanel,
                products,
                slider,
                author,
                genres,
                users,
                roles,
                createNotes);

            _db.SaveChanges();

            return RedirectToAction("Roles");
        }

        [HttpPost]
        public IActionResult CreateNote(string title, string text, NoteType type)
        {
            var role = GetCurrentUserRole();
            var guard = RequireAdminPanel(role);
            if (guard != null) return guard;

            if (role?.CreateNotes != true)
                return BuildAccessDeniedResult("Create Notes");

            var normalizedTitle = (title ?? string.Empty).Trim();
            var normalizedText = (text ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(normalizedTitle) ||
                normalizedTitle.Length > 120 ||
                string.IsNullOrWhiteSpace(normalizedText) ||
                normalizedText.Length > 1000 ||
                !Enum.IsDefined(typeof(NoteType), type))
            {
                return RedirectToAction("Notes");
            }

            _db.Notes.Add(new Note
            {
                Title = normalizedTitle,
                Text = normalizedText,
                Type = type,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = GetSessionUserId()
            });

            _db.SaveChanges();
            return RedirectToAction("Notes");
        }

        [HttpPost]
        public IActionResult DeleteNote(int id)
        {
            var role = GetCurrentUserRole();
            var guard = RequireAdminPanel(role);
            if (guard != null) return guard;

            var note = _db.Notes.Find(id);
            if (note != null)
            {
                _db.Notes.Remove(note);
                _db.SaveChanges();
            }

            return RedirectToAction("Notes");
        }

        [HttpPost]
        public IActionResult DeleteRole(int id)
        {
            var currentRole = GetCurrentUserRole();
            var guard = RequirePermission(currentRole, r => r.Roles, "Roles");
            if (guard != null) return guard;

            NormalizeRoleOrder();

            var role = _db.Roles.FirstOrDefault(r => r.Id == id);
            if (role == null)
                return RedirectToAction("Roles");

            if (!CanManageRole(currentRole, role))
                return RedirectToAction("Roles");

            var linkedAssignments = _db.UserRoles
                .Where(ur => ur.RoleId == id)
                .ToList();

            var affectedUserIds = linkedAssignments
                .Select(ur => ur.UserId)
                .Union(_db.Users.Where(u => u.RoleId == id).Select(u => u.Id))
                .Distinct()
                .ToList();

            if (linkedAssignments.Count > 0)
                _db.UserRoles.RemoveRange(linkedAssignments);

            _db.Roles.Remove(role);
            _db.SaveChanges();

            foreach (var userId in affectedUserIds)
                SyncPrimaryRole(userId);

            _db.SaveChanges();

            return RedirectToAction("Roles");
        }

        [HttpPost]
        public IActionResult AssignUserRole(int roleId, int userId)
        {
            var currentRole = GetCurrentUserRole();
            var guard = RequirePermission(currentRole, r => r.Roles, "Roles");
            if (guard != null) return guard;

            var actorUserId = GetSessionUserId();
            if (!actorUserId.HasValue) return Unauthorized();
            if (!CanManageUser(actorUserId.Value, userId))
                return RedirectToAction("Roles");

            var role = _db.Roles.Find(roleId);
            var user = _db.Users.Find(userId);
            if (role == null || user == null)
                return RedirectToAction("Roles");

            if (!CanManageRole(currentRole, role))
                return RedirectToAction("Roles");

            var alreadyAssigned = _db.UserRoles.Any(ur => ur.UserId == userId && ur.RoleId == roleId);
            if (!alreadyAssigned)
            {
                _db.UserRoles.Add(new UserRole
                {
                    UserId = userId,
                    RoleId = roleId,
                    AssignedAt = DateTime.UtcNow
                });
            }

            SyncPrimaryRole(userId);
            _db.SaveChanges();

            return RedirectToAction("Roles");
        }

        [HttpPost]
        public IActionResult RemoveUserRole(int roleId, int userId)
        {
            var currentRole = GetCurrentUserRole();
            var guard = RequirePermission(currentRole, r => r.Roles, "Roles");
            if (guard != null) return guard;

            var actorUserId = GetSessionUserId();
            if (!actorUserId.HasValue) return Unauthorized();
            if (!CanManageUser(actorUserId.Value, userId))
                return RedirectToAction("Roles");

            var user = _db.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return RedirectToAction("Roles");

            var assignment = user.UserRoles.FirstOrDefault(ur => ur.RoleId == roleId);
            if (assignment == null)
                return RedirectToAction("Roles");

            if (assignment.Role == null || !CanManageRole(currentRole, assignment.Role))
                return RedirectToAction("Roles");

            var currentSessionUserId = GetSessionUserId();
            if (currentSessionUserId.HasValue && currentSessionUserId.Value == userId)
            {
                var highestRoleId = user.UserRoles
                    .OrderBy(ur => ur.Role.SortOrder)
                    .ThenBy(ur => ur.Role.Id)
                    .Select(ur => (int?)ur.RoleId)
                    .FirstOrDefault();

                // A user cannot remove their own highest role.
                if (highestRoleId.HasValue && highestRoleId.Value == roleId)
                    return RedirectToAction("Roles");
            }

            _db.UserRoles.Remove(assignment);
            SyncPrimaryRole(userId);
            _db.SaveChanges();

            return RedirectToAction("Roles");
        }

        [HttpPost]
        public IActionResult UpdateUserRoles(int userId, int[]? roleIds)
        {
            var currentRole = GetCurrentUserRole();
            var guard = RequirePermission(currentRole, r => r.Roles, "Roles");
            if (guard != null) return guard;

            var actorUserId = GetSessionUserId();
            if (!actorUserId.HasValue) return Unauthorized();
            if (!CanManageUser(actorUserId.Value, userId))
                return RedirectToAction("Roles");

            var user = _db.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return RedirectToAction("Roles");

            var requestedRoleIds = (roleIds ?? Array.Empty<int>())
                .Distinct()
                .ToHashSet();
            var currentAssignedRoleIds = user.UserRoles
                .Select(ur => ur.RoleId)
                .ToHashSet();

            var currentSessionUserId = GetSessionUserId();
            if (currentSessionUserId.HasValue && currentSessionUserId.Value == userId)
            {
                var highestRoleId = user.UserRoles
                    .OrderBy(ur => ur.Role.SortOrder)
                    .ThenBy(ur => ur.Role.Id)
                    .Select(ur => (int?)ur.RoleId)
                    .FirstOrDefault();

                if (highestRoleId.HasValue && !requestedRoleIds.Contains(highestRoleId.Value))
                    requestedRoleIds.Add(highestRoleId.Value);
            }

            var relevantRoleIds = requestedRoleIds
                .Union(currentAssignedRoleIds)
                .ToHashSet();

            var validRoleIds = _db.Roles
                .Where(r => relevantRoleIds.Contains(r.Id))
                .AsEnumerable()
                .Where(r =>
                    (requestedRoleIds.Contains(r.Id) && CanManageRole(currentRole, r)) ||
                    (currentAssignedRoleIds.Contains(r.Id) && !CanManageRole(currentRole, r)))
                .Select(r => r.Id)
                .ToHashSet();

            var toRemove = user.UserRoles
                .Where(ur => !validRoleIds.Contains(ur.RoleId))
                .ToList();

            if (toRemove.Count > 0)
                _db.UserRoles.RemoveRange(toRemove);

            var existingRoleIds = user.UserRoles.Select(ur => ur.RoleId).ToHashSet();
            foreach (var roleId in validRoleIds)
            {
                if (existingRoleIds.Contains(roleId)) continue;
                _db.UserRoles.Add(new UserRole
                {
                    UserId = userId,
                    RoleId = roleId,
                    AssignedAt = DateTime.UtcNow
                });
            }

            SyncPrimaryRole(userId);
            _db.SaveChanges();

            return RedirectToAction("Roles");
        }

        [HttpPost]
        public IActionResult MoveRoleUp(int id)
        {
            var role = GetCurrentUserRole();
            var guard = RequirePermission(role, r => r.Roles, "Roles");
            if (guard != null) return guard;

            NormalizeRoleOrder();

            var roles = _db.Roles
                .OrderBy(r => r.SortOrder)
                .ThenBy(r => r.Id)
                .ToList();

            var index = roles.FindIndex(r => r.Id == id);
            if (index <= 0)
                return RedirectToAction("Roles");

            if (!CanSwapRoleOrder(role, roles[index], roles[index - 1]))
                return RedirectToAction("Roles");

            (roles[index - 1].SortOrder, roles[index].SortOrder) = (roles[index].SortOrder, roles[index - 1].SortOrder);
            _db.SaveChanges();
            SyncPrimaryRoleForAllUsers();
            _db.SaveChanges();

            return RedirectToAction("Roles");
        }

        [HttpPost]
        public IActionResult MoveRoleDown(int id)
        {
            var role = GetCurrentUserRole();
            var guard = RequirePermission(role, r => r.Roles, "Roles");
            if (guard != null) return guard;

            NormalizeRoleOrder();

            var roles = _db.Roles
                .OrderBy(r => r.SortOrder)
                .ThenBy(r => r.Id)
                .ToList();

            var index = roles.FindIndex(r => r.Id == id);
            if (index < 0 || index >= roles.Count - 1)
                return RedirectToAction("Roles");

            if (!CanSwapRoleOrder(role, roles[index], roles[index + 1]))
                return RedirectToAction("Roles");

            (roles[index + 1].SortOrder, roles[index].SortOrder) = (roles[index].SortOrder, roles[index + 1].SortOrder);
            _db.SaveChanges();
            SyncPrimaryRoleForAllUsers();
            _db.SaveChanges();

            return RedirectToAction("Roles");
        }

        [HttpPost]
        public IActionResult ToggleUserBlock(int id)
        {
            var role = GetCurrentUserRole();
            var guard = RequirePermission(role, r => r.Users, "Users");
            if (guard != null) return guard;

            var currentUserId = GetSessionUserId();
            if (currentUserId == null)
                return Unauthorized();

            if (id == currentUserId.Value)
                return RedirectToAction("Users");

            if (!CanManageUser(currentUserId.Value, id))
                return RedirectToAction("Users");

            var user = _db.Users.Find(id);
            if (user == null)
                return NotFound();

            user.IsActive = !user.IsActive;
            _db.SaveChanges();

            return RedirectToAction("Users");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserName(int id, string name)
        {
            var role = GetCurrentUserRole();
            var guard = RequirePermission(role, r => r.Users, "Users");
            if (guard != null) return guard;

            var currentUserId = GetSessionUserId();
            if (!currentUserId.HasValue) return Unauthorized();
            if (!CanManageUser(currentUserId.Value, id))
                return RedirectToAction("Users");

            var user = _db.Users.Find(id);
            if (user == null)
                return NotFound();

            var normalizedName = (name ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(normalizedName) || normalizedName.Length > 60)
                return RedirectToAction("Users");

            user.Name = normalizedName;
            _db.SaveChanges();

            var currentUserIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(currentUserIdRaw, out var refreshedUserId) && refreshedUserId == user.Id)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email)
                };

                var rememberSessionKey = User.FindFirstValue(UserService.RememberSessionClaimType);
                if (!string.IsNullOrWhiteSpace(rememberSessionKey))
                    claims.Add(new Claim(UserService.RememberSessionClaimType, rememberSessionKey));

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity));
            }

            return RedirectToAction("Users");
        }

        [HttpPost]
        public IActionResult CreateProduct(Product model, IFormFile? imageFile, IFormFile? hoverImageFile)
        {
            var role = GetCurrentUserRole();
            var guard = RequirePermission(role, r => r.Products, "Products");
            if (guard != null) return guard;

            NormalizeProductModelStateForOptionalFields();

            if (!HasUploadedFile(imageFile))
                ModelState.AddModelError(nameof(Product.ImageUrl), "Cover image is required.");

            var imageError = ValidateImageUpload(imageFile);
            var hoverImageError = ValidateImageUpload(hoverImageFile);
            if (imageError != null)
                ModelState.AddModelError(nameof(imageFile), imageError);
            if (hoverImageError != null)
                ModelState.AddModelError(nameof(hoverImageFile), hoverImageError);

            if (!ModelState.IsValid)
            {
                var vm = BuildProductsVm();
                vm.CurrentUserRole = role;
                return View("Products", vm);
            }

            try
            {
                model.ImageUrl = SaveImage(imageFile!);
                if (HasUploadedFile(hoverImageFile))
                    model.HoverImageUrl = SaveImage(hoverImageFile!);
            }
            catch (InvalidOperationException ex)
            {
                DeleteImage(model.ImageUrl);
                DeleteImage(model.HoverImageUrl);
                ModelState.AddModelError(string.Empty, ex.Message);
                var vm = BuildProductsVm();
                vm.CurrentUserRole = role;
                return View("Products", vm);
            }

            model.CreatedAt = DateTime.UtcNow;
            _db.Products.Add(model);
            _db.SaveChanges();
            return RedirectToAction("Products");
        }

        [HttpPost]
        public IActionResult DeleteProduct(int id)
        {
            var role = GetCurrentUserRole();
            var guard = RequirePermission(role, r => r.Products, "Products");
            if (guard != null) return guard;

            var product = _db.Products.Find(id);
            if (product != null)
            {
                DeleteImage(product.ImageUrl);
                DeleteImage(product.HoverImageUrl);
                _db.Products.Remove(product);
                _db.SaveChanges();
            }
            return RedirectToAction("Products");
        }

        [HttpPost]
        public IActionResult UpdateProduct(Product model, IFormFile? imageFile, IFormFile? hoverImageFile)
        {
            var role = GetCurrentUserRole();
            var guard = RequirePermission(role, r => r.Products, "Products");
            if (guard != null) return guard;

            var product = _db.Products.Find(model.Id);
            if (product == null)
                return NotFound();

            NormalizeProductModelStateForOptionalFields();

            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.CostPrice = model.CostPrice;
            product.DiscountPercent = model.DiscountPercent;
            product.AuthorId = model.AuthorId;
            product.GenreId = model.GenreId;
            product.IsFeatured = model.IsFeatured;
            product.IsNew = model.IsNew;

            var imageError = ValidateImageUpload(imageFile);
            var hoverImageError = ValidateImageUpload(hoverImageFile);
            if (imageError != null)
                ModelState.AddModelError(nameof(imageFile), imageError);
            if (hoverImageError != null)
                ModelState.AddModelError(nameof(hoverImageFile), hoverImageError);

            if (!ModelState.IsValid)
            {
                var vm = BuildProductsVm();
                vm.CurrentUserRole = role;
                return View("Products", vm);
            }

            string? newImageUrl = null;
            string? newHoverImageUrl = null;
            try
            {
                if (HasUploadedFile(imageFile))
                    newImageUrl = SaveImage(imageFile!);
                if (HasUploadedFile(hoverImageFile))
                    newHoverImageUrl = SaveImage(hoverImageFile!);
            }
            catch (InvalidOperationException ex)
            {
                DeleteImage(newImageUrl);
                DeleteImage(newHoverImageUrl);
                ModelState.AddModelError(string.Empty, ex.Message);
                var vm = BuildProductsVm();
                vm.CurrentUserRole = role;
                return View("Products", vm);
            }

            var oldImageUrl = product.ImageUrl;
            var oldHoverImageUrl = product.HoverImageUrl;
            if (newImageUrl != null)
                product.ImageUrl = newImageUrl;
            if (newHoverImageUrl != null)
                product.HoverImageUrl = newHoverImageUrl;

            _db.SaveChanges();
            if (newImageUrl != null)
                DeleteImage(oldImageUrl);
            if (newHoverImageUrl != null)
                DeleteImage(oldHoverImageUrl);
            return RedirectToAction("Products");
        }

        [HttpPost]
        public IActionResult CreateCarousel(Carousel model, IFormFile? imageFile)
        {
            var role = GetCurrentUserRole();
            var guard = RequirePermission(role, r => r.Slider, "Slider");
            if (guard != null) return guard;

            var imageError = ValidateImageUpload(imageFile);
            if (imageError != null)
            {
                ModelState.AddModelError(nameof(imageFile), imageError);
                return View("Carousel", new AdminVm
                {
                    Carousels = _db.Carousels.OrderBy(c => c.Order).ToList(),
                    CurrentUserRole = role
                });
            }

            try
            {
                if (HasUploadedFile(imageFile))
                    model.ImageUrl = SaveImage(imageFile!);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Carousel", new AdminVm
                {
                    Carousels = _db.Carousels.OrderBy(c => c.Order).ToList(),
                    CurrentUserRole = role
                });
            }

            _db.Carousels.Add(model);
            _db.SaveChanges();
            return RedirectToAction("Carousel");
        }

        [HttpPost]
        public IActionResult DeleteCarousel(int id)
        {
            var role = GetCurrentUserRole();
            var guard = RequirePermission(role, r => r.Slider, "Slider");
            if (guard != null) return guard;

            var slide = _db.Carousels.Find(id);
            if (slide != null)
            {
                DeleteImage(slide.ImageUrl);
                _db.Carousels.Remove(slide);
                _db.SaveChanges();
            }
            return RedirectToAction("Carousel");
        }

        [HttpPost]
        public IActionResult UpdateCarousel(Carousel model, IFormFile? imageFile)
        {
            var role = GetCurrentUserRole();
            var guard = RequirePermission(role, r => r.Slider, "Slider");
            if (guard != null) return guard;

            var slide = _db.Carousels.Find(model.Id);
            if (slide == null) return NotFound();

            slide.Name = model.Name;
            slide.Decription = model.Decription;
            slide.ButtonText = model.ButtonText;
            slide.ButtonLink = model.ButtonLink;
            slide.Order = model.Order;
            slide.IsActive = Request.Form["IsActive"].Contains("true");

            var imageError = ValidateImageUpload(imageFile);
            if (imageError != null)
            {
                ModelState.AddModelError(nameof(imageFile), imageError);
                return View("Carousel", new AdminVm
                {
                    Carousels = _db.Carousels.OrderBy(c => c.Order).ToList(),
                    CurrentUserRole = role
                });
            }

            if (HasUploadedFile(imageFile))
            {
                var oldImageUrl = slide.ImageUrl;
                string? newImageUrl = null;
                try
                {
                    newImageUrl = SaveImage(imageFile!);
                    slide.ImageUrl = newImageUrl;
                }
                catch (InvalidOperationException ex)
                {
                    DeleteImage(newImageUrl);
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return View("Carousel", new AdminVm
                    {
                        Carousels = _db.Carousels.OrderBy(c => c.Order).ToList(),
                        CurrentUserRole = role
                    });
                }

                _db.SaveChanges();
                DeleteImage(oldImageUrl);
                return RedirectToAction("Carousel");
            }
            _db.SaveChanges();
            return RedirectToAction("Carousel");
        }

        [HttpPost]
        public IActionResult CreateAuthor(string name)
        {
            var role = GetCurrentUserRole();
            var guard = RequirePermission(role, r => r.Author, "Authors");
            if (guard != null) return guard;

            _db.Authors.Add(new Author { Name = name });
            _db.SaveChanges();
            return RedirectToAction("Authors");
        }

        [HttpPost]
        public IActionResult UpdateAuthor(int id, string name)
        {
            var role = GetCurrentUserRole();
            var guard = RequirePermission(role, r => r.Author, "Authors");
            if (guard != null) return guard;

            var author = _db.Authors.Find(id);
            if (author != null) { author.Name = name; _db.SaveChanges(); }
            return RedirectToAction("Authors");
        }

        [HttpPost]
        public IActionResult DeleteAuthor(int id)
        {
            var role = GetCurrentUserRole();
            var guard = RequirePermission(role, r => r.Author, "Authors");
            if (guard != null) return guard;

            var author = _db.Authors.Find(id);
            if (author != null) { _db.Authors.Remove(author); _db.SaveChanges(); }
            return RedirectToAction("Authors");
        }

        [HttpPost]
        public IActionResult CreateGenre(string name)
        {
            var role = GetCurrentUserRole();
            var guard = RequirePermission(role, r => r.Genres, "Genres");
            if (guard != null) return guard;

            _db.Genres.Add(new Genre { Name = name });
            _db.SaveChanges();
            return RedirectToAction("Genres");
        }

        [HttpPost]
        public IActionResult UpdateGenre(int id, string name)
        {
            var role = GetCurrentUserRole();
            var guard = RequirePermission(role, r => r.Genres, "Genres");
            if (guard != null) return guard;

            var genre = _db.Genres.Find(id);
            if (genre != null) { genre.Name = name; _db.SaveChanges(); }
            return RedirectToAction("Genres");
        }

        [HttpPost]
        public IActionResult DeleteGenre(int id)
        {
            var role = GetCurrentUserRole();
            var guard = RequirePermission(role, r => r.Genres, "Genres");
            if (guard != null) return guard;

            var genre = _db.Genres.Find(id);
            if (genre != null) { _db.Genres.Remove(genre); _db.SaveChanges(); }
            return RedirectToAction("Genres");
        }
    }
}
