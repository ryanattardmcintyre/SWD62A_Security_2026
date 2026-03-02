using DataAccess.Repositories;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    public class BlogController : Controller
    {
        private BlogRepository _blogsRepository;
        public BlogController(BlogRepository blogRepository) { _blogsRepository = blogRepository; }
        public IActionResult Index()
        {
            return View(_blogsRepository.GetBlogs());
        }

        [HttpGet]
        public IActionResult Create()
        { return View(); }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Blog b)
        { 

            string regex = @"^[a-z][A-Z][0-9]*$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(b.Content, regex))
            {
                TempData["error"] = "Content contains prohibited symbols";
                return View(b);
            }



            b.AuthorEmail = ""; b.FilePath = "";
            b.CreatedAt = DateTime.Now;

            b.Title = System.Net.WebUtility.HtmlEncode(b.Title);
            b.Content = System.Net.WebUtility.HtmlEncode(b.Content);

            _blogsRepository.AddBlog(b);
            TempData["success"] = "Blog added successfully";
            return View();
        }
    }
}
