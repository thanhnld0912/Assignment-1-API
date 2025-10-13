using EXE_201.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.Models;

namespace EXE_201.Infrastructure.Implements
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly FunewsManagementContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(FunewsManagementContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task Add(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await Save();
            }
        }
        public async Task Update(T entity)
        {
            _dbSet.Update(entity);
            await Save();
        }

        public async Task<T> GetById(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _dbSet.ToListAsync();
        }


        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
