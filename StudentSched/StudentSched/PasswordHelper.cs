using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace StudentSched
{
	public static class PasswordHelper
	{
		private const int SaltSize = 16;     // 16 bytes
		private const int KeySize = 32;      // 32 bytes (256-bit key)
		private const int Iterations = 100_000;

		// Create password hash and salt
		public static void CreatePasswordHash(
			string password,
			out string passwordHash,
			out string salt)
		{
			byte[] saltBytes = RandomNumberGenerator.GetBytes(SaltSize);

			byte[] hashBytes = Rfc2898DeriveBytes.Pbkdf2(
				password,
				saltBytes,
				Iterations,
				HashAlgorithmName.SHA256,
				KeySize
			);

			passwordHash = Convert.ToBase64String(hashBytes);
			salt = Convert.ToBase64String(saltBytes);
		}

		public static bool VerifyPassword(
			string password,
			string storedHash,
			string storedSalt)
		{
			if (string.IsNullOrEmpty(password) ||
				string.IsNullOrEmpty(storedHash) ||
				string.IsNullOrEmpty(storedSalt))
			{
				return false;
			}

			byte[] saltBytes = Convert.FromBase64String(storedSalt);
			byte[] storedHashBytes = Convert.FromBase64String(storedHash);

			byte[] computedHash = Rfc2898DeriveBytes.Pbkdf2(
				password,
				saltBytes,
				Iterations,
				HashAlgorithmName.SHA256,
				KeySize
			);

			return CryptographicOperations.FixedTimeEquals(
				computedHash,
				storedHashBytes
			);
		}
	}
}
