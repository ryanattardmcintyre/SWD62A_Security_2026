using System.Security.Cryptography;

namespace Presentation.Utilities
{
    public class DigitalSigningHelper
    {
        public static string SignData(byte[] data, string privateKey)
        {
            RSA myAlg = RSA.Create();
            myAlg.FromXmlString(privateKey);

            HashingHelper hashingHelper = new HashingHelper();
            byte[] digest = hashingHelper.Hash(data);

            byte [] signature = myAlg.SignHash(digest, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);
            return Convert.ToBase64String(signature);
        }

        public static bool VerifyData(byte[] data, string publicKey, string signature)
        {
            RSA myAlg = RSA.Create();
            myAlg.FromXmlString(publicKey);

            HashingHelper hashingHelper = new HashingHelper();
            byte[] digest = hashingHelper.Hash(data);

            byte[] signatureBytes = Convert.FromBase64String(signature);


            bool valid = myAlg.VerifyHash(digest, signatureBytes, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);
            return valid;

        }
    }
}
