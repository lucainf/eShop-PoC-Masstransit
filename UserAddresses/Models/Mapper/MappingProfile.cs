using AutoMapper;

namespace UserAddresses.Models.Mapper;

public class MappingProfile: Profile
{
    public MappingProfile()
    {
        CreateMap<Entities.Address, Dto.Address>().ReverseMap();
        CreateMap<Entities.Address, Request.Address>().ReverseMap();
    }
}