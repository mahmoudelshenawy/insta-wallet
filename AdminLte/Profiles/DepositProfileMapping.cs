using AdminLte.Data.Entities;
using AdminLte.DataTableViewModels;
using AdminLte.Models;
using AutoMapper;

namespace AdminLte.Profiles
{
    public class DepositProfileMapping : Profile
    {
        public DepositProfileMapping() => CreateMap<Deposit, DepositsDataTable>()
                .ForMember(dest => dest.User, src => src.MapFrom(src => src.User.FirstName + " " + src.User.LastName))
                .ForMember(dest => dest.PaymentType, src => src.MapFrom(src => src.PaymentType.ToString()))
                .ForMember(dest => dest.Currency, src => src.MapFrom(src => src.Currency.Code))
                .ForMember(dest => dest.Status, src => src.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Fees, src => src.MapFrom(src => src.Amount + src.FixedFeeAmount + (src.Amount * (src.PercentFeeAmount ?? 0.00m) / 100)))
            .ForMember(dest => dest.TotalAmount, src => src.MapFrom(src => src.Amount + (src.Amount + src.FixedFeeAmount + (src.Amount * (src.PercentFeeAmount ?? 0.00m) / 100))))
            .ReverseMap()
                ;

    }




}
