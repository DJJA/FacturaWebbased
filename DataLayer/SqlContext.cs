using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace DataLayer
{
    public abstract class SqlContext<TEntity> : IContext<TEntity> where TEntity : class
    {
        public string ConnectionString
        {
            get
            {
                return @"Data Source=facturasrv.database.windows.net;Initial Catalog=FacturaDB;Persist Security Info=True;User ID=daphnevandelaar;Password=HnUVN21994";
            }
        }

        public abstract IEnumerable<TEntity> GetAll();

        public abstract void Insert(TEntity entity);

        public abstract void Update(TEntity entity);

        public abstract TEntity GetById(int id);
    }
}
