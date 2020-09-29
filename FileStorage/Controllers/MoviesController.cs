using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FileStorage.Data;
using FileStorage.Models;
using Microsoft.AspNetCore.Authorization;
using FileStorage.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.IO.Compression;
using Microsoft.AspNetCore.Hosting;
using System.Runtime.CompilerServices;

namespace FileStorage
{
    [Authorize]
    public class MoviesController : Controller
    {
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly UserManager<FileStorageUser> _userManager;
        private readonly FileStorageContextData _context;
        public string filename;


        public MoviesController(FileStorageContextData context, UserManager<FileStorageUser> userManager, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _appEnvironment = appEnvironment;
        }

        // GET: Movies
        public async Task<IActionResult> Index(string searchString)
        {
            var user = await _userManager.GetUserAsync(User);
            var movies = from m in _context.Movie
                         select m;
            searchString = user.Id;
            if (!String.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(s => s.IDUser.Contains(searchString));
            }
            return View(await movies.ToListAsync());
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            string file_path = Path.Combine(_appEnvironment.ContentRootPath, "Files/" + movie.Name.Replace(Path.GetExtension(movie.Name), ".zip"));
            // Тип файла - content-type
            string file_type = "application/zip";
            // Имя файла - необязательно
            string file_name = movie.Name.Replace(Path.GetExtension(movie.Name), ".zip");
            if (movie == null)
            {
                return NotFound();
            }

            return PhysicalFile(file_path, file_type, file_name);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IDUser,Title,ReleaseDate,Name")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                //var user = await _userManager.GetUserAsync(User);
                //movie.IDUser = user.Id;
                //movie.Title = user.FirstName+" "+user.LastName;
                //movie.ReleaseDate = DateTime.Now;
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IDUser,Title,ReleaseDate,Name")] Movie movie)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movie.FindAsync(id);
            _context.Movie.Remove(movie);
            string path = @".\Files\" + movie.Name.Replace(Path.GetExtension(movie.Name), ".zip");
            FileInfo fileInf = new FileInfo(path);
            if (fileInf.Exists)
            {
                fileInf.Delete();
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("FileUpload")]
        [ValidateAntiForgeryToken]
        [RequestFormLimits(MultipartBodyLengthLimit = 10737418240)]
        [RequestSizeLimit(10737418240)]
        public async Task<ActionResult> FileUpload(IFormFile formFile, [Bind("Id,Title,ReleaseDate")] Movie movie, List<IFormFile> file)
        {
            await UploadFile(file);
            TempData["msg"] = "File Uploaded successfully.";
            var user = await _userManager.GetUserAsync(User);
            movie.IDUser = user.Id;
            movie.Title = user.FirstName + " " + user.LastName;
            movie.ReleaseDate = DateTime.Now;
            movie.Name =filename.Replace(Path.GetExtension(filename), ".zip");
            ZipFile.CreateFromDirectory(@".\Upload", @".\Files\" + movie.Name.Replace(Path.GetExtension(movie.Name), ".zip"));
            DirectoryInfo dirInfo = new DirectoryInfo(@".\Upload");
            foreach (FileInfo File in dirInfo.GetFiles())
            {
                File.Delete();
            }



            if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);

        }

        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 10737418240)]
        [RequestSizeLimit(10737418240)]
        public async Task<bool> UploadFile(List<IFormFile> file)
        {

            string path = "";
            bool iscopied = false;


            foreach (var formFile in file)
            {
                {
                    filename = formFile.FileName;
                    path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "Upload"));

                    using (var filestream = new FileStream(Path.Combine(path, filename), FileMode.Create))
                    {
                        await formFile.CopyToAsync(filestream);
                    }

                }
            }
            return iscopied;

        }

        private bool MovieExists(int id)
        {
            return _context.Movie.Any(e => e.Id == id);
        }
    }
}
