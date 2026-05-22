using Microsoft.AspNetCore.Mvc;
using Presentation.Utilities;
using System.Security.Cryptography;
using System.Text;

namespace Presentation.Controllers
{
    public class TestController : Controller
    {
        private bool VerifyKeyPair(byte[] privateKeyBytes, byte[] publicKeyBytes)
        {
            bool foundMatchingPublicKey = false;
            try
            {
                using (RSA privateKeyRsa = RSA.Create())
                using (RSA publicKeyRsa = RSA.Create())
                {
                    privateKeyRsa.ImportRSAPrivateKey(privateKeyBytes, out _);
                    publicKeyRsa.ImportRSAPublicKey(publicKeyBytes, out _);
                    // Test the keys by signing and verifying a message
                    byte[] testMessage = Encoding.UTF8.GetBytes("TestMessage");

                    SHA256 hashAlg = SHA256.Create();
                    byte[] digest = hashAlg.ComputeHash(testMessage);


                    byte[] signature =
                        privateKeyRsa.SignHash(digest, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                    //Answer2: verify signature produced here
                    bool tempOutput =
                        publicKeyRsa.VerifyHash(digest, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                    if (tempOutput) return true;
                }
            }
            catch
            {
                return false; // If any error occurs, the keys do not match
            }
            return false;
        }

        public IActionResult Index()
        {
            string digest = "vniFoU8cYTPLrO7eqi4IHOBPSCQSdPVe4tn5+SXJOJc=";
            SHA256 sHA256 = SHA256.Create();

            string path = "C:\\Users\\attar\\Downloads\\testdata.txt";

            byte[] contents = System.IO.File.ReadAllBytes(path);
            byte [] digestBytes =sHA256.ComputeHash(contents);

            string digestoutput = Convert.ToBase64String(digestBytes);






            string privateKeyPath = "C:\\Users\\attar\\Downloads\\keys3\\privatekey.txt";
            string publicKeyPath1 = "C:\\Users\\attar\\Downloads\\keys3\\publickey1.txt";
            string publicKeyPath2 = "C:\\Users\\attar\\Downloads\\keys3\\publickey2.txt";
            string publicKeyPath3 = "C:\\Users\\attar\\Downloads\\keys3\\publickey3.txt";

            string privateKeyText = System.IO.File.ReadAllText(privateKeyPath);
            byte[] privateKey = Convert.FromBase64String(privateKeyText);

            string publicKeyText1 = System.IO.File.ReadAllText(publicKeyPath1);
            byte[] publicKey1 = Convert.FromBase64String(publicKeyText1);

            string publicKeyText2 = System.IO.File.ReadAllText(publicKeyPath2);
            byte[] publicKey2 = Convert.FromBase64String(publicKeyText2);

            string publicKeyText3 = System.IO.File.ReadAllText(publicKeyPath3);
            byte[] publicKey3 = Convert.FromBase64String(publicKeyText3);


            bool answer1=  VerifyKeyPair(privateKey, publicKey1);
            bool answer2 = VerifyKeyPair(privateKey, publicKey2);
            bool answer3 = VerifyKeyPair(privateKey, publicKey3);


            /*
             * 229,40,188,60,221,98,193,19,22,116,225,239,224,217,11,146,36,89,20,80,216,6,98,237,28,46,89,161,88,8,253,129
,,,,,,,,,,,,,,,212,197,20,130,243,43,210,203,169,193,14,101,206,98,59,92
Cipher: xslyhz/gjwoGk6zUizNFIQ==
             */

            SymmetricEncryptionHelper helper = new SymmetricEncryptionHelper();

            SymmetricParameters symmParams = new SymmetricParameters();
            symmParams.Key = new byte[] { 229, 40, 188, 60, 221, 98, 193, 19, 22, 116, 225, 239, 224, 217, 11, 146, 36, 89, 20, 80, 216, 6, 98, 237, 28, 46, 89, 161, 88, 8, 253, 129 };
            symmParams.IV = new byte[] { 212, 197, 20, 130, 243, 43, 210, 203, 169, 193, 14, 101, 206, 98, 59, 92 };

            var myAlg = Aes.Create();
            myAlg.Mode = CipherMode.ECB;
            myAlg.Padding = PaddingMode.ANSIX923;

            string cipher = "xslyhz/gjwoGk6zUizNFIQ==";
            byte[] cipherAsBytes = Convert.FromBase64String(cipher);


            byte[] originalBytes = helper.Decrypt(myAlg, symmParams, cipherAsBytes);
            string output = Encoding.UTF8.GetString(originalBytes);








            return Content(output);
        }
    }
}
