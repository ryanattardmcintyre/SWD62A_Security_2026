using System.Security.Cryptography;
using System.Text;

namespace Presentation.Utilities
{
    public class HashingHelper
    {


        public int AsciiHashing(string input)
        {
              int sum = 0;
            
               foreach (char c in input)
                   sum += c;

            int weakHash = sum % 256;

            return weakHash;
        }
        public string Hash(string input)
        {
            //Convert string into an array of bytes
            //Cryptographic value can be converted to a byte array using Convert.FromBase64String.....
            //Cryptographic value can be converted to a string using Convert.Tobase64String
            //User-input/Human readable value can be converted to a byte array using Encoding.UTF8.GetBytes(input)


            byte[] inputAsBytes = Encoding.UTF8.GetBytes(input);
            byte[] digestAsBytes = Hash(inputAsBytes);

            string digestResult = Convert.ToBase64String(digestAsBytes);

            return digestResult;
        }

        public byte[] Hash(byte[] input)
        { 
          //MD5, SHA1, SHA256, SHA384, SHA512

            using (var sha512 = SHA512.Create())
                {
                    return sha512.ComputeHash(input);
            }


        }

        //salt is a random value that is generated ideally for every hash operation you run
        //e.g. if you are hashing passwords, you can generate a random of bytes (salt) you pass them in the algorithm
        //     and in the database together with the Password hash, you store also the salt

        public byte[] Hash(byte[] input, byte[] salt)
        {
            HMACSHA512 hmac = new HMACSHA512(salt);
            return hmac.ComputeHash(input);
        }
    }
}
