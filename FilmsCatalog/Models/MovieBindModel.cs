using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace FilmsCatalog.Models
{
    public sealed class MovieBindModel
    {
        public string Description { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTimeOffset ReleaseDate { get; set; } 
        [Required]
        public string Director { get; set; }
        public IFormFile Poster { get; set; }
        public string PosterPath { get; set; }
        public uint Id { get; set; }
    }
}