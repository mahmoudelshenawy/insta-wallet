using AdminLte.Areas.User.Models;
using AdminLte.Data.Entities;
using AdminLte.Data.Enums;
using AdminLte.DataTableViewModels;
using AutoMapper;
using System.Text.Json;

namespace AdminLte.Profiles
{
    public class WithdrawalModelMapping : Profile
    {
        JsonSerializerOptions options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition =
                System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
        public WithdrawalModelMapping() => CreateMap<Withdrawal, WithdrawViewModel>()
            .ForMember(dest => dest.CurrencyId, source => source.MapFrom(src => src.CurrencyId))
            .ForMember(dest => dest.PaymentMethodId, source => source.MapFrom(src => src.PaymentMethodId))
            .ForMember(dest => dest.Amount, source => source.MapFrom(src => src.Amount))
            .ForMember(dest => dest.TotalFees, source => source.MapFrom(src => src.TotalFees))
            .ForMember(dest => dest.PercentFeeAmount, source => source.MapFrom(src => src.PercentFeeAmount))
            .ForMember(dest => dest.FixedFeeAmount, source => source.MapFrom(src => src.FixedFeeAmount))
            .ForMember(dest => dest.TotalAmount, source => source.MapFrom(src => src.Amount - src.TotalFees))
            .ForPath(dest => dest.PaymentMethod, source => source.MapFrom(src => src.PaymentMethod.Name))
            .ForPath(dest => dest.Currency, source => source.MapFrom(src => src.Currency.Code))
            .ForPath(dest => dest.PayoutSetting.PayoutSettingId, source => source.MapFrom(src => src.PayoutSettingId))

            .ForPath(dest => dest.PayoutSetting.CashSetting, source => source.MapFrom(src =>
            JsonSerializer.Deserialize<CashSetting>(src.WithdrawalDetail.CashSetting,options)))
            .ForPath(dest => dest.PayoutSetting.BankSetting, source => source.MapFrom(src => 
            JsonSerializer.Deserialize<BankSetting>(src.WithdrawalDetail.BankSetting,options)))
            .ForPath(dest => dest.PayoutSetting.PayoneerSetting, source => source.MapFrom(src => 
            JsonSerializer.Deserialize<PayoneerSetting>(src.WithdrawalDetail.PayoneerSetting,options)))
             .ForPath(dest => dest.PayoutSetting.PaypalSetting, source => source.MapFrom(src => 
             JsonSerializer.Deserialize<PaypalSetting>(src.WithdrawalDetail.PaypalSetting,options)))

            .ForPath(dest => dest.PayoutSetting.VodafoneCashSetting, source => source.MapFrom(src => src.PaymentMethod.Name == WithdrawalPaymentMethodsEnum.VodafoneCash.ToString() ? JsonSerializer.Deserialize<WalletSetting>(src.WithdrawalDetail.WalletSetting,
                options) : null))

             .ForPath(dest => dest.PayoutSetting.PerfectMoneySetting, source => source.MapFrom(src => src.PaymentMethod.Name == WithdrawalPaymentMethodsEnum.PerfectMoney.ToString() ? JsonSerializer.Deserialize<WalletSetting>(src.WithdrawalDetail.WalletSetting,
                options) : null))
             .ForPath(dest => dest.PayoutSetting.OrangeMoneySetting, source => source.MapFrom(src => src.PaymentMethod.Name == WithdrawalPaymentMethodsEnum.OrangeMoney.ToString() ? JsonSerializer.Deserialize<WalletSetting>(src.WithdrawalDetail.WalletSetting,
                options) : null))

            .ForPath(dest => dest.PayoutSetting.CryptoWalletSetting, source => source.MapFrom(src => src.PaymentMethod.Name == WithdrawalPaymentMethodsEnum.Usdt.ToString() ? JsonSerializer.Deserialize<CryptoWalletSetting>(src.WithdrawalDetail.WalletSetting,
               options) : null))
             .ForPath(dest => dest.PayoutSetting.CryptoWalletSetting, source => source.MapFrom(src => src.PaymentMethod.Name == WithdrawalPaymentMethodsEnum.Bitcoin.ToString() ? JsonSerializer.Deserialize<CryptoWalletSetting>(src.WithdrawalDetail.WalletSetting,
                options) : null))
            .ReverseMap();
    }
}
