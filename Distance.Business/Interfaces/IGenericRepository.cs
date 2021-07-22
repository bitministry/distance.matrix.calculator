using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Distance.Business.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        TEntity Get(object id);

        IQueryable<TEntity> All { get; }
        IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> criteria);
        IQueryable<TEntity> Find(string sql);

        void Delete(object id);
        void Delete(TEntity entityObject);
        
        void DeleteShallow(TEntity entityObject);
        void InsertShallow(TEntity entityObject);
        void PersistUnsavedChanges();

        void Save(TEntity obj);
        void Update(TEntity obj);
        void Dispose();
    }
}
