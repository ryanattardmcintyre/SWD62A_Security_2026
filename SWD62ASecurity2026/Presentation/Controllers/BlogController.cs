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

            //note: applying validators in the controller requires forcing them to run
            //ModelState.IsValid will force the validators to run. be aware validators will
            //not always run on the client side
            if(ModelState.IsValid == false)
            {
                TempData["error"] = "Validation failed, please correct the errors and try again";
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
