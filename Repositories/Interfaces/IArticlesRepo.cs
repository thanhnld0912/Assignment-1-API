using EXE_201.Infrastructure.Interfaces;
using Models.Models;

namespace Repositories.Interfaces
{
    public interface IArticlesRepo : IRepository<NewsArticle>
    {
        Task<IEnumerable<NewsArticle>> GetNewsArticles();
        Task DeleteArticle(NewsArticle article);

        Task<IEnumerable<NewsArticle>> SearchArticles(string NewsTitle);

        Task<NewsArticle> GetArticleById(string id);
        Task<IEnumerable<NewsArticle>> GetArticlesByUserId(short id);
        Task AddNewsArticle(NewsArticle article);
    }
}
