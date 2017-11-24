using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using Models;

namespace DataLayer
{
    public class SqlInvoiceContext : SqlContext<Invoice>, IInvoiceContext
    {
        //TODO: kijken of de public methods internal of protected kunnen
        //TODO:IEnumerable .AddRange() method bij getall om niet van de opgehaalde data een list te maken

        private SqlConnection conn;
        private SqlCommand cmd;
        private SqlDataReader rdr;
        private Invoice invoice;
        private Customer customer;
        private List<Invoice> invoices;

        #region all parameter settings
        private IEnumerable<SqlParameter> InvoiceSqlParameters(Invoice invoice)
        {
            var parameters = new List<SqlParameter>()
            {
                new SqlParameter("@Id", invoice.Id),
                new SqlParameter("@customerId", invoice.Customer.ID),
                new SqlParameter("@TotalPrice", invoice.TotalPrice),
                new SqlParameter("@DateSend", invoice.DateSend),
            };

            if (invoice.DatePayed == new DateTime())
            {
                parameters.Add(new SqlParameter("@DatePayed", DBNull.Value));
            }
            else
            {
                parameters.Add(new SqlParameter("@DatePayed", invoice.DatePayed));
            }

            foreach (var parameter in parameters)
            {
                if (parameter.Value == null) parameter.Value = DBNull.Value;
            }
            return parameters;
        }
        private IEnumerable<SqlParameter> TasksOnInvoiceSqlParameters(int invoiceId, Task task)
        {
            var parameters = new List<SqlParameter>();
            
                parameters.Add(new SqlParameter("@TaskId", task.Id));
                parameters.Add(new SqlParameter("@InvoiceId", invoiceId));
                parameters.Add(new SqlParameter("@date", task.Date));
                parameters.Add(new SqlParameter("@amount", task.Amount));
                parameters.Add(new SqlParameter("@price", task.Price));
                parameters.Add(new SqlParameter("@unit", (int)task.Unit));
            
            return parameters;
        }
        private Invoice InvoiceFromDataRow(DataRow datarow)
        {
            customer = new Customer(
                lastname: datarow["lastname"].ToString(),
                firstname: datarow["firstname"].ToString()
            );

            return new Invoice(
                id: Convert.ToInt16(datarow["id"]),
                dateSend: ReadPaymentDate(datarow["dateSend"].ToString()),
                datePayed: ReadPaymentDate(datarow["datePayed"].ToString()),
                customer: customer,
                totalPrice: Convert.ToDecimal(datarow["TotalPrice"])

            );
        }
        private List<Task> TasksOnInvoiceFromDataRow(DataRow datarow)
        {
            return new List<Task>()
            {
                new Task(
                    description: datarow["description"].ToString(),
                    date: ReadPaymentDate(datarow["date"].ToString()),
                    amount: Convert.ToDecimal(datarow["amount"]),
                    price: Convert.ToDecimal(datarow["price"]),
                    unit: (Unit)Convert.ToInt16(datarow["unit"])
                )
            }; 
        }
        #endregion

