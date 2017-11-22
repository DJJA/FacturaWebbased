using System;
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
        private SqlConnection conn;
        private SqlCommand cmd;
        private SqlDataReader rdr;
        private Invoice invoice;
        private Customer customer;
        private List<Invoice> invoices;

        //TODO: nog alle code nalopen of er geen customers queries zijn & Exception
        public override IEnumerable<Invoice> GetAll()
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
                    string DOR = rdr["dateSend"].ToString();
                    invoice = new Invoice(
                        id: Convert.ToInt16(rdr["id"]),
                        dateSend: DateTime.Parse(DOR),
                        datePayed: DateTime.Parse(DOR),
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

        public void InsertTasksToInvoice(Invoice invoice)
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

        public Invoice GetTasksOnInvoice(Invoice recentInvoice)
        {
            List<Task> tasks = new List<Task>();
            conn = new SqlConnection(ConnectionString);
            try
            {
                int id = recentInvoice.Id;
                cmd = new SqlCommand($"SELECT * FROM funcTasksOnInvoice({id})", conn);

                conn.Open();
               
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    string DOR = this.rdr["date"].ToString();
                    Task task = new Task(
                            description: this.rdr["description"].ToString(),
                            date: DateTime.Parse(DOR),
                            amount: Convert.ToDecimal(this.rdr["amount"]),
                            price: Convert.ToDecimal(this.rdr["price"])
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

        public override void Insert(Invoice invoice)
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
                cmd.Parameters.Add("@DatePayed", SqlDbType.DateTime).Value = invoice.DatePayed;
                cmd.Parameters.Add("@TotalPrice", SqlDbType.Money).Value = invoice.TotalPrice;

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
                    Customer invoiceCustomer= new Customer(
                        id: Convert.ToInt16(rdr["customerid"])
                    );

                    invoice = new Invoice(
                        id: Convert.ToInt16(rdr["id"]),
                        customer: invoiceCustomer,
                        dateSend: Convert.ToDateTime(rdr["dateSend"]),
                        datePayed: Convert.ToDateTime(rdr["datePayed"]),
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
                    string DOR = this.rdr["dateSend"].ToString();
                    invoice = new Invoice(
                            id: Convert.ToInt16(rdr["id"]),
                            customer: customer,
                            dateSend: DateTime.Parse(DOR),
                            datePayed: DateTime.Parse(DOR),
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
