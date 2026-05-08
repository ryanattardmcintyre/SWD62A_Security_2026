using DataAccess.Repositories;
using Domain.Exceptions;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Filters;
using System.Runtime.Intrinsics.Arm;

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

            if (file != null)
            {
                //wwwroot is public
                //outside the wwwroot folder is private, only accessible by the server

                if (System.IO.Path.GetExtension(file.FileName).ToLower() != ".jpg")
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


        [HasDeletePermission("admin")]
        public IActionResult Delete(int id)
        {
            _blogsRepository.DeleteBlog(id);
            return Content("Blog with id " + id + " has been deleted");
        }



        public IActionResult TestPermissionsSaving()
        {
            try
            {
                SharingPermission p1 = new SharingPermission()
                {
                    BlogFK = 4,
                    UserEmail = "ryanattard@gmail.com",
                    PermissionType = "Read"
                };

                SharingPermission p2 = new SharingPermission()
                {
                    BlogFK = 4,
                    UserEmail = "joeborg@gmail.com",
                    PermissionType = "Read"
                };

                List<SharingPermission> myPermissions = new List<SharingPermission>();
                myPermissions.Add(p1); myPermissions.Add(p2);

                _blogsRepository.UpdatePermissionsOnBlog(myPermissions.ToArray());

                return Content("permissions saved");
            }
            catch (BlogsException ex)
            {
                //email the admin of the site

                return Content("Error while updating permissions");
            }
            catch (Exception ex)
            {
                //log the ex.Message and ex.StackTrace in a file or cloud

                return Content("Error - we 've logged the error ;try again later");
            }
        }


        public IActionResult Details(string id)
        {
            Presentation.Utilities.SymmetricEncryptionHelper encryptionHelper = 
                new Presentation.Utilities.SymmetricEncryptionHelper();

            var myParams =
                  encryptionHelper.GenerateParameters(2, System.Security.Cryptography.Aes.Create(),
                  "p@$$w0rd1234567");


            //What conversion method should you use? the Convert or the Encoding?

            id = id.Replace("-", "+").Replace("_", "/").Replace("|", "=");

            byte[] cipher = Convert.FromBase64String(id);



            byte [] clearTextAsBytes =encryptionHelper.Decrypt(System.Security.Cryptography.Aes.Create()
                , myParams, cipher);

            string originalId = System.Text.Encoding.UTF8.GetString(clearTextAsBytes);


            Blog b = _blogsRepository.GetBlogs().
                FirstOrDefault(b => b.Id == Convert.ToInt32(originalId));   
            if (b == null) return Content("Blog not found");
            return View(b);
        }
    }
}
