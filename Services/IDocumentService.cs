using Database;
using RecipeLewis.Models.Results;
using RecipeLewis.Models;
using RecipeLewis.Models.Requests;

namespace RecipeLewis.Services
{
    public interface IDocumentService
    {
        public List<DocumentModel> GetAll(RecipeId recipeId); 
        public DocumentModel Get(RecipeId recipeId, int documentId);
        public bool AddDocuments(List<DocumentModel> documentsToAdd, RecipeId recipeId, UserModel currentUser);
        public bool DeleteDocuments(List<DocumentModel> documentsToDelete, RecipeId recipeId, UserModel currentUser);
    }
}
