using diaz_fdtest.Data;
using diaz_fdtest.Pages.Books;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;

namespace diaz_fdtest.Tests
{
    public class BookTests
    {
        private AppDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task OnPostAsync_Create_MenambahkanBukuKeDatabase()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = GetDbContext(dbName);
            var mockEnv = new Mock<IWebHostEnvironment>();

            var user = new ApplicationUser { UserName = "testuser", FullName = "Budi Santoso" };
            var mockUserManager = MockHelpers.MockUserManager(new List<ApplicationUser> { user });
            mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            var pageModel = new CreateModel(context, mockUserManager.Object, mockEnv.Object);

            pageModel.Book = new Book
            {
                Title = "Buku Testing",
                Description = "Deskripsi Testing",
                Rating = 5
            };

            var result = await pageModel.OnPostAsync();

            Assert.IsType<RedirectToPageResult>(result);

            var bookInDb = await context.Books.FirstOrDefaultAsync();
            Assert.NotNull(bookInDb);
            Assert.Equal("Buku Testing", bookInDb.Title);
            Assert.Equal("Budi Santoso", bookInDb.Author);
        }

        [Fact]
        public async Task OnPostAsync_Edit_MenolakAkses_JikaBukanPemilik()
        {
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetDbContext(dbName))
            {
                context.Books.Add(new Book
                {
                    Id = 1,
                    Title = "Buku Asli",
                    Author = "User Asli",
                    Description = "Desc",

                    ImageThumbnail = "dummy.jpg",

                    Rating = 5
                });
                await context.SaveChangesAsync();
            }

            var hackerUser = new ApplicationUser { UserName = "hacker", FullName = "Hacker Jahat" };
            var mockUserManager = MockHelpers.MockUserManager(new List<ApplicationUser> { hackerUser });
            mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(hackerUser);

            var mockEnv = new Mock<IWebHostEnvironment>();

            using (var context = GetDbContext(dbName))
            {
                var pageModel = new EditModel(context, mockEnv.Object, mockUserManager.Object);
                pageModel.Book = new Book { Id = 1, Title = "Judul Dirusak", Description = "Hacked", Rating = 1 };

                var result = await pageModel.OnPostAsync();

                var redirectResult = Assert.IsType<RedirectToPageResult>(result);
                Assert.Equal("./Index", redirectResult.PageName);

                var bookInDb = await context.Books.FindAsync(1);
                Assert.Equal("Buku Asli", bookInDb.Title);
            }
        }
    }
}