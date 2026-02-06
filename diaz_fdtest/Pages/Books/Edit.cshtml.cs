using diaz_fdtest.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace diaz_fdtest.Pages.Books
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<ApplicationUser> _userManager;

        public EditModel(
            AppDbContext context,
            IWebHostEnvironment environment,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _environment = environment;
            _userManager = userManager;
        }

        [BindProperty]
        public Book Book { get; set; }

        [BindProperty]
        public IFormFile? ImageUpload { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Book = await _context.Books.FindAsync(id);

            if (Book == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null || Book.Author != user.FullName)
            {
                return RedirectToPage("./Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var bookToUpdate = await _context.Books.FindAsync(Book.Id);

            if (bookToUpdate == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null || bookToUpdate.Author != user.FullName)
            {
                return RedirectToPage("./Index");
            }

            bookToUpdate.Title = Book.Title;
            bookToUpdate.Description = Book.Description;
            bookToUpdate.Rating = Book.Rating;

            if (ImageUpload != null)
            {
                if (!string.IsNullOrEmpty(bookToUpdate.ImageThumbnail))
                {
                    var oldPath = Path.Combine(_environment.WebRootPath, bookToUpdate.ImageThumbnail.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                var uploadFolder = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageUpload.FileName;
                var filePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageUpload.CopyToAsync(fileStream);
                }

                bookToUpdate.ImageThumbnail = "/uploads/" + uniqueFileName;
            }

            ModelState.Remove("Book.Author");
            ModelState.Remove("Book.ImageThumbnail");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}