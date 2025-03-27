using Infrastructure.Extensions;

namespace InfrastructureLayerUnitTests
{
    [TestFixture]
    public class OTPGeneratorTests
    {
        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public void GenerateOTP_ShouldReturnValidOTP()
        {
            // Act
            var otp = OTPGenerator.GenerateOTP();

            // Assert
            Assert.That(otp, Is.Not.Null.And.Not.Empty);

            Assert.Multiple(() =>
            {
                Assert.That(otp, Has.Length.EqualTo(6));
                Assert.That(otp, Is.TypeOf<string>());
            });
        }

        [TearDown]
        public void TearDown()
        {
            
        }   
    }
}
