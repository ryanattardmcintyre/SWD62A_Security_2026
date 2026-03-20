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
        public IActionResult Create(Blog b, string Public, IFormFile file, [FromServices] IWebHostEnvironment host)
        {
            string relativePath = "";

            if(file != null)
            {
                //wwwroot is public
                //outside the wwwroot folder is private, only accessible by the server

                if(System.IO.Path.GetExtension(file.FileName).ToLower() != ".jpg")
                {
                    //for this to appear we must create a span with asp-validation-for="file" in the view
                    ModelState.AddModelError("file", "Only .jpg files are allowed");
                    return View(b);
                }

                //this mitigation is more important - setting up a whitelist of allowed extensions      
                //FF D8
                using (var myFileStream = file.OpenReadStream())
                {
                    byte[] fileHeader = new byte[2];
                    myFileStream.Read(fileHeader, 0, 2); //pointer moved two positions
                    if (fileHeader[0] != 0xFF || fileHeader[1] != 0xD8)
                    {
                        ModelState.AddModelError("file", "The file content does not match the .jpg format");
                        return View(b);
                    }
                }

                string uniqueFilename = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(file.FileName);
                string absolutePath = "";
                if (b.Public == true)
                {
                   absolutePath = host.WebRootPath + "/publicBlogImages";
                    relativePath = "/publicBlogImages/" + uniqueFilename;
                }
                else 
                {
                    relativePath = "/privateBlogImages/" + uniqueFilename;
                    absolutePath = host.ContentRootPath + "/privateBlogImages";
                }

                absolutePath += "/" + uniqueFilename;

                using (var myFileStream = new FileStream(absolutePath,
                    FileMode.CreateNew, FileAccess.Write))
                {
                    file.CopyTo(myFileStream);
                }
            }

            //note: applying validators in the controller requires forcing them to run
            //ModelState.IsValid will force the validators to run. be aware validators will
            //not always run on the client side

            if (Public == "1") b.Public = true; else b.Public = false;

            ModelState.Remove("Public");

            if (ModelState.IsValid == false)
            {
                TempData["error"] = "Validation failed, please correct the errors and try again";
                return View(b);
            }   

            b.AuthorEmail = ""; b.FilePath = "";
            b.CreatedAt = DateTime.Now;

            b.Title = System.Net.WebUtility.HtmlEncode(b.Title);
            b.Content = System.Net.WebUtility.HtmlEncode(b.Content);

            b.FilePath = relativePath;

            _blogsRepository.AddBlog(b);
            TempData["success"] = "Blog added successfully";
            return View();
        }
    }
}
