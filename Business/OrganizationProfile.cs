using AutoMapper;
using Database;
using RecipeLewis.Models;
using RecipeLewis.Models.Requests;

namespace RecipeLewis.Business
{
    public class OrganizationProfile : Profile
    {
        public OrganizationProfile()
        {
            CreateMap<EntityData, EntityDataModel>();
            CreateMap<EntityDataUser, EntityDataUserModel>();
            CreateMap<User, UserModel>();
            CreateMap<RegisterRequest, User>();
            CreateMap<RefreshToken, RefreshTokenModel>();
        }
    }
}