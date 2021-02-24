using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

using FilmsCatalog.Data;
using FilmsCatalog.Extensions;

namespace FilmsCatalog.ModelBinders
{
    public sealed class MovieModelBinder: IModelBinder
    {
        private readonly ApplicationDbContext _dbContext;
        public MovieModelBinder(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext
                ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext) 
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue("id");
            if (valueProviderResult == ValueProviderResult.None)
                return;

            var movieValue = valueProviderResult.FirstValue;
            if (movieValue.IsNullOrEmpty())
                return;

            if (uint.TryParse(movieValue, out uint movieId) == false)
                return;

            var movie = await _dbContext.Movies
                .Include(m => m.Owner)
                .SingleOrDefaultAsync(x => x.MovieId == movieId);
            if (movie == null)
                return;

            bindingContext.Result = ModelBindingResult.Success(movie);
        }
    }
}