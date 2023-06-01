using AdminLte.Areas.Repositories;
using AdminLte.Areas.User.Models;
using AdminLte.Data;
using AdminLte.Data.Entities;
using AdminLte.Data.Enums;
using AdminLte.Models;
using AdminLte.Test.Faker;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using System.Net.Sockets;
using Twilio.Http;

namespace AdminLte.Test
{
    public class _WithdrawalRepositoryTests
    {
        public List<PaymentMethod> PaymentMethods = new();
        public List<Currency> Currencies = new();
        public List<ApplicationUser> User = new();
        public List<Wallet> Wallets = new();
        public List<FeeLimit> FeeLimits = new();
        public List<TransactionTypes> TransactionTypes = new();

        public Mock<DbSet<PaymentMethod>> MockDbSetPaymentMethod = null;
        public Mock<DbSet<Currency>> MockDbSetCurrencies = null;
        public Mock<DbSet<ApplicationUser>> MockDbSetUser = null;
        public Mock<DbSet<Wallet>> MockDbSetWallet = null;
        public Mock<DbSet<TransactionTypes>> MockDbSetTransactionType = null;
        public Mock<DbSet<FeeLimit>> MockDbSetFeeLimit = null;

        public Mock<ApplicationDbContext> context = null;
        public DbContextOptions<ApplicationDbContext> options = new DbContextOptions<ApplicationDbContext>();
        //public DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
        //.UseInMemoryDatabase(Guid.NewGuid().ToString())
        //.Options;
        public Mock<IMapper> IMapper = new Mock<IMapper>();

        public IWithdrawalRepository repository;
        public _WithdrawalRepositoryTests()
        {
            //Data Generator
            PaymentMethods = new PaymentMethodFactory().GeneratePaymentMethodsList();
            Currencies = new CurrencyFactory().GenerateCurrenciesList();
            User = new ApplicationUserFaker().GenerateApplicationUser();
            TransactionTypes = new TransactionTypeFaker().GenerateTransactionTypes();
            Wallets = new WalletFaker().GenerateWallets();
            FeeLimits = new FeeLimitFaker().GenerateListOfFeesLimits();


            //Set Db<PaymentMethod>
            var PaymentMethodData = PaymentMethods.AsQueryable();
            MockDbSetPaymentMethod = new Mock<DbSet<PaymentMethod>>();
            MockDbSetPaymentMethod.As<IQueryable<PaymentMethod>>().Setup(m => m.Provider).Returns(PaymentMethodData.Provider);
            MockDbSetPaymentMethod.As<IQueryable<PaymentMethod>>().Setup(m => m.ElementType).Returns(PaymentMethodData.ElementType);
            MockDbSetPaymentMethod.As<IQueryable<PaymentMethod>>().Setup(m => m.Expression).Returns(PaymentMethodData.Expression);
            MockDbSetPaymentMethod.As<IQueryable<PaymentMethod>>().Setup(m => m.GetEnumerator()).Returns(PaymentMethodData.GetEnumerator());

            //Set Db<Currency>
            var CurrenciesData = Currencies.AsQueryable();
            MockDbSetCurrencies = new Mock<DbSet<Currency>>();
            MockDbSetCurrencies.As<IQueryable<Currency>>().Setup(m => m.Provider).Returns(CurrenciesData.Provider);
            MockDbSetCurrencies.As<IQueryable<Currency>>().Setup(m => m.ElementType).Returns(CurrenciesData.ElementType);
            MockDbSetCurrencies.As<IQueryable<Currency>>().Setup(m => m.Expression).Returns(CurrenciesData.Expression);
            MockDbSetCurrencies.As<IQueryable<Currency>>().Setup(m => m.GetEnumerator()).Returns(CurrenciesData.GetEnumerator());

            //Set Db<ApplicationUser>
            var UserData = User.AsQueryable();
            MockDbSetUser = new Mock<DbSet<ApplicationUser>>();
            MockDbSetUser.As<IQueryable<ApplicationUser>>().Setup(m => m.Provider).Returns(UserData.Provider);
            MockDbSetUser.As<IQueryable<ApplicationUser>>().Setup(m => m.ElementType).Returns(UserData.ElementType);
            MockDbSetUser.As<IQueryable<ApplicationUser>>().Setup(m => m.Expression).Returns(UserData.Expression);
            MockDbSetUser.As<IQueryable<ApplicationUser>>().Setup(m => m.GetEnumerator()).Returns(UserData.GetEnumerator());

            //Set Db<Wallet>
            var WalletData = Wallets.AsQueryable();
            MockDbSetWallet = new Mock<DbSet<Wallet>>();
            MockDbSetWallet.As<IQueryable<Wallet>>().Setup(m => m.Provider).Returns(WalletData.Provider);
            MockDbSetWallet.As<IQueryable<Wallet>>().Setup(m => m.ElementType).Returns(WalletData.ElementType);
            MockDbSetWallet.As<IQueryable<Wallet>>().Setup(m => m.Expression).Returns(WalletData.Expression);
            MockDbSetWallet.As<IQueryable<Wallet>>().Setup(m => m.GetEnumerator()).Returns(WalletData.GetEnumerator());

            //Set Db<TransactionTypes>
            var TransactionTypesData = TransactionTypes.AsQueryable();
            MockDbSetTransactionType = new Mock<DbSet<TransactionTypes>>();
            MockDbSetTransactionType.As<IQueryable<TransactionTypes>>().Setup(m => m.Provider).Returns(TransactionTypesData.Provider);
            MockDbSetTransactionType.As<IQueryable<TransactionTypes>>().Setup(m => m.ElementType).Returns(TransactionTypesData.ElementType);
            MockDbSetTransactionType.As<IQueryable<TransactionTypes>>().Setup(m => m.Expression).Returns(TransactionTypesData.Expression);
            MockDbSetTransactionType.As<IQueryable<TransactionTypes>>().Setup(m => m.GetEnumerator()).Returns(TransactionTypesData.GetEnumerator());

            //Set Db<FeeLimit>
            var FeeLimitData = FeeLimits.AsQueryable();
            MockDbSetFeeLimit = new Mock<DbSet<FeeLimit>>();
            MockDbSetFeeLimit.As<IQueryable<FeeLimit>>().Setup(m => m.Provider).Returns(FeeLimitData.Provider);
            MockDbSetFeeLimit.As<IQueryable<FeeLimit>>().Setup(m => m.ElementType).Returns(FeeLimitData.ElementType);
            MockDbSetFeeLimit.As<IQueryable<FeeLimit>>().Setup(m => m.Expression).Returns(FeeLimitData.Expression);
            MockDbSetFeeLimit.As<IQueryable<FeeLimit>>().Setup(m => m.GetEnumerator()).Returns(FeeLimitData.GetEnumerator());


            //Set ApplicationDbContext
            context = new Mock<ApplicationDbContext>(options);

            //Set DbSets
            context.Setup(x => x.PaymentMethods).Returns(MockDbSetPaymentMethod.Object);
            context.Setup(x => x.Currencies).Returns(MockDbSetCurrencies.Object);
            context.Setup(x => x.Users).Returns(MockDbSetUser.Object);
            context.Setup(x => x.Wallets).Returns(MockDbSetWallet.Object);
            context.Setup(x => x.TransactionTypes).Returns(MockDbSetTransactionType.Object);
            context.Setup(x => x.FeesLimits).Returns(MockDbSetFeeLimit.Object);

            context.Setup(x => x.SaveChanges()).Returns(1);

            repository = new WithdrawalRepository(context.Object, IMapper.Object);
        }
        //[Fact]
        //public async Task GetPaymentMethods_GetAllMethods()
        //{
        //    var response =  repository.GetPaymentMethods().Result;
        //    Assert.NotEmpty(response);
        //    Assert.Equal(11, response.Count());
        //}

