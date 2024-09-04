using AutoMapper;

namespace Products.Models.Mapper;

public class MappingProfile: Profile
{
    public MappingProfile()
    {
        CreateMap<Entities.Product, Dto.Product>().ReverseMap();
        CreateMap<Entities.Category, Dto.Category>().ReverseMap();
        
    }
}