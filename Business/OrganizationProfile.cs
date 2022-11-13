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
            CreateMap<Recipe, RecipeModel>()
                .ForMember(x => x.Documents, x => x.Ignore());
            CreateMap<CreateRecipeRequest, RecipeModel>();
            CreateMap<UpdateRecipeRequest, RecipeModel>();
            CreateMap<CreateRecipeRequest, Recipe>();
            CreateMap<UpdateRecipeRequest, Recipe>()
                .ForMember(x => x.Documents, x => x.Ignore())
                .ForMember(x => x.Category, x => x.Ignore())
                .ForMember(x => x.Tags, x => x.Ignore());
            CreateMap<Category, CategoryModel>();
            CreateMap<CategoryModel, Category>();
            CreateMap<Tag, TagModel>();
            CreateMap<TagModel, Tag>();
            CreateMap<Document, DocumentModel>();
            CreateMap<DocumentModel, Document>();
        }
    }
}