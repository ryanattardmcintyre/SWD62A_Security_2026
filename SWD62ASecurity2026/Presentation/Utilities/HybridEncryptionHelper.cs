using System.Security.Cryptography;
using System.Text;

namespace Presentation.Utilities
{
    public class HybridEncryptionHelper
    {
        public static MemoryStream Encrypt(MemoryStream msIn,
            string publicKey)
        {

            //1. Ensure that the asymmetric parameters were generated and you're passing ONLY
            //   the PUBLIC KEY

            //2. Generete the symmetric parameters
            SymmetricEncryptionHelper symmetricEncryptionHelper = new SymmetricEncryptionHelper();
            var myParams = symmetricEncryptionHelper.GenerateParameters(1, 
                Aes.Create(), null);

            //Note:
            //Symmetric parameters will be used to encrypt the data in msIn
            //ASymmetric parameters will be used to encrypt the symmetric parameters

            //3. Encrypt the data in msIn using the Symmetric Parameters
            msIn.Position = 0; //reset the position of the stream to the beginning
            byte[] cipher = 
                symmetricEncryptionHelper.Encrypt(Aes.Create(), myParams, msIn.ToArray());


            //4. Encrypt the symmetric parameters using the public key
            string encryptedSecretKey = AsymmetricEncryptionHelper.Encrypt(
                Convert.ToBase64String(myParams.Key), publicKey);
            string encryptedIV = AsymmetricEncryptionHelper.Encrypt(
                Convert.ToBase64String(myParams.IV), publicKey);

            //5. Store in the outgoing file the data in the following order:
            //   a. the encrypted secret key <= it has a fixed length
            //   b. the encrypted IV <= it has a fixed length
            //   c. the encrypted data <= it has a variable length

            MemoryStream msOut = new MemoryStream();
            byte[] encryptedSecretKeyBytes = Convert.FromBase64String(encryptedSecretKey);
            byte[] encryptedIvBytes = Convert.FromBase64String(encryptedIV);

            msOut.Write(encryptedSecretKeyBytes, 0, encryptedSecretKeyBytes.Length);
            msOut.Write(encryptedIvBytes, 0, encryptedIvBytes.Length);

            MemoryStream msForLargeCipher = new MemoryStream(cipher);
            msForLargeCipher.Position = 0;
            msForLargeCipher.CopyTo(msOut);

            //6. return the outgoing file as a MemoryStream with the bytes converted into a string

            msOut.Position = 0;
            return msOut;
        }

        public MemoryStream Decrypt(MemoryStream msInCipher, string privateKey)
        {
            //1. Ensure that you pass the private key - the only key that can decrypt!

            //2. open the msInCipher and start reading "in a split way" the various data we have in that stream
            //2.1 read the first X ("128") no of bytes which represent the encrypted secret key
            //2.2 read the next X ("128") no of bytes which represent the encrypted IV
            //2.4 read the rest until end of file which represent the encrypted data and store it in MemoryStream

            //3. Decrypt the encrypted secret key and IV using the private key
            //4. Decrypt the encrypted data using the decrypted secret key and IV
            //5 . return the decrypted data as a MemoryStream
            //6. (where eventually you will have to use Encoding.<charset>.GetString()) if you're dealing with a string
            return new MemoryStream();
        }
    }
}
