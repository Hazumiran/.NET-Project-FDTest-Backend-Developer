using diaz_fdtest.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class IndexModelBooks : PageModel
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _environment;
    private readonly UserManager<ApplicationUser> _userManager;

    public IndexModelBooks(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public List<Book> Books { get; set; } = new List<Book>();
    public ApplicationUser CurrentUser { get; set; }

    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user != null)
        {
            CurrentUser = user;

            Books = await _context.Books
                .Where(b => b.Author == user.FullName)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }
        else
        {
            Books = new List<Book>();
        }
    }
    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var book = await _context.Books.FindAsync(id);

        if (book != null)
        {
            if (!string.IsNullOrEmpty(book.ImageThumbnail))
            {
                var filePath = Path.Combine(_environment.WebRootPath, book.ImageThumbnail.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage();
    }
}
