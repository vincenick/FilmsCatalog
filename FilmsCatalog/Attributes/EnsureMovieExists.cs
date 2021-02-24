using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;

using FilmsCatalog.Data;

namespace FilmsCatalog.Attributes
{
    public sealed class EnsureMovieExistsAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context) { }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var requestedMovieId = uint.Parse(context.RouteData.Values["id"] as string);
            var dbContext = context.HttpContext
                .RequestServices
                .GetRequiredService<ApplicationDbContext>();

            var movieNotExists = dbContext.Movies
                .Any(m => m.MovieId == requestedMovieId) == false;
            if (movieNotExists)
                context.Result = new NotFoundResult();
        }
    }
}