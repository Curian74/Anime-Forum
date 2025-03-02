using System.Security.Cryptography;

namespace Application.Common.OTPGenerator
{
    public class OTPGenerator
    {
        public static string GenerateOTP()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] randomBytes = new byte[4];
                rng.GetBytes(randomBytes);
                int otp = BitConverter.ToInt32(randomBytes, 0) % 1000000;
                otp = (otp < 0) ? -otp : otp; //Eliminate minus values
                return otp.ToString("D6"); // 6-digit format
            }
        }
    }
}
