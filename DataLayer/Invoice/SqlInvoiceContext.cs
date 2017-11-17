﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
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
                    //if (rdr["dateSend"] != null)
                    //{
                        
                    //}
                    invoice = new Invoice(
                        id: Convert.ToInt16(rdr["id"]),
                        dateSend: DateTime.Now, 
                        datePayed: DateTime.MinValue, 
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
                cmd.Parameters.Add("@Amount", SqlDbType.Int).Value = invoice.Amount;
                cmd.Parameters.Add("@TotalPrice", SqlDbType.Money).Value = invoice.TotalPrice;
                cmd.Parameters.Add("@StatementType", SqlDbType.Text).Value = "insert";

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
            throw new NotImplementedException();
        }

    }
}
