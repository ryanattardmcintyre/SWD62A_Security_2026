using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Presentation.Utilities
{

    public class SymmetricParameters
    {
        public byte[] Key { get; set; }
        public byte[] IV { get; set; }
    }

    public class SymmetricEncryptionHelper
    {
        public SymmetricParameters GenerateParameters(int choiceOfGeneration, SymmetricAlgorithm algorithm, string? password)
        {

            SymmetricParameters parameters = new SymmetricParameters();

            switch (choiceOfGeneration)
            {
                case 0:
                    //Approach 1: you randomly generate a number of bytes for the key and IV
                    parameters.Key = new byte[algorithm.KeySize / 8];
                    for (int i = 0; i < parameters.Key.Length; i++)
                    {
                        parameters.Key[i] = (byte)i;
                    }

                    parameters.IV = new byte[algorithm.BlockSize / 8];
                    for (int i = 0; i < parameters.IV.Length; i++)
                    {
                        parameters.IV[i] = (byte)i;
                    }

                    break;

                case 1:
                    //approach 2: you use the algorithm instance to generate the key and IV for you
                    algorithm.GenerateKey(); algorithm.GenerateIV();
                    parameters.Key = algorithm.Key;
                    parameters.IV = algorithm.IV;

                    break;

                case 2:
                    //approach 3: you can use a helper class like Rfc2898DeriveBytes to generate the key and IV for you based on a password and salt
                    Rfc2898DeriveBytes rfc2898DeriveBytes
                        = new Rfc2898DeriveBytes(
                            Encoding.UTF8.GetBytes(password ?? string.Empty), //human generated input
                            new byte[] { 23, 45, 90, 150, 250, 230, 201, 190, 43, 45, 65, 23 },
                            1000,
                            HashAlgorithmName.SHA256
                            );

                    parameters.Key = rfc2898DeriveBytes.GetBytes(algorithm.KeySize / 8);
                    parameters.IV = rfc2898DeriveBytes.GetBytes(algorithm.BlockSize / 8);
                    break;
            }
            return parameters;
        }


        public byte[] Encrypt(SymmetricAlgorithm alg, SymmetricParameters paramters, byte[] data)
        {
            alg.Key = paramters.Key;
            alg.IV = paramters.IV;

            //Memorystream its just a stream of data which holds your data temporarily in memory
            MemoryStream outputStream = new MemoryStream();

            MemoryStream inputStream = new MemoryStream(data);
            inputStream.Position = 0;

            using (CryptoStream cryptoStream =
                new CryptoStream(inputStream, alg.CreateEncryptor(), CryptoStreamMode.Read))
            {
                cryptoStream.CopyTo(outputStream);
            }
            outputStream.Position = 0;
            return outputStream.ToArray();
        }

        public byte[] Decrypt(SymmetricAlgorithm alg, SymmetricParameters parameters, byte[] cipher)
        {
            alg.Key = parameters.Key;
            alg.IV = parameters.IV;

            //Memorystream its just a stream of data which holds your data temporarily in memory
            MemoryStream outputStream = new MemoryStream();

            MemoryStream inputStream = new MemoryStream(cipher);
            inputStream.Position = 0;

            using (CryptoStream cryptoStream =
                new CryptoStream(inputStream, alg.CreateDecryptor(), CryptoStreamMode.Read))
            {
                cryptoStream.CopyTo(outputStream);
            }
            outputStream.Position = 0;
            return outputStream.ToArray();
        }

        //This method uses MemoryStream only
        public MemoryStream Encrypt(MemoryStream msIn, SymmetricAlgorithm alg, SymmetricParameters parameters)
        {
            alg.Key = parameters.Key; alg.IV = parameters.IV;

            MemoryStream msOut = new MemoryStream(); //encrypted data goes here

            CryptoStream cryptoStream = new CryptoStream(msIn, alg.CreateEncryptor(), CryptoStreamMode.Read);
            cryptoStream.CopyTo(msOut);

            return msOut;
        }

        //This method uses byte[]
        public void Encrypt(string fileInPath, string fileOutPath,
            SymmetricAlgorithm alg, SymmetricParameters parameters)
        {
            alg.Key = parameters.Key; alg.IV = parameters.IV;

            using (FileStream fsIn = new FileStream(fileInPath, FileMode.Open, FileAccess.Read))
            {
                using (FileStream fsOut = new FileStream(fileOutPath, FileMode.Create, FileAccess.Write))
                {
                    CryptoStream cryptoStream = new CryptoStream(fsIn, alg.CreateEncryptor(), CryptoStreamMode.Read);
                    cryptoStream.CopyTo(fsOut);
                }
            }
        }
        public void Decrypt(string fileInPath, string fileOutPath,
            SymmetricAlgorithm alg, SymmetricParameters parameters)
        {
            alg.Key = parameters.Key; alg.IV = parameters.IV;
            using (FileStream fsIn = new FileStream(fileInPath, FileMode.Open, FileAccess.Read))
            {
                using (FileStream fsOut = new FileStream(fileOutPath, FileMode.Create, FileAccess.Write))
                {
                    CryptoStream cryptoStream = new CryptoStream(fsIn,
                        alg.CreateDecryptor(), CryptoStreamMode.Read);
                    cryptoStream.CopyTo(fsOut);
                }
            }
        }
    }
}
