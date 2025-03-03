namespace Infrastructure.Extensions
{
    public class OTPGenerator
    {
        public static string GenerateOTP()
        {
            var random = new Random();
            return random.Next(0, 1000000).ToString("D6");
        }
    }
}
