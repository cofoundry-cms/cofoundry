using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PasswordGenerationService : IPasswordGenerationService
    {
        private const string ALLOWED_CHARACTERS = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ23456789";
        private const int DEFAULT_LENGTH = 10;

        public string Generate()
        {
            return Generate(DEFAULT_LENGTH);
        }

        public string Generate(int passwordLength)
        {
            Byte[] randomBytes = new Byte[passwordLength];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomBytes);
            }
            char[] chars = new char[passwordLength];
            int allowedCharCount = ALLOWED_CHARACTERS.Length;

            for (int i = 0; i < passwordLength; i++)
            {
                chars[i] = ALLOWED_CHARACTERS[(int)randomBytes[i] % allowedCharCount];
            }

            return new string(chars);
        }
    }
}
