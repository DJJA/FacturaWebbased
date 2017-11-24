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
        protected SqlParameter ExecuteProcedureWithOutput(string procedureQuery, IEnumerable<SqlParameter> procedureParameters)
        {
            SqlParameter output = new SqlParameter("@InvoiceId", SqlDbType.Int);

            using (var sqlConnection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(procedureQuery, sqlConnection))
            {
                command.CommandType = CommandType.StoredProcedure;

                if (procedureParameters != null)
                {

                    output.Direction = ParameterDirection.Output;

                    command.Parameters.Add(output);
                    command.Parameters.AddRange(procedureParameters.ToArray());
                }

                sqlConnection.Open();
                command.ExecuteNonQuery();
                sqlConnection.Close();
            }

            return output;
        }





        public abstract IEnumerable<TEntity> GetAll();

        public abstract void Insert(TEntity entity);

        public abstract void Update(TEntity entity);

        public abstract TEntity GetById(int id);
    }
}