        public async Task GetCurrencies_GetAllCurrencies()
        {
            var response = repository.GetCurrencies().Result;
            Assert.NotEmpty(response);
            Assert.Equal(3, response.Count());
        }
        public async Task GetActiveCurrenciesForSelectedPaymentMethod_ReturnCurrenciesWhenUserSelectAPaymentMethod()
        {
            var response = await repository.GetActiveCurrenciesForSelectedPaymentMethod(2, "093732b4-e94b-40b0-a0ee-8dab1192aaba"); //bank
            Assert.NotNull(response);
        }
 
        public async Task TestEntityFrameworkCoreAsync_getAsyncList()
        {
            //Arrange
            var mock = new PaymentMethodFactory().GeneratePaymentMethodsList().BuildMock().BuildMockDbSet();
            var ApplicationContextMock = new Mock<ApplicationDbContext>();
            ApplicationContextMock.Setup(x => x.PaymentMethods).Returns(mock.Object);
            //Act
            var repo = new WithdrawalRepository(ApplicationContextMock.Object , IMapper.Object);
            var response = (await repo.GetPaymentMethods());
            //Assert
            Assert.NotEmpty(response);
            Assert.Equal(11, response.Count());
        }
        //Task<List<Country>> GetCountriesList();
        //Task<SuccessModel> GetFeesLimitAndCheckBalanceAvailability(string body, string userId);
        //Task<SuccessModel> CheckBalanceAvailability(WithdrawViewModel model, string userId);
        //Task<bool> SubmitWithdrawalAndCreateTransaction(WithdrawViewModel model, string userId);
        //Task<WithdrawViewModel> GetWithdrawalDetails();
        //Task<List<PayoutSettingViewModel>> GetPayoutSettings(string userId, int paymentMethodId);
    }
}