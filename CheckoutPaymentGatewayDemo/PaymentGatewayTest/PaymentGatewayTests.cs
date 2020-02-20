using AutoFixture;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using PaymentGateway;
using PaymentGatewayDbContext;
using PaymentGateWayModels;
using SharedResource;
using System;
using System.Linq;

namespace PaymentGatewayTest
{
    [TestFixture]
    public class Tests
    {
        public DbContextOptionsBuilder<PaymentGatewayDbContext.PaymentGatewayDbContext> GetInMemoryDatabaseOption()
        {
            return new DbContextOptionsBuilder<PaymentGatewayDbContext.PaymentGatewayDbContext>()
                  .UseInMemoryDatabase(databaseName: "PaymentGatewayDbTest");
        }
        [SetUp]
        public void Setup()
        {           
            using (var context = new PaymentGatewayDbContext.PaymentGatewayDbContext(GetInMemoryDatabaseOption().Options))
            {
                var fixture = new Fixture();
                fixture.Customize<Merchant>(m => m.With(m => m.IsActive, true));
                var merchant = fixture.CreateMany<Merchant>(9).ToList();
                merchant.Add(new Merchant {
                    Email = "aghoghobernard@gmail.com",
                    FirstName = "Aghogho",
                    LastName = "Bernard",
                    FullName = "Aghogho Bernard",
                    IsActive = true,
                    Username = "teghoz",
                    Id = 1024567,
                });

                merchant.ForEach(m =>
                {
                    context.tblMerchant.Add(m);
                    context.SaveChanges();
                });
            }
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
        [Test]
        public void TestExpiryUtility()
        {
            var testString = "10/21";
            var expiry = Utilities.GetCardExpiry(testString);
            Assert.AreEqual(expiry.month, 10);
            Assert.AreEqual(expiry.year, 2000 + 21);

            var testStringWithoutSlash = "1021";
            var expiryWithoutSlah = Utilities.GetCardExpiry(testStringWithoutSlash);
            Assert.AreEqual(expiryWithoutSlah.month, 0);
            Assert.AreEqual(expiryWithoutSlah.year, 0);
        }
        [Test]
        public void TestWhiteSpaceRemoval()
        {
            var testString = "4111 1111 1111 1111";
            var textWithoutString = Utilities.RemoveWhitespaceAndNumber(testString);
            Assert.AreEqual(textWithoutString, "4111111111111111");

            var testAndNumberString = "4111 1111 1111 1111 ABC `";
            var textWithoutStringOrNumbersString = Utilities.RemoveWhitespaceAndNumber(testAndNumberString);
            Assert.AreEqual(textWithoutStringOrNumbersString, "4111111111111111");
        }
        [Test]
        public void RepositoryCountWorking()
        {          
            var options = new DbContextOptionsBuilder<PaymentGatewayDbContext.PaymentGatewayDbContext>()
                  .UseInMemoryDatabase(databaseName: "PaymentGatewayDbTest")
                  .Options;

            using (var context = new PaymentGatewayDbContext.PaymentGatewayDbContext(options))
            {
                var unitOfWork = new UnitOfWork(context);
                Assert.AreEqual(unitOfWork.MerchantRepository.Count(), 10);
            }              
        }
        [Test]
        public void RepositoryGetWorking()
        {
            var options = new DbContextOptionsBuilder<PaymentGatewayDbContext.PaymentGatewayDbContext>()
                  .UseInMemoryDatabase(databaseName: "PaymentGatewayDbTest")
                  .Options;

            using (var context = new PaymentGatewayDbContext.PaymentGatewayDbContext(options))
            {
                var unitOfWork = new UnitOfWork(context);
                Assert.AreEqual(unitOfWork.MerchantRepository.Get(x => x.Id == 1024567).FirstOrDefault().Email, "aghoghobernard@gmail.com");
                Assert.AreEqual(unitOfWork.MerchantRepository.GetByID(1024567).Email, "aghoghobernard@gmail.com");
            }
        }

        [Test]
        public void RepositoryAddWorking()
        {
            using (var context = new PaymentGatewayDbContext.PaymentGatewayDbContext(GetInMemoryDatabaseOption().Options))
            {
                var unitOfWork = new UnitOfWork(context);
                var merchant = new Merchant
                {
                    Email = "mabelbernard@gmail.com",
                    FirstName = "Mabel",
                    LastName = "Bernard",
                    FullName = "Mabel Bernard",
                    IsActive = true,
                    Username = "mabel",
                    Id = 5024567,
                };
                unitOfWork.MerchantRepository.Insert(merchant);
                unitOfWork.Save();
                Assert.AreEqual(unitOfWork.MerchantRepository.Count(), 11);
                Assert.AreEqual(unitOfWork.MerchantRepository.Get(x => x.Id == 5024567).FirstOrDefault().Email, "mabelbernard@gmail.com");
            }
        }
        [Test]
        public void RepositoryUpdateWorking()
        {
            using (var context = new PaymentGatewayDbContext.PaymentGatewayDbContext(GetInMemoryDatabaseOption().Options))
            {
                var unitOfWork = new UnitOfWork(context);
                var merchant = new Merchant
                {
                    Email = "mabelbernard@gmail.com",
                    FirstName = "Mabel",
                    LastName = "Bernard",
                    FullName = "Mabel Bernard",
                    IsActive = true,
                    Username = "mabel",
                    Id = 5024567,
                };
                unitOfWork.MerchantRepository.Insert(merchant);
                unitOfWork.Save();

                Assert.AreEqual(unitOfWork.MerchantRepository.Count(), 11);
                Assert.AreEqual(unitOfWork.MerchantRepository.Get(x => x.Id == 5024567).FirstOrDefault().Email, "mabelbernard@gmail.com");

                merchant.IsActive = false;
                unitOfWork.MerchantRepository.Update(merchant);
                unitOfWork.Save();

                Assert.AreEqual(unitOfWork.MerchantRepository.Get(x => x.Id == 5024567).FirstOrDefault().IsActive, false);
            }
        }

        [Test]
        public void RepositoryDeleteWorking()
        {
            using (var context = new PaymentGatewayDbContext.PaymentGatewayDbContext(GetInMemoryDatabaseOption().Options))
            {
                var unitOfWork = new UnitOfWork(context);
                var merchant = new Merchant
                {
                    Email = "mabelbernard@gmail.com",
                    FirstName = "Mabel",
                    LastName = "Bernard",
                    FullName = "Mabel Bernard",
                    IsActive = true,
                    Username = "mabel",
                    Id = 5024567,
                };
                unitOfWork.MerchantRepository.Insert(merchant);
                unitOfWork.Save();

                Assert.AreEqual(unitOfWork.MerchantRepository.Count(), 11);
                Assert.AreEqual(unitOfWork.MerchantRepository.Get(x => x.Id == 5024567).FirstOrDefault().Email, "mabelbernard@gmail.com");

                unitOfWork.MerchantRepository.Delete(merchant);
                unitOfWork.Save();

                Assert.AreEqual(unitOfWork.MerchantRepository.Count(), 10);
                Assert.IsNull(unitOfWork.MerchantRepository.Get(x => x.Id == 5024567).FirstOrDefault());

                var unitOfWork2 = new UnitOfWork(context);
                var merchant2 = new Merchant
                {
                    Email = "mabelbernard@gmail.com",
                    FirstName = "Mabel",
                    LastName = "Bernard",
                    FullName = "Mabel Bernard",
                    IsActive = true,
                    Username = "mabel",
                    Id = 5024567,
                };
                unitOfWork.MerchantRepository.Insert(merchant2);
                unitOfWork.Save();

                Assert.AreEqual(unitOfWork2.MerchantRepository.Count(), 11);
                Assert.AreEqual(unitOfWork2.MerchantRepository.Get(x => x.Id == 5024567).FirstOrDefault().Email, "mabelbernard@gmail.com");

                unitOfWork2.MerchantRepository.Delete(merchant2);
                unitOfWork2.Save();

                Assert.AreEqual(unitOfWork2.MerchantRepository.Count(), 10);
                Assert.IsNull(unitOfWork2.MerchantRepository.Get(x => x.Id == 5024567).FirstOrDefault());
            }
        }

        [TearDown]
        public void Cleanup()
        {
            using (var context = new PaymentGatewayDbContext.PaymentGatewayDbContext(GetInMemoryDatabaseOption().Options))
            {
                context.Database.EnsureDeleted();
            }
        }
    }
}