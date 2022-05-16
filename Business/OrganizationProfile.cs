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
            CreateMap<UserModel, User>();
            CreateMap<RegisterRequest, User>();
            CreateMap<RefreshToken, RefreshTokenModel>();
            CreateMap<Recipe, RecipeModel>();
            CreateMap<CreateRecipeRequest, RecipeModel>();
            CreateMap<UpdateRecipeRequest, RecipeModel>();
            CreateMap<Category, CategoryModel>();
            CreateMap<Tag, TagModel>();
            CreateMap<Document, DocumentModel>();
            CreateMap<CategoryModel, Category>();
            CreateMap<TagModel, Tag>();
            CreateMap<DocumentModel, Document>();
        }
    }
}