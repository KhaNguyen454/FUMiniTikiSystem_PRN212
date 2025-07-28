using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;

namespace DataAccessLayer
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> GetAll(); // Trả về IQueryable trực tiếp
        Task<TEntity?> GetByIdAsync(int id);
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity); // Đảm bảo tên này nhất quán
        Task DeleteAsync(int id); // Đảm bảo tên này nhất quán
        Task<IQueryable<TEntity>> FindByConditionAsync(Expression<Func<TEntity, bool>> expression);
    }
}