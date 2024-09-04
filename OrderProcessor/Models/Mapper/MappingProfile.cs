using AutoMapper;

namespace OrderProcessor.Models.Mapper;

public class MappingProfile: Profile
{
    public MappingProfile()
    {
        CreateMap<Dto.Order, Entities.Order>().ReverseMap();
    }
}