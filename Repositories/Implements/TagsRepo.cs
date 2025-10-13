using EXE_201.Infrastructure.Implements;
using Microsoft.EntityFrameworkCore;
using Models.Models;
using Repositories.Interfaces;

namespace Repositories.Implements
{
    public class TagsRepo : Repository<Tag>, ITagsRepo
    {

        public TagsRepo(FunewsManagementContext context) : base(context)
        {
        }

        public async Task AddTagToArticle(string articleId, int tagId)
        {
            var article = await _context.NewsArticles
                .Include(a => a.Tags)
                .FirstOrDefaultAsync(a => a.NewsArticleId == articleId);

            var tag = await _context.Tags.FirstOrDefaultAsync(t => t.TagId == tagId);

            if (article != null && tag != null && !article.Tags.Any(t => t.TagId == tagId))
            {
                _context.Attach(tag); // avoid tracking conflict
                article.Tags.Add(tag);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Tag> GetTagById(int id)
        {
            return await _context.Tags.AsNoTracking().FirstOrDefaultAsync(t => t.TagId == id);
        }

        public async Task<IEnumerable<Tag>> GetTagsByArticle(string articleId)
        {
            return await _context.NewsArticles
                .Where(a => a.NewsArticleId == articleId)
                .SelectMany(a => a.Tags)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Tag>> GetTagsByIds(List<int> tagIds)
        {
            return await _context.Tags
                .AsNoTracking()
                .Where(t => tagIds.Contains(t.TagId))
                .ToListAsync();
        }

        public async Task RemoveTagFromArticle(string articleId, int tagId)
        {
            var article = await _context.NewsArticles
                .Include(a => a.Tags)
                .FirstOrDefaultAsync(a => a.NewsArticleId == articleId);

            var tag = await _context.Tags.FirstOrDefaultAsync(t => t.TagId == tagId);

            if (article != null && tag != null && article.Tags.Any(t => t.TagId == tagId))
            {
                article.Tags.Remove(tag);
                await _context.SaveChangesAsync();
            }
        }
    }
}
