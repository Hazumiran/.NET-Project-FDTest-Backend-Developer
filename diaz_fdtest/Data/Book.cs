using System;
using System.ComponentModel.DataAnnotations;

namespace diaz_fdtest.Data
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(150)]
        public string Author { get; set; }

        [Required]
        [StringLength(150)]
        public string Description { get; set; }

        [StringLength(150)]
        public string ImageThumbnail { get; set; }

        public int Rating { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
