using System;
using IO = System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

using FilmsCatalog.Data;
using FilmsCatalog.Models;
using FilmsCatalog.Attributes;
using FilmsCatalog.ModelBinders;

namespace FilmsCatalog.Controllers
{
    [AllowAnonymous]
    public sealed class MoviesController: Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _environment;

        public MoviesController(
            ApplicationDbContext dbContext, 
            UserManager<User> userManager,
            IWebHostEnvironment environment
        )
        {
            _dbContext = dbContext
                ?? throw new ArgumentNullException(nameof(dbContext));

            _userManager = userManager
                ?? throw new ArgumentNullException(nameof(userManager));

            _environment = environment
                ?? throw new ArgumentNullException(nameof(environment));
        }

        [Authorize, HttpGet]
        public ActionResult Add() => View();

        [Authorize, HttpPost]
        public async Task<IActionResult> Add(
            MovieBindModel model, 
            CancellationToken token
        )
        {
            if (ModelState.IsValid == false) return View();

            var user = await _userManager.GetUserAsync(User);
            var newMovie = new Movie {
                Description = model.Description,
                Title = model.Title,
                Director = model.Director,
                ReleaseDate = model.ReleaseDate,
                OwnerId = user.Id
            };

            if (model.Poster != null)
                newMovie.PosterPath = await LoadImage(model.Poster);

            await _dbContext.Movies.AddAsync(newMovie, token);
            await _dbContext.SaveChangesAsync(token);

            return Redirect($"/Home/Show/{newMovie.MovieId}");
        }

        [Authorize, HttpGet, EnsureMovieExists]
        public IActionResult Edit(
            [ModelBinder(typeof(MovieModelBinder))]
            Movie selectedMovie
        )
        {
            var userId = _userManager.GetUserId(User);
            if (selectedMovie.OwnerId != userId) return Forbid(); 

            var viewModel = new MovieBindModel {
                Description = selectedMovie.Description,
                Director = selectedMovie.Director,
                ReleaseDate = selectedMovie.ReleaseDate,
                Title = selectedMovie.Title,
                Id = selectedMovie.MovieId,
                PosterPath = selectedMovie.PosterPath
            };

            return View(viewModel);
        }

        [Authorize, HttpPost, EnsureMovieExists]
        public async Task<IActionResult> Edit(
            [ModelBinder(BinderType = typeof(MovieModelBinder))]
            Movie selectedMovie,
            MovieBindModel model,
            CancellationToken token = default
        )
        {
            var userId = _userManager.GetUserId(User);
            if (selectedMovie.OwnerId != userId) return Forbid(); 

            selectedMovie.Description = model.Description;
            selectedMovie.Director = model.Director;
            selectedMovie.ReleaseDate = model.ReleaseDate;
            selectedMovie.Title = model.Title;

            var oldFileName = IO.Path.GetFileName(selectedMovie.PosterPath);
            if(model.Poster != null && oldFileName != model.Poster.FileName)
            {
                var localPosterPath = _environment.WebRootPath + selectedMovie.PosterPath;
                IO.File.Delete(localPosterPath);

                selectedMovie.PosterPath = await LoadImage(model.Poster, token);
            }

            await _dbContext.SaveChangesAsync();

            return Redirect($"/Home/Show/{selectedMovie.MovieId}");
        }

        private async Task<string> LoadImage(IFormFile image, CancellationToken token = default)
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var extension = IO.Path.GetExtension(image.FileName);
            var fileName = $"{timestamp}{extension}";
            var filePath = IO.Path.Combine("Posters",fileName);
            var localPath = IO.Path.Combine(_environment.WebRootPath, filePath);
            using var stream = IO.File.Open(localPath, IO.FileMode.OpenOrCreate);
            await image.CopyToAsync(stream, token);

            return "/" + filePath;
        }
    }
}