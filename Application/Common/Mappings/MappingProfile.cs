﻿using Application.DTO;
using AutoMapper;
using Domain.Entities;


namespace Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        // Configure object mappings o day
        // Them CreateMap<[From], [To]>.ReverseMap() vao parameterless constructor
        // Dung ReverseMap() de co 2-way mapping

        public MappingProfile()
        {
            CreateMap<Post, PostDto>().ReverseMap();
            CreateMap<Post, GetPostDto>().ReverseMap();
            CreateMap<Post, CreatePostDto>().ReverseMap();
            CreateMap<User, UserProfileDto>().ReverseMap();
            CreateMap<User, UserProfileDto>().ReverseMap();
            CreateMap<Ticket, TicketDto>().ReverseMap();
            CreateMap<Ticket, CreateTicketDto>().ReverseMap();
            CreateMap<Ticket, UpdateTicketDto>().ReverseMap();
        }
    }
}