        public override void Insert(Invoice invoice)
        {
            try
            {
                var parameter = ExecuteProcedureWithOutput("spManageInvoice", InvoiceSqlParameters(invoice));
                invoice.Id = Convert.ToInt32(parameter.Value.ToString());
                InsertTasksToInvoice(invoice);
            }
            catch (SqlException sqlEx)
            {
                throw new InvoiceException(sqlEx.Message);
            }
        }
        public void InsertTasksToInvoice(Invoice invoice)
        {
            try
            {
                foreach (var task in invoice.Tasks)
                {
                    ExecuteProcedure("spInsertTaskToInvoice", TasksOnInvoiceSqlParameters(invoice.Id, task));
                }
            }
            catch (SqlException sqlEx)
            {
                throw new CustomerException(
                    $"Neem contact op met de beheerder onder sqldatabase exceptionCode:{sqlEx.Number}");
            }
        }
        public override IEnumerable<Invoice> GetAll()
        {
            var invoices = new List<Invoice>();
            try
            {
                var dataTable = GetDataByView("SELECT * FROM vwInvoices");
                invoices.AddRange(from DataRow row in dataTable.Rows select InvoiceFromDataRow(row));
            }
            catch (SqlException sqlEx)
            {
                throw new CustomerException(
                    $"Neem contact op met de beheerder onder sqldatabase exceptionCode:{sqlEx.Number}");
            }
            catch (Exception ex)
            {
                throw new CustomerException($"Neem contact op met de beheerder onder exceptionCode:{ex.HResult}");
            }
            return invoices;
        }
        public Invoice GetTasksOnInvoice(Invoice invoice)
        {
            List<Task> tasks = new List<Task>();
            try
            {
                var dataTable = GetDataByView($"SELECT * FROM funcTasksOnInvoice({invoice.Id})");
                if (dataTable.Rows.Count > 0)
                {
                    tasks = TasksOnInvoiceFromDataRow(dataTable.Rows[0]);
                }
            }
            catch (SqlException sqlEx)
            {
                throw new CustomerException(
                    $"Neem contact op met de beheerder onder sqldatabase exceptionCode:{sqlEx.Number}");
            }
            catch (Exception ex)
            {
                throw new CustomerException($"Neem contact op met de beheerder onder exceptionCode:{ex.HResult}");
            }
            invoice.Tasks = tasks;
            return invoice;
        }


        //TODO: nog alle code nalopen of er geen customers queries zijn & Exception
        public IEnumerable<Invoice> _GetAllinv()
        {
            invoices = new List<Invoice>();
            conn = new SqlConnection(ConnectionString);
            try
            {
                conn.Open();
                cmd = new SqlCommand("SELECT * FROM vwInvoices", conn);

                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    customer = new Customer(
                            lastname: rdr["lastname"].ToString(),
                            firstname: rdr["firstname"].ToString()
                        );


                    //TODO: kijken naar datums, of deze ook NULL kunnen zijn en de correctheid
                    string DS = rdr["dateSend"].ToString();

                    invoice = new Invoice(
                        id: Convert.ToInt16(rdr["id"]),
                        dateSend: DateTime.Parse(DS),
                        datePayed: ReadPaymentDate(rdr["datePayed"].ToString()),
                        customer: customer,
                        totalPrice: Convert.ToDecimal(rdr["TotalPrice"])

                    );
                    invoices.Add(invoice);
                }

                return invoices;
            }
            catch (SqlException sqlException)
            {
                switch (sqlException.Number)
                {

                    default:
                        throw new CustomerException(sqlException.Number.ToString());
                }
            }
            finally
            {
                conn.Close();
            }

        }

