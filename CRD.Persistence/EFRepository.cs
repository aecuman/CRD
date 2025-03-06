using CRD.Application.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CRD.Persistence
{
    public class EFRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly ApplicationDbContext _context;
        public EFRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void Add(in TEntity sender)
        {
          var l =  _context.Set<TEntity>().Add(sender);
            
            _context.SaveChanges();
           
        }
        public void AddWithoutSaving(in TEntity sender)
        {
           _context.Set<TEntity>().Add(sender);
        }
        public void AddMany(IEnumerable<TEntity> entities)
        {
            _context.Set<TEntity>().AddRange(entities);
            _context.SaveChanges();
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _context.Set<TEntity>();
        }

        public async Task<List<TEntity>> GetAllAsync()
        {
            return await  _context.Set<TEntity>().ToListAsync();
        }

        public TEntity GetById(int id)
        {
            return _context.Set<TEntity>().Find(id);
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }
        public IQueryable<TEntity> IncludeMultiple(IQueryable<TEntity> query, params Expression<Func<TEntity, object>>[] includes)
        {
            if (includes != null)
            {
                query = includes.Aggregate(query,
                          (current, include) => current.Include(include));
            }

            return query;
        }
        public IQueryable<TEntity> GetAllIncludes(Expression<Func<TEntity, object>>[] children)
        {
            
            var query = _context.Set<TEntity>().AsQueryable();
            query = children.Aggregate(query,
                          (current, include) => current.Include(include));
            return query;
        }
     
        public IQueryable<TEntity> IncludeMultiple(Expression<Func<TEntity, object>>[] includes)
        {
            var query= _context.Set<TEntity>().AsQueryable();
            if (includes != null)
            {
                query = includes.Aggregate(query,
                          (current, include) => current.Include(include));
            }

            return query;
        }

        public TEntity GetByIdWithIncludes(int id)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> GetByIdWithIncludesAsync(int id)
        {
            throw new NotImplementedException();
        }

        public bool Remove(int id)
        {
            var entity = GetById(id);
            _context.Set<TEntity>().Remove(entity);
            _context.SaveChanges();
            return true;
        }

        public int Save()
        {
            return _context.SaveChanges();
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public TEntity Select(Expression<Func<TEntity, bool>> predicate)
        {
            return _context.Set<TEntity>().FirstOrDefault(predicate);
        }

        public Task<TEntity> SelectAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return _context.Set<TEntity>().FirstOrDefaultAsync(predicate);
        }

        public void Update(in TEntity sender)
        {
            _context.Entry(sender).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
        /*public void Add(TEntity model)
{
_context.Set<TEntity>().Add(model);
_context.SaveChanges();
}

public void AddRange(IEnumerable<TEntity> model)
{
_context.Set<TEntity>().AddRange(model);
_context.SaveChanges();
}

public TEntity? GetId(int id)
{
return _context.Set<TEntity>().Find(id);
}

public async Task<TEntity?> GetIdAsync(int id)
{
return await _context.Set<TEntity>().FindAsync(id);
}

public TEntity? Get(Expression<Func<TEntity, bool>> predicate)
{
return _context.Set<TEntity>().FirstOrDefault(predicate);
}

public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate)
{
return await _context.Set<TEntity>().FirstOrDefaultAsync(predicate);
}

public IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> predicate)
{
return _context.Set<TEntity>().Where<TEntity>(predicate).ToList();
}

public async Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate)
{
return await Task.Run(() => _context.Set<TEntity>().Where<TEntity>(predicate));
}

public IEnumerable<TEntity> GetAll()
{
return _context.Set<TEntity>().ToList();
}

public async Task<IEnumerable<TEntity>> GetAllAsync()
{
return await Task.Run(() => _context.Set<TEntity>());
}

public int Count()
{
return _context.Set<TEntity>().Count();
}

public async Task<int> CountAsync()
{
return await _context.Set<TEntity>().CountAsync();
}

public void Update(TEntity objModel)
{
_context.Entry(objModel).State = EntityState.Modified;
_context.SaveChanges();
}

public void Remove(TEntity objModel)
{
_context.Set<TEntity>().Remove(objModel);
_context.SaveChanges();
}

public void Dispose()
{
_context.Dispose();
}*/
    }

}
