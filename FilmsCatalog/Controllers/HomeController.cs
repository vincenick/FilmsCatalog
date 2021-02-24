using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using FilmsCatalog.Data;
using FilmsCatalog.Models;
using FilmsCatalog.Attributes;
using FilmsCatalog.ModelBinders;
using FilmsCatalog.Extensions;

namespace FilmsCatalog.Controllers
{
    public class HomeController : Controller
    {
        private static readonly string DEFAULT_POSTER_URI = "https://via.placeholder.com/100x100";

        public async Task<IActionResult> Index(
            [FromServices] ApplicationDbContext dbContext,
            uint? page, 
            uint? size,
            CancellationToken token = default
        )
        {
            var requestedPage = page ?? 0;
            var requestSize = size ?? 10;
            var movies =
                from m in dbContext.Movies.AsNoTracking()
                orderby m.Title
                select m;
            var currentPage = await Page<Movie>.CreateAsync(
                source: movies, 
                pageIndex: requestedPage, 
                pageSize: requestSize, 
                token
            );

            return View(currentPage);
        }

        [EnsureMovieExists]
        public IActionResult Show(
            [ModelBinder(BinderType = typeof(MovieModelBinder))]
            Movie selectedMovie,
            [FromServices] UserManager<User> userManager
        )
        {
            var userId = userManager.GetUserId(User);
            var viewModel = new MovieViewModel {
                Id = selectedMovie.MovieId,
                Director = selectedMovie.Director,
                Title = selectedMovie.Title,
                ReleaseDate = selectedMovie.ReleaseDate,
                Owner = selectedMovie.Owner.UserName,
                PosterPath = selectedMovie.PosterPath.IsNullOrEmpty()
                    ? DEFAULT_POSTER_URI
                    : selectedMovie.PosterPath,
                Description = selectedMovie.Description.IsNullOrEmpty()
                    ? "Description is not provied"
                    : selectedMovie.Description,
                IsEditable = selectedMovie.OwnerId == userId
            };

            return View(viewModel);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
