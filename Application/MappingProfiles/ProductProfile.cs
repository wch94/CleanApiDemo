using Application.DTOs;
using AutoMapper;
using Core.Entities;

namespace Application.MappingProfiles;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductDto>().ReverseMap();
    }
}
