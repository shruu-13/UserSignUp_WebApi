using System;
using System.Security.Cryptography;

public static class KeyGenerator
{
	public static string GenerateSecureKey()
	{
		using (var rng = new RNGCryptoServiceProvider())
		{
			byte[] key = new byte[32]; // 32 bytes = 256 bits
			rng.GetBytes(key);
			return Convert.ToBase64String(key);
		}
	}
}
