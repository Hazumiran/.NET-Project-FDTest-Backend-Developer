using diaz_fdtest.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace diaz_fdtest.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public ApplicationUser CurrentUser { get; set; }

        // Data Buku untuk ditampilkan
        public IList<Book> Books { get; set; }

        // Filter & Sorting Properties
        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; } // Pencarian Judul

        [BindProperty(SupportsGet = true)]
        public string BookAuthor { get; set; } // Filter Author

        [BindProperty(SupportsGet = true)]
        public int? MinRating { get; set; } // Filter Rating

        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; } // Sorting (Date/Rating)

        // Dropdown Lists
        public SelectList AuthorsList { get; set; }

        // Pagination Properties
        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;
        public int TotalPages { get; set; }
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public async Task OnGetAsync()
        {
            // 1. Logika User Login (Mempertahankan kode lama Anda)
            if (User.Identity.IsAuthenticated)
            {
                CurrentUser = await _userManager.GetUserAsync(User);
            }

            // 2. Query Awal
            var booksQuery = _context.Books.AsQueryable();

            // 3. FILTERING
            // Filter by Title
            if (!string.IsNullOrEmpty(SearchString))
            {
                booksQuery = booksQuery.Where(b => b.Title.Contains(SearchString));
            }

            // Filter by Author
            if (!string.IsNullOrEmpty(BookAuthor))
            {
                booksQuery = booksQuery.Where(b => b.Author == BookAuthor);
            }

            // Filter by Rating (Minimum Rating)
            if (MinRating.HasValue)
            {
                booksQuery = booksQuery.Where(b => b.Rating >= MinRating.Value);
            }

            // 4. SORTING (Filter by Date Uploaded & Rating)
            switch (SortOrder)
            {
                case "date_asc":
                    booksQuery = booksQuery.OrderBy(b => b.CreatedAt);
                    break;
                case "rating_desc":
                    booksQuery = booksQuery.OrderByDescending(b => b.Rating);
                    break;
                case "rating_asc":
                    booksQuery = booksQuery.OrderBy(b => b.Rating);
                    break;
                case "date_desc":
                default:
                    booksQuery = booksQuery.OrderByDescending(b => b.CreatedAt); // Default: Terbaru
                    break;
            }

            // 5. Populate Dropdown Author (Mengambil list author unik dari DB)
            IQueryable<string> authorQuery = _context.Books.OrderBy(b => b.Author).Select(b => b.Author).Distinct();
            AuthorsList = new SelectList(await authorQuery.ToListAsync());

            // 6. PAGINATION LOGIC
            int pageSize = 6; // Menampilkan 6 buku per halaman
            var count = await booksQuery.CountAsync();
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            Books = await booksQuery
                .Skip((PageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}