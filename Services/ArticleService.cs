using Models.Models;
using Repositories.Interfaces;

namespace Services
{
    public class ArticleService
    {
        private readonly IArticlesRepo _articleRepo;
        public ArticleService(IArticlesRepo articleRepo)
        {
            _articleRepo = articleRepo;
        }
        public Task AddNewsArticle(NewsArticle article) => _articleRepo.AddNewsArticle(article);

        public Task DeleteArticle(NewsArticle article) => _articleRepo.DeleteArticle(article);

        public Task<NewsArticle> GetArticleById(string id) => _articleRepo.GetArticleById(id);

        public Task<IEnumerable<NewsArticle>> GetArticlesByUserId(short id) => _articleRepo.GetArticlesByUserId(id);

        public Task<IEnumerable<NewsArticle>> GetNewsArticles() => _articleRepo.GetNewsArticles();

        public Task<IEnumerable<NewsArticle>> SearchAricles(string NewsTitle) => _articleRepo.SearchArticles(NewsTitle);

        public Task UpdateArticle(NewsArticle article) => _articleRepo.Update(article);

    }
}
