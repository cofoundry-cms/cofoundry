// Copyright (c) 2016, Taylor Hornby
// All rights reserved.
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
// 1. Redistributions of source code must retain the above copyright notice, this
// list of conditions and the following disclaimer.
// 
// 2. Redistributions in binary form must reproduce the above copyright notice,
// this list of conditions and the following disclaimer in the documentation and/or
// other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
// ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

// Adapted from methods and source described at https://crackstation.net/hashing-security.htm


using System;
using System.Text;
using System.Security.Cryptography;
using Cofoundry.Core;

namespace Defuse
{
    class InvalidHashException : Exception
    {
        public InvalidHashException() { }
        public InvalidHashException(string message)
            : base(message) { }
        public InvalidHashException(string message, Exception inner)
            : base(message, inner) { }
    }

    class CannotPerformOperationException : Exception
    {
        public CannotPerformOperationException() { }
        public CannotPerformOperationException(string message)
            : base(message) { }
        public CannotPerformOperationException(string message, Exception inner)
            : base(message, inner) { }
    }

    public class PasswordCryptographyV2
    {
        // These constants may be changed without breaking existing hashes.
        public const int SALT_BYTES = 24;
        public const int SALT_BASE64_LENGTH = 32; // manually calc'ed

        public const int HASH_BYTES = 18;
        public const int PBKDF2_ITERATIONS = 64000;

        // These constants define the encoding and may not be changed.
        public const int HASH_SECTIONS = 5;
        public const int HASH_ALGORITHM_INDEX = 0;
        public const int ITERATION_INDEX = 1;
        public const int HASH_SIZE_INDEX = 2;
        public const int SALT_INDEX = 3;
        public const int PBKDF2_INDEX = 4;

        #region adapted methods

        // Adapted because we need to store the salt separately 

        public static string GenerateSalt()
        {
            // Generate a random salt
            byte[] salt = new byte[SALT_BYTES];
            try
            {
                using (var generator = RandomNumberGenerator.Create())
                {
                    generator.GetBytes(salt);
                }
            }
            catch (CryptographicException ex)
            {
                throw new CannotPerformOperationException(
                    "Random number generator not available.",
                    ex
                );
            }
            catch (ArgumentNullException ex)
            {
                throw new CannotPerformOperationException(
                    "Invalid argument given to random number generator.",
                    ex
                );
            }

            return Convert.ToBase64String(salt);
        }

        public static string CreateHash(string password)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (string.IsNullOrEmpty(password)) throw new ArgumentEmptyException(nameof(password));

            var salt = GenerateSalt();

            var saltBytes = Convert.FromBase64String(salt);
            byte[] hash = PBKDF2(password, saltBytes, PBKDF2_ITERATIONS, HASH_BYTES);

            return salt + Convert.ToBase64String(hash);
        }

        public static bool VerifyPassword(string password, string hash)
        {
            if (hash == null) throw new ArgumentNullException(nameof(hash));
            if (string.IsNullOrWhiteSpace(hash)) throw new ArgumentEmptyException(nameof(hash));
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (string.IsNullOrEmpty(password)) throw new ArgumentEmptyException(nameof(password));

            if (hash.Length <= SALT_BASE64_LENGTH)
            {
                throw new ArgumentException($"The length of {nameof(hash)} cannot be shorter than the salt length.", nameof(hash));
            }

            string salt = hash.Substring(0, SALT_BASE64_LENGTH);
            string hashWithoutSalt = hash.Substring(SALT_BASE64_LENGTH);
            byte[] saltBytes = Convert.FromBase64String(salt);
            byte[] hashBytes = Convert.FromBase64String(hashWithoutSalt);

            byte[] testHash = PBKDF2(password, saltBytes, PBKDF2_ITERATIONS, hashBytes.Length);
            return SlowEquals(hashBytes, testHash);
        }

        #endregion

        //public static string CreateHash(string password)
        //{
        //    // Generate a random salt
        //    byte[] salt = new byte[SALT_BYTES];
        //    try
        //    {
        //        using (RNGCryptoServiceProvider csprng = new RNGCryptoServiceProvider())
        //        {
        //            csprng.GetBytes(salt);
        //        }
        //    }
        //    catch (CryptographicException ex)
        //    {
        //        throw new CannotPerformOperationException(
        //            "Random number generator not available.",
        //            ex
        //        );
        //    }
        //    catch (ArgumentNullException ex)
        //    {
        //        throw new CannotPerformOperationException(
        //            "Invalid argument given to random number generator.",
        //            ex
        //        );
        //    }

