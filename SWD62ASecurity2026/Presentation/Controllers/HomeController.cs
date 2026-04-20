using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;
using Presentation.Utilities;
using System.Diagnostics;
using System.Text;

namespace Presentation.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }


        public IActionResult WorksheetHashing()
        {

            //Part 1

            HashingHelper myHashingHelper = new HashingHelper();

            string input1 = "Transfer 250 EUR to account 458923";
            string digest1 = myHashingHelper.Hash(input1);
            bool tampered1 = digest1 == "b4Ff8n/2dWtxSGl/X8qzxF/DwfFabzEVmo23SftOw01RZNmkGQMUwCqCyb0cToKyjIEVNsoI0sWZqrUHsIg60A==";
            
            string input2 = "Transfer 2500 EUR to account 458923";
            string digest2 = myHashingHelper.Hash(input2);
            bool tampered2 = digest2 == "XEWsgeU/mmxf5URp9hm4+Ae0IrH1MRMu0nwGZ3Kb4JaQfVUdXeZIxfiPtvxRv0Kp4Vm98DLBpx3wrGpNgmeTPg==";

            string input3 = "Transfer 250 EUR to account 458923...";
            string digest3 = myHashingHelper.Hash(input3);
            bool tampered3 = digest3== "fpMvTBCMXiaaCs6cKNvxZRcDwYIgsdjIZ8LEns7eiC3fPhaMmdbeJHtQ8nb0WNFmjGcuZd2o8GBdaV3egqD8mQ==";


            //Part 2

            string collisionDetection1 = "SECURITY";
            string collisionDetection2 = "YTRUICES";

            int collisionHash1 = myHashingHelper.AsciiHashing(collisionDetection1);
            int collisionHash2 = myHashingHelper.AsciiHashing(collisionDetection2);

            string collisionResult1 = myHashingHelper.Hash(collisionDetection1);
            string collisionResult2 = myHashingHelper.Hash(collisionDetection2);


            byte[] salt = new byte[16] ;
            Random r = new Random();
            for (int i = 0; i < salt.Length; i++)
            {
                salt[i] = Convert.ToByte(r.Next(256));
            }

            byte[] digestFromSalt =  myHashingHelper.Hash(Encoding.UTF8.GetBytes("hello world"), salt);
            string digestStringFromSalt = Convert.ToBase64String(digestFromSalt);


            return Content($"input 1 was tampered: {!tampered1} | input 2 was tampered: {!tampered2} | input 3 was tampered: {!tampered3}");
        }


        public IActionResult Index()
        {

            HashingHelper myHashingHelper = new HashingHelper();
            return Content(myHashingHelper.Hash("hello world"));

            //return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //centralized error handler for standardized status codes
        public IActionResult StatusCode(int code)
        {

            //the original url that was typed in the browser
            var statusCodeData = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            //log


            _logger.LogWarning("Status code {code} occurred while accessing {originalPath} with query string {originalQueryString}",
              code, statusCodeData?.OriginalPath, statusCodeData?.OriginalQueryString);

            switch (code)
            {
                case 404:
                    return Content("The resource you are looking for was not found.");
                    break;
            }

            return Content("done");

        }
    }
}
