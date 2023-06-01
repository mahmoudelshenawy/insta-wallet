using AdminLte.Areas.User.Models;
using AdminLte.Data.Entities;
using AdminLte.Data.Enums;
using AutoMapper;
using System.Text.Json;

namespace AdminLte.Profiles
{
    public class PayoutSettingViewModelMapping : Profile
    {
        JsonSerializerOptions options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition =
                System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        public PayoutSettingViewModelMapping() => CreateMap<PayoutSetting, PayoutSettingViewModel>()
            .ForMember(dest => dest.PayoutSettingId, source => source.MapFrom(src => src.Id))
           .ForMember(dest => dest.BankSetting, source => source.MapFrom(src => JsonSerializer.Deserialize<BankSetting>(src.BankSetting, options)))
           .ForMember(dest => dest.CashSetting, source => source.MapFrom(src => JsonSerializer.Deserialize<CashSetting>(src.CashSetting, options)))
           .ForMember(dest => dest.CryptoWalletSetting, source => source.MapFrom(src => JsonSerializer.Deserialize<WalletSetting>(src.WalletSetting, options)))
           .ForMember(dest => dest.OrangeMoneySetting, source => source.MapFrom(src => JsonSerializer.Deserialize<WalletSetting>(src.WalletSetting, options)))
           .ForMember(dest => dest.PayeerWalletSetting, source => source.MapFrom(src => JsonSerializer.Deserialize<WalletSetting>(src.WalletSetting, options)))
           .ForMember(dest => dest.PayoneerSetting, source => source.MapFrom(src => JsonSerializer.Deserialize<PayoneerSetting>(src.PayoneerSetting, options)))
           .ForMember(dest => dest.PaypalSetting, source => source.MapFrom(src => JsonSerializer.Deserialize<PaypalSetting>(src.PaypalSetting, options)))
           .ForMember(dest => dest.PerfectMoneySetting, source => source.MapFrom(src => JsonSerializer.Deserialize<WalletSetting>(src.WalletSetting, options)))
           .ForMember(dest => dest.UsdtSetting, source => source.MapFrom(src => JsonSerializer.Deserialize<WalletSetting>(src.WalletSetting, options)))
           .ForMember(dest => dest.VodafoneCashSetting, source => source.MapFrom(src => JsonSerializer.Deserialize<WalletSetting>(src.WalletSetting, options)))
            ;

    }
}
