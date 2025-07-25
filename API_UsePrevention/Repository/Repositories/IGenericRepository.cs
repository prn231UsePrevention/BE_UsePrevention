using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public interface IGenericRepository<T>
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task DeleteAsync(T entity);
        Task UpdateAsync(T entity);
        IQueryable<T> Query();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task AddRangeAsync(IEnumerable<T> entities);
        IQueryable<T> Find(Expression<Func<T, bool>> predicate);
        IGenericRepository<CommunityProgram> CommunityProgramRepository { get; }

        Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate);


    }
}
