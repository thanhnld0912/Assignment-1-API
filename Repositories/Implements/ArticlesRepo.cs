using EXE_201.Infrastructure.Implements;
using Microsoft.EntityFrameworkCore;
using Models.Models;
using Repositories.Interfaces;

namespace Repositories.Implements
{
    public class ArticlesRepo : Repository<NewsArticle>, IArticlesRepo
    {

        public ArticlesRepo(FunewsManagementContext context) : base(context)
        {
        }

        public async Task DeleteArticle(NewsArticle article)
        {

            var p1 = _context.NewsArticles.SingleOrDefault(c => c.NewsArticleId == article.NewsArticleId);
            _context.NewsArticles.Remove(p1);
            await _context.SaveChangesAsync();
        }

        public async Task<NewsArticle> GetArticleById(string id)
        {
            return await _context.NewsArticles.
                Include(n => n.Category).
                Include(n => n.Tags).
                Include(n => n.CreatedBy).
                SingleOrDefaultAsync(c => c.NewsArticleId.Equals(id));

        }
        public async Task AddNewsArticle(NewsArticle article)
        {
            if (article.Tags?.Any() == true)
            {
                foreach (var tag in article.Tags)
                {
                    var existing = _context.Tags.Local.FirstOrDefault(t => t.TagId == tag.TagId);
                    if (existing != null)
                        _context.Entry(existing).State = EntityState.Unchanged;
                    else
                        _context.Attach(tag);
                }
            }

            await _context.NewsArticles.AddAsync(article);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<NewsArticle>> GetArticlesByUserId(short id)
        {
            return await _context.NewsArticles.Where(c => c.CreatedBy.AccountId.Equals(id)).ToListAsync();
        }

        public async Task<IEnumerable<NewsArticle>> GetNewsArticles()
        {
            return await _context.NewsArticles.Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.Tags)
               .ToListAsync();
        }

        public async Task<IEnumerable<NewsArticle>> SearchArticles(string NewsTitle)
        {
            return await _context.NewsArticles.Where(c => c.NewsTitle == NewsTitle).ToListAsync();
        }
    }
}
