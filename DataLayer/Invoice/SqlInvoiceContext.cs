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
        //TODO: Niet specifieke exceptions gewoon exception gebruike en bij specifieke sql met een message.
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
            Customer customer = null;
            if (datarow.Table.Columns.Contains("customerId"))
            {
                customer = new Customer(
                    id: Convert.ToInt16(datarow["customerId"])
                );
            }
            if (datarow.Table.Columns.Contains("firstname"))
            {
                customer = new Customer(
                    firstname: datarow["firstname"].ToString(),
                    lastname: datarow["lastname"].ToString()
                );
            }
           

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
                throw new InvoiceException(
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
                throw new InvoiceException(
                    $"Neem contact op met de beheerder onder sqldatabase exceptionCode:{sqlEx.Number}");
            }
            catch (Exception ex)
            {
                throw new InvoiceException($"Neem contact op met de beheerder onder exceptionCode:{ex.HResult}");
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
                throw new InvoiceException(
                    $"Neem contact op met de beheerder onder sqldatabase exceptionCode:{sqlEx.Number}");
            }
            catch (Exception ex)
            {
                throw new InvoiceException($"Neem contact op met de beheerder onder exceptionCode:{ex.HResult}");
            }
            invoice.Tasks = tasks;
            return invoice;
        }
        public override Invoice GetById(int id)
        {
            Invoice invoice = null;
            try
            {
                var dataTable = GetDataByView($"SELECT * FROM funcInvoiceById({id})");
                if (dataTable.Rows.Count > 0)
                {
                    invoice = InvoiceFromDataRow(dataTable.Rows[0]);
                }
            }
            catch (SqlException sqlEx)
            {
                throw new InvoiceException(
                    $"Neem contact op met de beheerder onder sqldatabase exceptionCode:{sqlEx.Number}");
            }
            catch (Exception ex)
            {
                throw new InvoiceException($"Neem contact op met de beheerder onder exceptionCode:{ex.HResult}");
            }
            return invoice;
        }
        public void InvoicePayed(Invoice invoice)
        {
            //TODO: datetime instelbaar maken
            try
            {
                ExecuteProcedure("spInvoicePayed", new List<SqlParameter>() { new SqlParameter("@Id", invoice.Id), new SqlParameter("@DatePayed", DateTime.Now) });
            }
            catch (SqlException sqlEx)
            {
                throw new InvoiceException(
                    $"Neem contact op met de beheerder onder sqldatabase exceptionCode:{sqlEx.Number}");
            }
            catch (Exception ex)
            {
                throw new InvoiceException($"Neem contact op met de beheerder onder exceptionCode:{ex.HResult}");
            }
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

      
        //TODO: hier loopt nog wat fout, bij klanten pagina doorklikken
        //public IEnumerable<Invoice> GetInvoicesPerCustomer(int customerId)
        //{
        //    //TODO: factuurtabel aanpassen dat niet elke keer de klant erin komt te staan. 
        //    conn = new SqlConnection(ConnectionString);
        //    try
        //    {
        //        cmd = new SqlCommand($"SELECT * FROM funcInvoicesByCustomer({customerId})", conn);

        //        conn.Open();

        //        rdr = cmd.ExecuteReader();
        //        while (rdr.Read())
        //        {
        //            //TODO: voorvoegsel toevoegen
        //            customerr = new Customer(
        //                firstname: rdr["firstname"].ToString(),
        //                lastname: rdr["lastname"].ToString()
        //            );
        //            string DS = this.rdr["dateSend"].ToString();

        //            invoice = new Invoice(
        //                id: Convert.ToInt16(rdr["id"]),
        //                customer: customerr,
        //                dateSend: DateTime.Parse(DS),
        //                datePayed: ReadPaymentDate(rdr["datePayed"].ToString()),
        //                totalPrice: Convert.ToDecimal(rdr["totalprice"])
        //            );
        //            invoices.Add(invoice);
        //        }

        //        return invoices;
        //    }
        //    catch (SqlException sqlException)
        //    {
        //        switch (sqlException.Number)
        //        {
        //            case 1:
        //                throw new CustomerException("Er kon geen verbinding gemaakt worden");
        //            default:
        //                throw new CustomerException(sqlException.Number.ToString());
        //        }
        //    }
        //    finally
        //    {
        //        conn.Close();
        //    }
        //}

        #region old code
        //public IEnumerable<Invoice> _GetAllinv()
        //{
        //    invoices = new List<Invoice>();
        //    conn = new SqlConnection(ConnectionString);
        //    try
        //    {
        //        conn.Open();
        //        cmd = new SqlCommand("SELECT * FROM vwInvoices", conn);

        //        rdr = cmd.ExecuteReader();
        //        while (rdr.Read())
        //        {
        //            customerr = new Customer(
        //                    lastname: rdr["lastname"].ToString(),
        //                    firstname: rdr["firstname"].ToString()
        //                );


        //            //TODO: kijken naar datums, of deze ook NULL kunnen zijn en de correctheid
        //            string DS = rdr["dateSend"].ToString();

        //            invoice = new Invoice(
        //                id: Convert.ToInt16(rdr["id"]),
        //                dateSend: DateTime.Parse(DS),
        //                datePayed: ReadPaymentDate(rdr["datePayed"].ToString()),
        //                customer: customerr,
        //                totalPrice: Convert.ToDecimal(rdr["TotalPrice"])

        //            );
        //            invoices.Add(invoice);
        //        }

        //        return invoices;
        //    }
        //    catch (SqlException sqlException)
        //    {
        //        switch (sqlException.Number)
        //        {

        //            default:
        //                throw new CustomerException(sqlException.Number.ToString());
        //        }
        //    }
        //    finally
        //    {
        //        conn.Close();
        //    }

        //}

        //public void _IinsertTasksToInvoice(Invoice invoice)
        //{
        //    conn = new SqlConnection(ConnectionString);


        //    try
        //    {
        //        foreach (var task in invoice.Tasks)
        //        {
        //            cmd = new SqlCommand("spInsertTaskToInvoice", conn);
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.Add("@TaskId", SqlDbType.Int).Value = task.Id;
        //            cmd.Parameters.Add("@InvoiceId", SqlDbType.Int).Value = invoice.Id;
        //            cmd.Parameters.Add("@date", SqlDbType.Date).Value = task.Date;
        //            cmd.Parameters.Add("@amount", SqlDbType.Decimal).Value = task.Amount;
        //            cmd.Parameters.Add("@price", SqlDbType.Money).Value = task.Price;
        //            cmd.Parameters.Add("@unit", SqlDbType.Int).Value = (int)task.Unit;

        //            conn.Open();
        //            cmd.ExecuteNonQuery();
        //            conn.Close();
        //        }
        //    }
        //    catch (SqlException sqlException)
        //    {
        //        switch (sqlException.Number)
        //        {
        //            default:
        //                throw new CustomerException(sqlException.Number.ToString());
        //        }
        //    }
        //    finally
        //    {
        //        conn.Close();
        //    }

        //}

        //public Invoice _GetTaskssOnInvoice(Invoice recentInvoice)
        //{
        //    List<Task> tasks = new List<Task>();
        //    conn = new SqlConnection(ConnectionString);
        //    try
        //    {

        //        cmd = new SqlCommand($"SELECT * FROM funcTasksOnInvoice({recentInvoice.Id})", conn);

        //        conn.Open();

        //        rdr = cmd.ExecuteReader();
        //        while (rdr.Read())
        //        {

        //            string DOR = this.rdr["date"].ToString();
        //            Task task = new Task(
        //                    description: this.rdr["description"].ToString(),
        //                    date: DateTime.Parse(DOR),
        //                    amount: Convert.ToDecimal(this.rdr["amount"]),
        //                    price: Convert.ToDecimal(this.rdr["price"]),
        //                    unit: (Unit)Convert.ToInt16(rdr["unit"])
        //                );
        //            tasks.Add(task);
        //            recentInvoice.Tasks = tasks;

        //            invoice = recentInvoice;
        //        }

        //        return invoice;
        //    }
        //    catch (SqlException sqlException)
        //    {
        //        switch (sqlException.Number)
        //        {
        //            case 1:
        //                throw new CustomerException("Er kon geen verbinding gemaakt worden");
        //            default:
        //                throw new CustomerException(sqlException.Number.ToString());
        //        }
        //    }
        //    finally
        //    {
        //        conn.Close();
        //    }
        //}

        //public void _InsertInvoice(Invoice invoice)
        //{
        //    conn = new SqlConnection(ConnectionString);
        //    //TODO: using gebruiken ipv con open en close
        //    try
        //    {

        //        cmd = new SqlCommand("spManageInvoice", conn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.Add("@id", SqlDbType.Int).Value = 0;
        //        cmd.Parameters.Add("@CustomerId", SqlDbType.Int).Value = invoice.Customer.ID;
        //        cmd.Parameters.Add("@DateSend", SqlDbType.DateTime).Value = invoice.DateSend;
        //        cmd.Parameters.Add("@TotalPrice", SqlDbType.Money).Value = invoice.TotalPrice;

        //        if (invoice.DatePayed == default(DateTime))
        //        {
        //            cmd.Parameters.Add("@DatePayed", SqlDbType.DateTime).Value = DBNull.Value;
        //        }
        //        else
        //        {
        //            cmd.Parameters.Add("@DatePayed", SqlDbType.DateTime).Value = invoice.DatePayed;
        //        }

        //        SqlParameter output = new SqlParameter("@InvoiceId", SqlDbType.Int);
        //        output.Direction = ParameterDirection.Output;
        //        cmd.Parameters.Add(output);

        //        conn.Open();
        //        cmd.ExecuteNonQuery();
        //    }
        //    catch (SqlException sqlException)
        //    {
        //        switch (sqlException.Number)
        //        {
        //            //TODO: Errorafhandeling 
        //            default:
        //                throw new CustomerException(sqlException.Number.ToString());
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new CustomerException(ex.Message);
        //    }
        //    finally
        //    {
        //        conn.Close();
        //    }

        //    invoice.Id = Convert.ToInt16(cmd.Parameters["@InvoiceId"].Value.ToString());
        //    InsertTasksToInvoice(invoice);
        //}

        //public Invoice _GetById(int id)
        //{
        //    conn = new SqlConnection(ConnectionString);
        //    try
        //    {
        //        conn.Open();
        //        cmd = new SqlCommand($"SELECT * FROM funcInvoiceById({id})", conn);

        //        rdr = cmd.ExecuteReader();
        //        while (rdr.Read())
        //        {
        //            Customer invoiceCustomer = new Customer(
        //                id: Convert.ToInt16(rdr["customerid"])
        //            );


        //            invoice = new Invoice(
        //                id: Convert.ToInt16(rdr["id"]),
        //                customer: invoiceCustomer,
        //                dateSend: Convert.ToDateTime(rdr["dateSend"]),
        //                datePayed: ReadPaymentDate(rdr["datePayed"].ToString()),
        //                totalPrice: Convert.ToDecimal(rdr["totalPrice"])
        //            );
        //        }

        //        return invoice;
        //    }
        //    catch (SqlException sqlException)
        //    {
        //        switch (sqlException.Number)
        //        {
        //            case 1:
        //                throw new CustomerException("Er kon geen verbinding gemaakt worden");
        //            default:
        //                throw new CustomerException(sqlException.Number.ToString());
        //        }
        //    }
        //    finally
        //    {
        //        conn.Close();
        //    }
        //}
        #endregion

    }
}
