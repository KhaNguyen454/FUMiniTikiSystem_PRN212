using Microsoft.EntityFrameworkCore;
using System;
using System.Linq; // Quan trọng: Đảm bảo có using System.Linq
using System.Linq.Expressions;
using System.Threading.Tasks;
using DataAccessLayer.Entities;

namespace DataAccessLayer
{
    public class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly FUMiniTikiSystemDBContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public GenericRepository(FUMiniTikiSystemDBContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        // ĐẢM BẢO TRIỂN KHAI NÀY: Trả về IQueryable<TEntity> trực tiếp
        public virtual IQueryable<TEntity> GetAll()
        {
            return _dbSet.AsNoTracking(); // Trả về IQueryable<TEntity>
        }

        public virtual async Task<TEntity?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public virtual Task<IQueryable<TEntity>> FindByConditionAsync(Expression<Func<TEntity, bool>> expression)
        {
            return Task.FromResult(_dbSet.Where(expression).AsNoTracking());
        }
    }
}
