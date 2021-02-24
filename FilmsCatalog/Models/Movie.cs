using System;

namespace FilmsCatalog.Models
{
    public sealed class Movie
    {
        public uint MovieId { get; set; }
        public string Title { get; set; }
        public DateTimeOffset ReleaseDate { get; set; }
        public string Director { get; set; }
        public string Description { get; set; }
        public string PosterPath {get; set; } 
        public string OwnerId { get; set; }
        public User Owner{ get; set; }
    }
}