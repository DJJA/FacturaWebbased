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
    public abstract class SqlContext<TEntity> : IContext<TEntity> where TEntity : class
    {
        public string ConnectionString
        {
            get
            {
                return @"Data Source=facturasrv.database.windows.net;Initial Catalog=FacturaDB;Persist Security Info=True;User ID=daphnevandelaar;Password=HnUVN21994";
            }
        }

        protected DataTable GetDataViaProcedure(string procedure, IEnumerable<SqlParameter> procedureParameters)
        {
            var datatable = new DataTable();
            using (var connection = new SqlConnection(ConnectionString))
            using (var adapter = new SqlDataAdapter(procedure, connection))
            {
                //connection.Open();
                adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                if (procedureParameters != null)
                {
                    adapter.SelectCommand.Parameters.AddRange(procedureParameters.ToArray());
                    //foreach (var parammeter in procedureParameters)
                    //{
                    //    adapter.SelectCommand.Parameters.Add(parammeter);
                    //}
                }

                adapter.Fill(datatable);
            }
            return datatable;
        }

        protected void ExecuteProcedure(string procedureQuery, IEnumerable<SqlParameter> procedureParameters)
        {
            using (var sqlConnection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(procedureQuery, sqlConnection))
            {
                command.CommandType = CommandType.StoredProcedure;

                if (procedureParameters != null)
                {
                    command.Parameters.AddRange(procedureParameters.ToArray());
                }

                sqlConnection.Open();  
                command.ExecuteNonQuery();
                sqlConnection.Close();
            }
        }

        protected DataTable GetDataByView(string viewQuery)
        {
            DataTable dataTable = new DataTable();

            using (var sqlConnection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(viewQuery, sqlConnection))
            {
                using (var dataAdapter = new SqlDataAdapter(command))
                {
                    sqlConnection.Open();
                    dataAdapter.Fill(dataTable);
                    sqlConnection.Close();
                }
            }
            
            return dataTable;
        }

        public abstract IEnumerable<TEntity> GetAll();

        public abstract void Insert(TEntity entity);

        public abstract void Update(TEntity entity);

        public abstract TEntity GetById(int id);
    }
}
