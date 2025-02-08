using Application.DTO;
using AutoMapper;
using Domain.Entities;


namespace Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Post, PostDto>().ReverseMap();
        }
    }
}
