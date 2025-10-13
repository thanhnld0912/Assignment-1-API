using Models.Models;
using Repositories.Interfaces;

namespace Services
{
    public class TagService
    {
        private readonly ITagsRepo _tagRepo;
        public TagService(ITagsRepo tagRepo)
        {
            _tagRepo = tagRepo;
        }
        public Task AddTagToArticle(string articleId, int tagId) => _tagRepo.AddTagToArticle(articleId, tagId);

        public Task<Tag> GetTagById(int id) => _tagRepo.GetTagById(id);
        public Task<IEnumerable<Tag>> GetTags() => _tagRepo.GetAll();

        public Task<IEnumerable<Tag>> GetTagsByArticle(string articleId) => _tagRepo.GetTagsByArticle(articleId);

        public Task<IEnumerable<Tag>> GetTagsByIds(List<int> tagIds) => _tagRepo.GetTagsByIds(tagIds);

        public Task RemoveTagFromArticle(string articleId, int tagId) => _tagRepo.RemoveTagFromArticle(articleId, tagId);
    }
}
