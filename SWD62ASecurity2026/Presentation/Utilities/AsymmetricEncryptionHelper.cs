using System.Security.Cryptography;

namespace Presentation.Utilities
{
    public class AsymmetricParameters
    {
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
    }
    public class AsymmetricEncryptionHelper
    {
        public static AsymmetricParameters GenerateKeys()
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            AsymmetricParameters parameters = new AsymmetricParameters
            {
                PublicKey = rsa.ToXmlString(false), //do not expert private key
                PrivateKey = rsa.ToXmlString(true)
            };

            return parameters;
        }

        public static string Encrypt(string data, string publicKey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(publicKey);
            byte[] encryptedData = rsa.Encrypt(System.Text.Encoding.UTF8.GetBytes(data), false);
            return Convert.ToBase64String(encryptedData);
        }

        public static string Decrypt(string encryptedData, string privateKey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(privateKey);
            byte[] decryptedData = rsa.Decrypt(Convert.FromBase64String(encryptedData), false);
            return System.Text.Encoding.UTF8.GetString(decryptedData);
        }

    }
}
