using System;
using System.ComponentModel.DataAnnotations;

namespace FilmsCatalog.Models
{
    public sealed class MovieViewModel
    {
        public uint Id { get; set; }
        public string Director { get; set; }
        public string Title { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        public string Owner { get; set; }
        public DateTimeOffset ReleaseDate { get; set; }
        public string PosterPath { get; set; }
        public bool IsEditable { get; set; }
    }
}