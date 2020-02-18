using NUnit.Framework;
using SharedResource;

namespace PaymentGatewayTest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
        [Test]
        public void TestEncryptionAndDecryption()
        {
            string testString = "CHECKOUT";
            string testKey = "1@#$%";
            var encrytedString = testString.EncryptString(testKey);
            Assert.AreEqual(encrytedString.DecryptString(testKey), testString);
        }
        [Test]
        public void TestTokenGeneration()
        {
            var token = Utilities.GenerateToken(15);
            Assert.AreEqual(token.Length, 15);
        }
    }
}