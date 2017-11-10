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

        //TODO: kijken of ze abstract kunnen
        public virtual IEnumerable<TEntity> GetAll()
        {
            throw new NotImplementedException();
        }

        public virtual void Insert(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public virtual void Update(TEntity entity) 
        {
            throw new NotImplementedException();
        }

        public virtual TEntity GetById(int id)
        {
            throw new NotImplementedException();
        }

        public virtual TEntity FindByEntity(TEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
