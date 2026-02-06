using diaz_fdtest.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace diaz_fdtest.Pages.Users
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public List<ApplicationUser>? Users { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool? Verified { get; set; }

        public async Task OnGetAsync()
        {
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(Search))
            {
                query = query.Where(u =>
                    u.FullName.Contains(Search) ||
                    (u.Email != null && u.Email.Contains(Search)));
            }

            if (Verified.HasValue)
            {
                query = query.Where(u => u.EmailConfirmed == Verified.Value);
            }

            Users = await query
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }
    }
}
