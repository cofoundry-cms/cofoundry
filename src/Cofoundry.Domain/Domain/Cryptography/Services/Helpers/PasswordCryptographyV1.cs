using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    /// <summary>
    /// SHA1, 4 byte salt 
    /// I'm uncertain but it looks liek this came from http://www.aspheute.com/english/20040105.asp.
    /// I've modified it to prepend the encoded salt to the password hash.
    /// </summary>
    public class PasswordCryptographyV1
    {
        const int SALT_LENGTH = 16;
        const char SALT_PAD_CHAR = '!';

        public bool Verify(string password, string hash)
        {
            if (hash == null) throw new ArgumentNullException(nameof(hash));
            if (string.IsNullOrWhiteSpace(hash)) throw new ArgumentEmptyException(nameof(hash));
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (string.IsNullOrEmpty(password)) throw new ArgumentEmptyException(nameof(password));

            // First 16 characters of the hash are the base64 encoded salt
            // BUT salt isn't always 16 characters long because the old method allowed for negative 
            // numbers and some therefore some are prepended the minus sign so we add some padding
            var saltPart = hash.Substring(0, SALT_LENGTH).Trim(SALT_PAD_CHAR);
            var decodedSaltString = Encoding.UTF8.GetString(Convert.FromBase64String(saltPart));

            var salt = IntParser.ParseOrNull(decodedSaltString);

            if (!salt.HasValue)
            {
                throw new ArgumentException("Hash is not correctly formatted");
            }

            var hashCheck = CreateHash(password, salt.Value);

            return hash == hashCheck;
        }

        public string CreateHash(string password)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentEmptyException(nameof(password));

            var salt = GenerateSalt();
            return CreateHash(password, salt);
        }

        private int GenerateSalt()
        {
            Byte[] _saltBytes = new Byte[4];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(_saltBytes);
            }

            return ((((int)_saltBytes[0]) << 24) + (((int)_saltBytes[1]) << 16) +
                (((int)_saltBytes[2]) << 8) + ((int)_saltBytes[3]));
        }

        private string CreateHash(string password, int salt)
        {
            // Create Byte array of password string
            Byte[] _secretBytes = UTF8Encoding.UTF8.GetBytes(password);

            // Create a new salt
            Byte[] _saltBytes = new Byte[4];
            _saltBytes[0] = (byte)(salt >> 24);
            _saltBytes[1] = (byte)(salt >> 16);
            _saltBytes[2] = (byte)(salt >> 8);
            _saltBytes[3] = (byte)(salt);

            // append the two arrays
            Byte[] toHash = new Byte[_secretBytes.Length + _saltBytes.Length];
            Array.Copy(_secretBytes, 0, toHash, 0, _secretBytes.Length);
            Array.Copy(_saltBytes, 0, toHash, _secretBytes.Length, _saltBytes.Length);

            SHA1 sha1 = SHA1.Create();
            Byte[] computedHash = sha1.ComputeHash(toHash);

            return EncodeSalt(salt) + Convert.ToBase64String(computedHash);
        }

        private string EncodeSalt(int salt)
        {
            var saltInBytes = UTF8Encoding.UTF8.GetBytes(salt.ToString());
            var encodedPaddedSalt = Convert.ToBase64String(saltInBytes).PadRight(SALT_LENGTH, SALT_PAD_CHAR);

            return encodedPaddedSalt;
        }
    }
}