        public void _IinsertTasksToInvoice(Invoice invoice)
        {
            conn = new SqlConnection(ConnectionString);


            try
            {
                foreach (var task in invoice.Tasks)
                {
                    cmd = new SqlCommand("spInsertTaskToInvoice", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@TaskId", SqlDbType.Int).Value = task.Id;
                    cmd.Parameters.Add("@InvoiceId", SqlDbType.Int).Value = invoice.Id;
                    cmd.Parameters.Add("@date", SqlDbType.Date).Value = task.Date;
                    cmd.Parameters.Add("@amount", SqlDbType.Decimal).Value = task.Amount;
                    cmd.Parameters.Add("@price", SqlDbType.Money).Value = task.Price;
                    cmd.Parameters.Add("@unit", SqlDbType.Int).Value = (int)task.Unit;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (SqlException sqlException)
            {
                switch (sqlException.Number)
                {
                    default:
                        throw new CustomerException(sqlException.Number.ToString());
                }
            }
            finally
            {
                conn.Close();
            }

        }

        public Invoice _GetTaskssOnInvoice(Invoice recentInvoice)
        {
            List<Task> tasks = new List<Task>();
            conn = new SqlConnection(ConnectionString);
            try
            {

                cmd = new SqlCommand($"SELECT * FROM funcTasksOnInvoice({recentInvoice.Id})", conn);

                conn.Open();

                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {

                    string DOR = this.rdr["date"].ToString();
                    Task task = new Task(
                            description: this.rdr["description"].ToString(),
                            date: DateTime.Parse(DOR),
                            amount: Convert.ToDecimal(this.rdr["amount"]),
                            price: Convert.ToDecimal(this.rdr["price"]),
                            unit: (Unit)Convert.ToInt16(rdr["unit"])
                        );
                    tasks.Add(task);
                    recentInvoice.Tasks = tasks;

                    invoice = recentInvoice;
                }

                return invoice;
            }
            catch (SqlException sqlException)
            {
                switch (sqlException.Number)
                {
                    case 1:
                        throw new CustomerException("Er kon geen verbinding gemaakt worden");
                    default:
                        throw new CustomerException(sqlException.Number.ToString());
                }
            }
            finally
            {
                conn.Close();
            }
        }

        public void _InsertInvoice(Invoice invoice)
        {
            conn = new SqlConnection(ConnectionString);
            //TODO: using gebruiken ipv con open en close
            try
            {

                cmd = new SqlCommand("spManageInvoice", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = 0;
                cmd.Parameters.Add("@CustomerId", SqlDbType.Int).Value = invoice.Customer.ID;
                cmd.Parameters.Add("@DateSend", SqlDbType.DateTime).Value = invoice.DateSend;
                cmd.Parameters.Add("@TotalPrice", SqlDbType.Money).Value = invoice.TotalPrice;

                if (invoice.DatePayed == default(DateTime))
                {
                    cmd.Parameters.Add("@DatePayed", SqlDbType.DateTime).Value = DBNull.Value;
                }
                else
                {
                    cmd.Parameters.Add("@DatePayed", SqlDbType.DateTime).Value = invoice.DatePayed;
                }

                SqlParameter output = new SqlParameter("@InvoiceId", SqlDbType.Int);
                output.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(output);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (SqlException sqlException)
            {
                switch (sqlException.Number)
                {
                    //TODO: Errorafhandeling 
                    default:
                        throw new CustomerException(sqlException.Number.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new CustomerException(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            invoice.Id = Convert.ToInt16(cmd.Parameters["@InvoiceId"].Value.ToString());
            InsertTasksToInvoice(invoice);
        }

        public override void Update(Invoice invoice)
        {
            throw new NotImplementedException();
        }

        private DateTime ReadPaymentDate(string date)
        {
            string DP = date;

            DateTime dt;
            if (DP == string.Empty)
            {
                return new DateTime();
            }
            else
            {
                return DateTime.Parse(DP);
            }
        }

        public override Invoice GetById(int id)
        {
            conn = new SqlConnection(ConnectionString);
            try
            {
                conn.Open();
                cmd = new SqlCommand($"SELECT * FROM funcInvoiceById({id})", conn);

                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Customer invoiceCustomer = new Customer(
                        id: Convert.ToInt16(rdr["customerid"])
                    );


                    invoice = new Invoice(
                        id: Convert.ToInt16(rdr["id"]),
                        customer: invoiceCustomer,
                        dateSend: Convert.ToDateTime(rdr["dateSend"]),
                        datePayed: ReadPaymentDate(rdr["datePayed"].ToString()),
                        totalPrice: Convert.ToDecimal(rdr["totalPrice"])
                    );
                }

                return invoice;
            }
            catch (SqlException sqlException)
            {
                switch (sqlException.Number)
                {
                    case 1:
                        throw new CustomerException("Er kon geen verbinding gemaakt worden");
                    default:
                        throw new CustomerException(sqlException.Number.ToString());
                }
            }
            finally
            {
                conn.Close();
            }
        }


        //TODO: hier loopt nog wat fout, bij klanten pagina doorklikken
        public IEnumerable<Invoice> GetInvoicesPerCustomer(int customerId)
        {
            //TODO: factuurtabel aanpassen dat niet elke keer de klant erin komt te staan. 
            conn = new SqlConnection(ConnectionString);
            try
            {
                cmd = new SqlCommand($"SELECT * FROM funcInvoicesByCustomer({customerId})", conn);

                conn.Open();

                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    //TODO: voorvoegsel toevoegen
                    customer = new Customer(
                            firstname: rdr["firstname"].ToString(),
                            lastname: rdr["lastname"].ToString()
                        );
                    string DS = this.rdr["dateSend"].ToString();

                    invoice = new Invoice(
                            id: Convert.ToInt16(rdr["id"]),
                            customer: customer,
                            dateSend: DateTime.Parse(DS),
                            datePayed: ReadPaymentDate(rdr["datePayed"].ToString()),
                            totalPrice: Convert.ToDecimal(rdr["totalprice"])
                        );
                    invoices.Add(invoice);
                }

                return invoices;
            }
            catch (SqlException sqlException)
            {
                switch (sqlException.Number)
                {
                    case 1:
                        throw new CustomerException("Er kon geen verbinding gemaakt worden");
                    default:
                        throw new CustomerException(sqlException.Number.ToString());
                }
            }
            finally
            {
                conn.Close();
            }
        }

        public void InsertInvoiceFile(PdfInvoice invoice)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                string commandText = @"insert into Documents(Name_File,DisplayName,Extension,ContentType,FileData,FileSize,UploadDate) values(@Name_File,@DisplayName,@Extension,@ContentType,@FileData,@FileSize,getdate())";
                cmd.CommandText = commandText;
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Add("@Name_File", SqlDbType.VarChar);
                cmd.Parameters["@Name_File"].Value = invoice.Name_File;

                cmd.Parameters.Add("@DisplayName", SqlDbType.VarChar);
                cmd.Parameters["@DisplayName"].Value = invoice.DisplayName;

                cmd.Parameters.Add("@Extension", SqlDbType.VarChar);
                cmd.Parameters["@Extension"].Value = invoice.Extension;

                cmd.Parameters.Add("@ContentType", SqlDbType.VarChar);
                cmd.Parameters["@ContentType"].Value = invoice.ContentType;

                cmd.Parameters.Add("@FileData", SqlDbType.VarBinary);
                cmd.Parameters["@FileData"].Value = invoice.FileData;

                cmd.Parameters.Add("@FileSize", SqlDbType.BigInt);
                cmd.Parameters["@FileSize"].Value = invoice.FileSize;

                cmd.ExecuteNonQuery();
                cmd.Dispose();
                connection.Close();
            }
        }


        //TODO: Deze wordt niet gebruikt, filehandling wordt op een andere manier gedaan.
        public IEnumerable<PdfInvoice> GetInvoiceFile()
        {
            PdfInvoice pdf = new PdfInvoice();
            List<PdfInvoice> pdfs = new List<PdfInvoice>();

            SqlConnection objConn = new SqlConnection(ConnectionString);
            objConn.Open();
            string sTSQL = "select * from Documents";
            SqlCommand objCmd = new SqlCommand(sTSQL, objConn);
            objCmd.CommandType = CommandType.Text;
            SqlDataAdapter ada = new SqlDataAdapter(objCmd);
            DataTable dt = new DataTable();
            ada.Fill(dt);
            objConn.Close();
            objCmd.Dispose();

            conn = new SqlConnection(ConnectionString);
            try
            {
                cmd = new SqlCommand($"select * from Documents", conn);

                conn.Open();

                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    pdf.ContentType = rdr["ContentType"].ToString();
                    pdf.Name_File = rdr["Name_File"].ToString();

                    byte[] toBytes = Encoding.ASCII.GetBytes(rdr["FileData"].ToString());
                    pdf.FileData = toBytes;
                    pdf.Extension = rdr["Extension"].ToString();

                    pdfs.Add(pdf);
                }

                return pdfs;
            }
            catch (SqlException sqlException)
            {
                switch (sqlException.Number)
                {
                    case 1:
                        throw new CustomerException("Er kon geen verbinding gemaakt worden");
                    default:
                        throw new CustomerException(sqlException.Number.ToString());
                }
            }
            finally
            {
                conn.Close();
            }

        }
    }
}
