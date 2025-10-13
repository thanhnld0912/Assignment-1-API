using EXE_201.Infrastructure.Interfaces;
using Models.Models;

namespace Repositories.Interfaces
{
    public interface ICategoriesRepo : IRepository<Category>
    {
        Task UpdateCategory(Category category);
        Task DeleteCategory(Category category);

        Task<IEnumerable<Category>> SearchCategory(string category);
        Task<Category> GetCategoryById(short id);
        Task<IEnumerable<Category>> GetCategories();
    }
}
