using Microsoft.AspNetCore.Mvc;
using Presentation.Utilities;
using System.Security.Cryptography;
using System.Text;
namespace Presentation.Controllers
{
    public class CryptographyController : Controller
    {

        public IActionResult TestAsymmetric()
        {
            AsymmetricParameters parameters = AsymmetricEncryptionHelper.GenerateKeys();


            string dataToEncrypt = "This is some data that I want to encrypt";
            string cipher = AsymmetricEncryptionHelper.Encrypt(dataToEncrypt, parameters.PublicKey);

            //--------------------------------------------------

            string originalValue = AsymmetricEncryptionHelper.Decrypt(cipher, parameters.PrivateKey);
            
            return Content($"Cipher is {cipher} Original Value is {originalValue}");

        }

        public IActionResult TestSymmetric()
        { 
            SymmetricEncryptionHelper helper = new SymmetricEncryptionHelper();

            string dataToEncrypt= "This is some data that I want to encrypt";

            var myParams = helper.GenerateParameters(2, Aes.Create(), "p@$$w0rd");

            byte[] dataToEncryptAsBytes = Encoding.UTF8.GetBytes(dataToEncrypt);

            byte[] cipher =
                helper.Encrypt(Aes.Create(), myParams, dataToEncryptAsBytes);



            byte[] originalValue =   helper.Decrypt(Aes.Create(), myParams, cipher);

            string originalValueAsString = Encoding.UTF8.GetString(originalValue);  

            return Content($"Cipher is {Convert.ToBase64String(cipher)} Original Value is {Encoding.UTF8.GetString(originalValue)}");
        }

       
    }
}
