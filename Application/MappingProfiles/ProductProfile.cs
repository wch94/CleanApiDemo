using Application.DTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.MappingProfiles;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductDto>();

        CreateMap<ProductDto, Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
