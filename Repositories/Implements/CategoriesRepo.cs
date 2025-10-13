using EXE_201.Infrastructure.Implements;
using Microsoft.EntityFrameworkCore;
using Models.Models;
using Repositories.Interfaces;

namespace Repositories.Implements
{
    public class CategoriesRepo : Repository<Category>, ICategoriesRepo
    {

        public CategoriesRepo(FunewsManagementContext context) : base(context)
        {
        }
        public async Task<IEnumerable<Category>> GetCategories()
        {

            return await _context.Categories.Include(c => c.ParentCategory).ToListAsync();
        }
        public async Task UpdateCategory(Category category)
        {

            _context.Entry<Category>(category).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        public async Task DeleteCategory(Category category)
        {


            var parent = _context.Categories
                                .Include(c => c.InverseParentCategory)
                                .SingleOrDefault(c => c.CategoryId == category.CategoryId);

            if (parent != null && parent.InverseParentCategory.Any())
            {
                _context.Categories.RemoveRange(parent.InverseParentCategory); // Xóa các category con
            }

            _context.Categories.Remove(parent); // Xóa category cha
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Category>> SearchCategory(string category)
        {

            return _context.Categories.Where(c => c.CategoryName == category).ToList();
        }
        public async Task<Category> GetCategoryById(short id)
        {
            return await _context.Categories.Include(c => c.ParentCategory).Include(c => c.NewsArticles).SingleOrDefaultAsync(c => c.CategoryId.Equals(id));
        }
    }
}
