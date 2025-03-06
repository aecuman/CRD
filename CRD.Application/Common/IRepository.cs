using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Application.Common
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        Task<List<T>> GetAllAsync();
        T GetById(int id);
        T GetByIdWithIncludes(int id);
        Task<T> GetByIdAsync(int id);
        Task<T> GetByIdWithIncludesAsync(int id);
        bool Remove(int id);
        void Add(in T sender);
        void AddWithoutSaving(in T sender);
        void AddMany(IEnumerable<T> entities);
        void Update(in T sender);
        int Save();
        Task<int> SaveChangesAsync();
        Task<int> SaveAsync();
        public T Select(Expression<Func<T, bool>> predicate);
        public Task<T> SelectAsync(Expression<Func<T, bool>> predicate);
        IQueryable<T> GetAllIncludes(Expression<Func<T, object>>[] children);
        IQueryable<T> IncludeMultiple(IQueryable<T> query, params Expression<Func<T, object>>[] includes);
    }
}
