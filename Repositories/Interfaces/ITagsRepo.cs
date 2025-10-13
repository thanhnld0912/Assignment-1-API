using EXE_201.Infrastructure.Interfaces;
using Models.Models;

namespace Repositories.Interfaces
{
    public interface ITagsRepo : IRepository<Tag>
    {
        Task<Tag> GetTagById(int id);
        Task<IEnumerable<Tag>> GetTagsByArticle(string articleId);
        Task<IEnumerable<Tag>> GetTagsByIds(List<int> tagIds);
        Task AddTagToArticle(string articleId, int tagId);
        Task RemoveTagFromArticle(string articleId, int tagId);
    }
}
