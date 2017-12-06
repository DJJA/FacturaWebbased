using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class DbHandler
    {
        private string ConnectionString => @"Data Source=facturasrv.database.windows.net;Initial Catalog=FacturaDB;Persist Security Info=True;User ID=daphnevandelaar;Password=HnUVN21994";

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
        internal DataTable GetDataByView(string viewQuery)
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

        //todo: output parameter buite deze class maken
        protected SqlParameter ExecuteProcedureWithOutput(string procedureQuery, IEnumerable<SqlParameter> procedureParameters, string queryoutput)
        {
            SqlParameter output = new SqlParameter(queryoutput, SqlDbType.Int);

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
    }
}
