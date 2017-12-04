using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace DataLayer
{
    public abstract class SqlContext<TEntity> : DbHandler, IContext<TEntity> where TEntity : class
    {
        public abstract IEnumerable<TEntity> GetAll();

        public abstract void Insert(TEntity entity);

        public abstract void Update(TEntity entity);

        public abstract TEntity GetById(int id);
    }
}
