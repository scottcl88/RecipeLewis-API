using AutoMapper;
using Database;
using RecipeLewis.Models;

namespace RecipeLewis.Business
{
    public class OrganizationProfile : Profile
    {
        public OrganizationProfile()
        {
            CreateMap<EntityData, EntityDataModel>();
            CreateMap<User, UserModel>();
        }
    }
}