using diaz_fdtest.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace diaz_fdtest.Pages.Books
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public DeleteModel(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
        public Book Book { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Book = await _context.Books.FindAsync(id);

            if (Book == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
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

            return RedirectToPage("Index");
        }
    }
}