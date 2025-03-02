namespace Infrastructure.Extensions
{
    public class OTPGenerator
    {
        public static string GenerateOTP()
        {
            var generator = new Random();
            return generator.Next(0, 1000000).ToString("D6");
        }
    }
}