        //    byte[] hash = PBKDF2(password, salt, PBKDF2_ITERATIONS, HASH_BYTES);

        //    // format: algorithm:iterations:hashSize:salt:hash
        //    String parts = "sha1:" +
        //        PBKDF2_ITERATIONS +
        //        ":" +
        //        hash.Length +
        //        ":" +
        //        Convert.ToBase64String(salt) +
        //        ":" +
        //        Convert.ToBase64String(hash);
        //    return parts;
        //}

        //public static bool VerifyPassword(string password, string goodHash)
        //{
        //    char[] delimiter = { ':' };
        //    string[] split = goodHash.Split(delimiter);

        //    if (split.Length != HASH_SECTIONS)
        //    {
        //        throw new InvalidHashException(
        //            "Fields are missing from the password hash."
        //        );
        //    }

        //    // We only support SHA1 with C#.
        //    if (split[HASH_ALGORITHM_INDEX] != "sha1")
        //    {
        //        throw new CannotPerformOperationException(
        //            "Unsupported hash type."
        //        );
        //    }

        //    int iterations = 0;
        //    try
        //    {
        //        iterations = Int32.Parse(split[ITERATION_INDEX]);
        //    }
        //    catch (ArgumentNullException ex)
        //    {
        //        throw new CannotPerformOperationException(
        //            "Invalid argument given to Int32.Parse",
        //            ex
        //        );
        //    }
        //    catch (FormatException ex)
        //    {
        //        throw new InvalidHashException(
        //            "Could not parse the iteration count as an integer.",
        //            ex
        //        );
        //    }
        //    catch (OverflowException ex)
        //    {
        //        throw new InvalidHashException(
        //            "The iteration count is too large to be represented.",
        //            ex
        //        );
        //    }

        //    if (iterations < 1)
        //    {
        //        throw new InvalidHashException(
        //            "Invalid number of iterations. Must be >= 1."
        //        );
        //    }

        //    byte[] salt = null;
        //    try
        //    {
        //        salt = Convert.FromBase64String(split[SALT_INDEX]);
        //    }
        //    catch (ArgumentNullException ex)
        //    {
        //        throw new CannotPerformOperationException(
        //            "Invalid argument given to Convert.FromBase64String",
        //            ex
        //        );
        //    }
        //    catch (FormatException ex)
        //    {
        //        throw new InvalidHashException(
        //            "Base64 decoding of salt failed.",
        //            ex
        //        );
        //    }

        //    byte[] hash = null;
        //    try
        //    {
        //        hash = Convert.FromBase64String(split[PBKDF2_INDEX]);
        //    }
        //    catch (ArgumentNullException ex)
        //    {
        //        throw new CannotPerformOperationException(
        //            "Invalid argument given to Convert.FromBase64String",
        //            ex
        //        );
        //    }
        //    catch (FormatException ex)
        //    {
        //        throw new InvalidHashException(
        //            "Base64 decoding of pbkdf2 output failed.",
        //            ex
        //        );
        //    }

        //    int storedHashSize = 0;
        //    try
        //    {
        //        storedHashSize = Int32.Parse(split[HASH_SIZE_INDEX]);
        //    }
        //    catch (ArgumentNullException ex)
        //    {
        //        throw new CannotPerformOperationException(
        //            "Invalid argument given to Int32.Parse",
        //            ex
        //        );
        //    }
        //    catch (FormatException ex)
        //    {
        //        throw new InvalidHashException(
        //            "Could not parse the hash size as an integer.",
        //            ex
        //        );
        //    }
        //    catch (OverflowException ex)
        //    {
        //        throw new InvalidHashException(
        //            "The hash size is too large to be represented.",
        //            ex
        //        );
        //    }

        //    if (storedHashSize != hash.Length)
        //    {
        //        throw new InvalidHashException(
        //            "Hash length doesn't match stored hash length."
        //        );
        //    }

        //    byte[] testHash = PBKDF2(password, salt, iterations, hash.Length);
        //    return SlowEquals(hash, testHash);
        //}

        private static bool SlowEquals(byte[] a, byte[] b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }
            return diff == 0;
        }

        private static byte[] PBKDF2(string password, byte[] salt, int iterations, int outputBytes)
        {
            using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt))
            {
                pbkdf2.IterationCount = iterations;
                return pbkdf2.GetBytes(outputBytes);
            }
        }
    }
}

