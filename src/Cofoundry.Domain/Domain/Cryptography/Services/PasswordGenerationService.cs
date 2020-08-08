using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class PasswordGenerationService : IPasswordGenerationService
    {
        private const string ALLOWED_CHARACTERS = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ2345679";

        private const int DEFAULT_LENGTH = 10;

        public string Generate()
        {
            return Generate(DEFAULT_LENGTH);
        }

        public string Generate(int passwordLength)
        {
            var generator = new RandomStringGenerator();

            return generator.Generate(passwordLength, ALLOWED_CHARACTERS);
        }
    }
}
