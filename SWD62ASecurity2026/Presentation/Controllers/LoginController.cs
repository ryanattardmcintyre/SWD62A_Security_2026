using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    public class ZapController : Controller
    {

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if(username == "admin" && password == "password")
            {
                // Authentication successful
                return Ok("Login successful");
            }
            else
            {
                // Authentication failed
                return Unauthorized("Invalid username or password");
            }
        }
    }
}
