using Models.Models;
using Repositories.Interfaces;

namespace Services
{
    public class CategoryService
    {
        private readonly ICategoriesRepo _cateRepo;
        public CategoryService(ICategoriesRepo cateRepo)
        {
            _cateRepo = cateRepo;
        }
        public Task AddCategory(Category category) => _cateRepo.Add(category);
        public Task DeleteCategory(Category category) => _cateRepo.DeleteCategory(category);

        public Task<IEnumerable<Category>> GetCategories() => _cateRepo.GetAll();

        public Task<Category> GetCategoryById(short id) => _cateRepo.GetCategoryById(id);

        public Task<IEnumerable<Category>> SearchCategory(string category) => _cateRepo.SearchCategory(category);

        public Task UpdateCategory(Category category) => _cateRepo.UpdateCategory(category);
    }
}
