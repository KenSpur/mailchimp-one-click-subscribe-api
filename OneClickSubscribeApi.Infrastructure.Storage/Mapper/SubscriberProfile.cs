using AutoMapper;
using OneClickSubscribeApi.Domain.Models;
using OneClickSubscribeApi.Infrastructure.Storage.Entities;

namespace OneClickSubscribeApi.Infrastructure.Storage.Mapper;

public class SubscriberProfile : Profile
{
    public SubscriberProfile()
    {
        CreateMap<Subscriber, SubscriberEntity>()
            .ForMember(dest => dest.RowKey, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
            .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.Details))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.Firstname, opt => opt.MapFrom(src => src.Firstname))
            .ForMember(dest => dest.Lastname, opt => opt.MapFrom(src => src.Lastname));

        CreateMap<SubscriberEntity, Subscriber>()
            .ConstructUsing(e => new Subscriber(e.Firstname, e.Lastname, e.Email, e.Type, e.State, e.Details));
    }
}