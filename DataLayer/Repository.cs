using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;



namespace DataLayer
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        public IContext<TEntity> Context { get; protected set; }

        public Repository(IContext<TEntity> context)
        {
            Context = context;
        }

        public TEntity Get(int id)
        {
            return Context.GetById(id);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return Context.GetAll();
        }

        public void Add(TEntity entity)
        {
            Context.Insert(entity);
        }

        public void Update(TEntity entity)
        {
            Context.Update(entity);
        }

        public TEntity GetById(int id)
        {
            return Context.GetById(id);
        }
    }
}
