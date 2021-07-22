using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Distance.Business.Interfaces;

namespace Distance.EntityFrame.Repository
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        readonly EfUnitOfWork _efContext;

        public GenericRepository(EfUnitOfWork efContext)
        {
            _efContext = efContext;
        }

        public IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> criteria )
        {
            return _efContext.Set<TEntity>()
//                .AsNoTracking()
                .Where(criteria);
        }

        public virtual TEntity Get(object id)
        {
            var entity = _efContext.Set<TEntity>().Find(id);
            return entity;
        }

        public IQueryable<TEntity> All
        {
            get
            {
                return _efContext.Set<TEntity>()
                    .AsNoTracking();
            }
        }

        public IQueryable<TEntity> Find(string sql)
        {
            return _efContext.Set<TEntity>().SqlQuery(sql).AsQueryable();
        }

        public void Delete(object id)
        {
            var entity = Get(id);
            Delete(entity);
        }

        public void Delete(TEntity entityObject)
        {
            _efContext.Set<TEntity>().Attach(entityObject);
            _efContext.Set<TEntity>().Remove(entityObject);
            _efContext.SaveChanges();
        }
        public void DeleteShallow(TEntity entityObject)
        {
            _efContext.Set<TEntity>().Attach(entityObject);
            _efContext.Set<TEntity>().Remove(entityObject);
        }
        public virtual void InsertShallow(TEntity obj)
        {
            if (_efContext.Entry(obj).State == EntityState.Detached)
                _efContext.Set<TEntity>().Add(obj);
        }

        public virtual void Save(TEntity obj)
        {
            if (_efContext.Entry(obj).State == EntityState.Detached)
                _efContext.Set<TEntity>().Add(obj);

            _efContext.SaveChanges();
        }

        public virtual void PersistUnsavedChanges( )
        {
            _efContext.SaveChanges();
        }

        public virtual void Update(TEntity obj)
        {
            _efContext.Set<TEntity>().Attach(obj);
            _efContext.Entry(obj).State = EntityState.Modified;
            _efContext.SaveChanges();
        }

        public void Dispose()
        {
        }
    }
}
