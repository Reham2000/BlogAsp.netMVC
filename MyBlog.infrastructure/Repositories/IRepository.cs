using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.infrastructure.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T entity,Action<string> logAction);
        Task UpdateAstnc(T entity, Action<string> logAction);
        Task DeleteAsync(int id, Action<string> logAction);
    }
}
