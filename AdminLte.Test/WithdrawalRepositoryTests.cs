using AdminLte.Areas.Repositories;
using AdminLte.Data;
using AdminLte.Data.Entities;
using AdminLte.Models;
using AdminLte.Test.Faker;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminLte.Test
{
    public class WithdrawalRepositoryTests
    {
        public Mock<IMapper> IMapper = new Mock<IMapper>();
        public Mock<ApplicationDbContext> Context = new Mock<ApplicationDbContext>();

        public Mock<DbSet<PaymentMethod>> MockDbSetPaymentMethod = null;
        public Mock<DbSet<Currency>> MockDbSetCurrencies = null;
        public Mock<DbSet<ApplicationUser>> MockDbSetUser = null;
        public Mock<DbSet<Wallet>> MockDbSetWallet = null;
        public Mock<DbSet<TransactionTypes>> MockDbSetTransactionType = null;
        public Mock<DbSet<FeeLimit>> MockDbSetFeeLimit = null;

        public IWithdrawalRepository repository;

        public WithdrawalRepositoryTests()
        {
            MockDbSetPaymentMethod = new PaymentMethodFactory().GeneratePaymentMethodsList().BuildMock().BuildMockDbSet();
            MockDbSetCurrencies = new CurrencyFactory().GenerateCurrenciesList().BuildMock().BuildMockDbSet();
            MockDbSetTransactionType = new TransactionTypeFaker().GenerateTransactionTypes().BuildMock().BuildMockDbSet();
            MockDbSetUser = new ApplicationUserFaker().GenerateApplicationUser().BuildMock().BuildMockDbSet();
            MockDbSetWallet = new WalletFaker().GenerateWallets().BuildMock().BuildMockDbSet();
            MockDbSetFeeLimit = new FeeLimitFaker().GenerateListOfFeesLimits().BuildMock().BuildMockDbSet();


            Context.Setup(x => x.PaymentMethods).Returns(MockDbSetPaymentMethod.Object);
            Context.Setup(x => x.Currencies).Returns(MockDbSetCurrencies.Object);
            Context.Setup(x => x.TransactionTypes).Returns(MockDbSetTransactionType.Object);
            Context.Setup(x => x.Users).Returns(MockDbSetUser.Object);
            Context.Setup(x => x.Wallets).Returns(MockDbSetWallet.Object);
            Context.Setup(x => x.FeesLimits).Returns(MockDbSetFeeLimit.Object);

            repository = new WithdrawalRepository(Context.Object, IMapper.Object);
        }
        [Fact]
        public async Task GetCurrencies_GetAllCurrencies()
        {
            // Arrange

            //Act
            var response = await repository.GetCurrencies();

            // Assert
            Assert.NotEmpty(response);
            Assert.Equal(3, response.Count());
        }
        [Fact]
        public async Task GetActiveCurrenciesForSelectedPaymentMethod_ReturnCurrenciesWhenUserSelectAPaymentMethod()
        {
            //Arrange
            //Act
            var response = await repository.GetActiveCurrenciesForSelectedPaymentMethod(2, "093732b4-e94b-40b0-a0ee-8dab1192aaba"); //bank
            //Assert
            Assert.NotEmpty(response);
        }
        [Fact]
        public async Task GetPaymentMethods_GetAllMethods()
        {
            //Arrange
            //Act
            var response = await repository.GetPaymentMethods();
            //Assert
            Assert.NotEmpty(response);
            Assert.Equal(11, response.Count());
        }
    }
}
