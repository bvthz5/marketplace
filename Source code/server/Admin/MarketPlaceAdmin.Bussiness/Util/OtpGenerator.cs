using System.Security.Cryptography;

namespace MarketPlaceAdmin.Bussiness.Util
{
    public static class OtpGenerator
    {
        public static string GenerateOTP(int length)
        {
            const string allowedChars = "1234567890";

            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] randomBytes = new byte[length];
                rng.GetBytes(randomBytes);

                char[] otpChars = new char[length];
                int maxIndex = allowedChars.Length;

                for (int i = 0; i < length; i++)
                {
                    int randomIndex = randomBytes[i] % maxIndex;
                    otpChars[i] = allowedChars[randomIndex];
                }

                return new string(otpChars);
            }
        }
    }
}
