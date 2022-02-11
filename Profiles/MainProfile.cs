using AutoMapper;
using Microsoft.AspNetCore.Identity;
using ReactProjectsAuthApi.Models;

namespace ReactProjectsAuthApi.Profiles
{
    public class MainProfile : Profile
    {
        public MainProfile()
        {
            CreateMap<RegisterModel, IdentityUser>();
            CreateMap<LoginModel, IdentityUser>();
            CreateMap<MessageDTO, MessageModel>().ForMember(dest=>dest.TimeOfCreation,src=>src.MapFrom(opts=>DateTime.UtcNow));
            CreateMap<MessageModel, ChatMessageModel>();
        }
    }
}
