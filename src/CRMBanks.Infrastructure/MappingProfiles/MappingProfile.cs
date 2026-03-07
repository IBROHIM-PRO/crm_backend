using AutoMapper;
using CRMBanks.Core.Dtos;
using CRMBanks.Core.Entities;

namespace CRMBanks.Infrastructure.MappingProfiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(d => d.RegionIds, o => o.MapFrom(s => s.Regions.Select(r => r.Id).ToList()))
            .ForMember(d => d.Regions, o => o.MapFrom(s => s.Regions));
        
        CreateMap<UserDto, User>()
            .ForMember(d => d.Regions, o => o.Ignore()); // Regions are handled in service

        CreateMap<Bank, BankDto>().ReverseMap();

        CreateMap<Region, RegionDto>().ReverseMap();

        CreateMap<Credit, CreditDto>().ReverseMap();
        CreateMap<DepositDto, Deposit>().ReverseMap();

        CreateMap<TypeCredit, TypeCreditDto>().ReverseMap();
        CreateMap<TypeDeposit, TypeDepositDto>().ReverseMap();

        CreateMap<RequestDto, Request>().ReverseMap();
        CreateMap<Role, RoleDto>().ReverseMap();
    }
}
