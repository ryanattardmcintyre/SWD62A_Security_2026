using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Presentation.Controllers
{
    public class VulnerableController : Controller
    {
        public IActionResult Index(string filename)
        {

            string absolutePath = "privateBlogImages/" + System.IO.Path.GetFileName(filename);
            if(System.IO.File.Exists(absolutePath) == false)
            {
                return Content("File doesn't exist");//returns a string //echo > ContentResult
            }
            else
            {
                byte [] fileBytes = System.IO.File.ReadAllBytes(absolutePath);
                return File(fileBytes, "application/octet-stream", filename); //returns a file > FileResult
            }
        }

        public IActionResult Resize(string file)
        {
            string command = "/c echo Starting conversion of " + file + " && echo Conversion finished";
            var psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = command,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            return Content("Output: \r\n" + output + "\r\nError:\r\n" + error);
        }
    }
}
