using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace DataLayer
{
    

    public class SqlCustomerContext : SqlContext<Customer>, ICustomerContext
    {
        public int CustomerIdAfterInsertion { get; set; }

        #region all parameter settings
        private IEnumerable<SqlParameter> CustomerSqlParameters(Customer customer)
        {
            var parameters = new List<SqlParameter>()
            {
                new SqlParameter("@FirstName", customer.FirstName),
                new SqlParameter("@Prefix", customer.Preposition),
                new SqlParameter("@LastName", customer.LastName),
                new SqlParameter("@PhoneNumber", customer.PhoneNumber),
                new SqlParameter("@Email", customer.Email),
                new SqlParameter("@Street", customer.Address.StreetName),
                new SqlParameter("@ZipCode", customer.Address.ZipCode),
                new SqlParameter("@Place", customer.Address.Place),
                new SqlParameter("@Id", customer.ID)
            };

            foreach (var parameter in parameters)
            {
                if (parameter.Value == null) parameter.Value = DBNull.Value;
            }
            return parameters;
        }
        private Customer CustomerFromDataRow(DataRow datarow)
        {
            Address address = new Address(
                streetName: datarow["street"].ToString(),
                place: datarow["place"].ToString(),
                zipCode: datarow["zipCode"].ToString()
            );

            return new Customer(
                id: Convert.ToInt16(datarow["id"]),
                firstName: datarow["firstname"].ToString(),
                preposition: datarow["prefix"].ToString(),
                lastName: datarow["lastname"].ToString(),
                email: datarow["email"].ToString(),
                phoneNumber: datarow["phonenumber"].ToString(),
                address: address
            );
        }
        #endregion

        public override IEnumerable<Customer> GetAll()
        {
            //TODO: kijken of list, IEnumerable kan blijven
            var customers = new List<Customer>();
            try
            {
                var dataTable = GetDataByView("SELECT * FROM CUSTOMER");
                customers.AddRange(from DataRow row in dataTable.Rows select CustomerFromDataRow(row));
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
            return customers;
        }
        public override void Insert(Customer customer)
        {
            try
            {
                var parameter = ExecuteProcedureWithOutput("spManageCustomer", CustomerSqlParameters(customer), "@customerId");
                CustomerIdAfterInsertion = Convert.ToInt32(parameter.Value.ToString());
            }
            catch (SqlException sqlEx)
            {
                throw new CustomerException(
                    $"Neem contact op met de beheerder onder sqldatabase exceptionCode:{sqlEx.Number}");
            }
        }
        public override void Update(Customer customer)
        {
            try
            {
                ExecuteProcedure("spManageCustomer", CustomerSqlParameters(customer));
            }
            catch (SqlException sqlEx)
            {
                switch (sqlEx.Number)
                {
                    case 2627:
                        throw new CustomerException("Er bestaat al een klant met dit email");

                    case 547:
                        throw new CustomerException("Het email adres voldoet niet aan de eisen van een email");
                    default:
                        throw new CustomerException(
                            $"Neem contact op met de beheerder onder sqldatabase exceptionCode:{sqlEx.Number}");
                }
            }
        }
        public IEnumerable<Customer> GetCustomersByZipcode(string zipcode)
        {
            var customers = new List<Customer>();
            try
            {
                var dataTable = GetDataByView($"SELECT * FROM funcCustomerByZipcode('{zipcode}')");
                customers.AddRange(from DataRow row in dataTable.Rows select CustomerFromDataRow(row));
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
            return customers;
        }
        public IEnumerable<Customer> GetCustomersByLastName(string lastname)
        {
            var customers = new List<Customer>();
            try
            {
                var dataTable = GetDataByView($"SELECT * FROM funcCustomerByLastName('{lastname}')");
                customers.AddRange(from DataRow row in dataTable.Rows select CustomerFromDataRow(row));
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
            return customers;
        }

        

        public override Customer GetById(int id)
        {
            Customer customer = null;
            try
            {
                var dataTable = GetDataByView($"SELECT * FROM funcCustomerById({id})");
                if (dataTable.Rows.Count > 0)
                {
                    customer = CustomerFromDataRow(dataTable.Rows[0]);
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
            return customer;
        }




        #region old code

        //    public Customer GetcusById(int id)
        //    {
        //        conn = new SqlConnection(ConnectionString);
        //        try
        //        {
        //            conn.Open();
        //            cmd = new SqlCommand($"SELECT * FROM CustomerById({id})", conn);

        //            rdr = cmd.ExecuteReader();
        //            while (rdr.Read())
        //            {
        //                Address address = new Address(
        //                    streetName: rdr["street"].ToString(),
        //                    place: rdr["place"].ToString(),
        //                    zipCode: rdr["zipCode"].ToString()
        //                );

        //                customer = new Customer(
        //                    id: Convert.ToInt16(rdr["id"]),
        //                    firstName: rdr["firstname"].ToString(),
        //                    preposition: rdr["prefix"].ToString(),
        //                    lastName: rdr["lastname"].ToString(),
        //                    email: rdr["email"].ToString(),
        //                    phoneNumber: rdr["phonenumber"].ToString(),
        //                    address: address
        //                );
        //            }

        //            return customer;
        //        }
        //        catch (SqlException sqlException)
        //        {
        //            switch (sqlException.Number)
        //            {
        //                case 1:
        //                    throw new CustomerException("Er kon geen verbinding gemaakt worden");
        //                default:
        //                    throw new CustomerException(sqlException.Number.ToString());
        //            }
        //        }
        //        finally
        //        {
        //            conn.Close();
        //        }
        //    }

        //    public IEnumerable<Customer> GetAllCus()
        //    {
        //        customers = new List<Customer>();

        //        conn = new SqlConnection(ConnectionString);
        //        try
        //        {
        //            conn.Open();
        //            cmd = new SqlCommand("SELECT * FROM CUSTOMER", conn);

        //            rdr = cmd.ExecuteReader();
        //            while (rdr.Read())
        //            {
        //                Address address = new Address(
        //                    streetName: rdr["street"].ToString(),
        //                    place: rdr["place"].ToString(),
        //                    zipCode: rdr["zipCode"].ToString()
        //                );

        //                customer = new Customer(
        //                    id: Convert.ToInt16(rdr["id"]),
        //                    firstName: rdr["firstname"].ToString(),
        //                    preposition: rdr["prefix"].ToString(),
        //                    lastName: rdr["lastname"].ToString(),
        //                    email: rdr["email"].ToString(),
        //                    phoneNumber: rdr["phonenumber"].ToString(),
        //                    address: address
        //                );
        //                customers.Add(customer);
        //            }

        //            return customers;
        //        }
        //        catch (SqlException sqlException)
        //        {
        //            switch (sqlException.Number)
        //            {
        //                case 1:
        //                    throw new CustomerException("Er kon geen verbinding gemaakt worden");
        //                default:
        //                    throw new CustomerException(sqlException.Number.ToString());
        //            }
        //        }
        //        finally
        //        {
        //            conn.Close();
        //        }
        //    }
        //    public void Insertcus(Customer customer)
        //    {
        //        conn = new SqlConnection(ConnectionString);

        //        try
        //        {
        //            cmd = new SqlCommand("spManageCustomer", conn);
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.Add("@id", SqlDbType.Int).Value = 5;
        //            cmd.Parameters.Add("@FirstName", SqlDbType.Text).Value = customer.FirstName;
        //            cmd.Parameters.Add("@LastName", SqlDbType.Text).Value = customer.LastName;
        //            cmd.Parameters.Add("@Prefix", SqlDbType.Text).Value = customer.Preposition;
        //            cmd.Parameters.Add("@Street", SqlDbType.Text).Value = customer.Address.StreetName;
        //            cmd.Parameters.Add("@Place", SqlDbType.Text).Value = customer.Address.Place;
        //            cmd.Parameters.Add("@ZipCode", SqlDbType.Text).Value = customer.Address.ZipCode;
        //            cmd.Parameters.Add("@Email", SqlDbType.Text).Value = customer.Email;
        //            cmd.Parameters.Add("@phoneNumber", SqlDbType.Text).Value = customer.PhoneNumber;
        //            cmd.Parameters.Add("@StatementType", SqlDbType.Text).Value = "insert";


        //            conn.Open();
        //            cmd.ExecuteNonQuery();
        //        }
        //        catch (SqlException sqlException)
        //        {
        //            switch (sqlException.Number)
        //            {
        //                case 2627:
        //                    throw new CustomerException("Er bestaat al een klant met dit email");

        //                case 547:
        //                    throw new CustomerException("Het email adres voldoet niet aan de eisen van een email");
        //                default:
        //                    throw new CustomerException(sqlException.Number.ToString());
        //            }
        //        }
        //    }
        //    public void UpdateCus(Customer customer)
        //    {
        //        conn = new SqlConnection(ConnectionString);

        //        try
        //        {
        //            cmd = new SqlCommand("spManageCustomer", conn);
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.Add("@id", SqlDbType.Int).Value = customer.ID;
        //            cmd.Parameters.Add("@FirstName", SqlDbType.Text).Value = customer.FirstName;
        //            cmd.Parameters.Add("@LastName", SqlDbType.Text).Value = customer.LastName;
        //            cmd.Parameters.Add("@Prefix", SqlDbType.Text).Value = customer.Preposition;
        //            cmd.Parameters.Add("@Street", SqlDbType.Text).Value = customer.Address.StreetName;
        //            cmd.Parameters.Add("@Place", SqlDbType.Text).Value = customer.Address.Place;
        //            cmd.Parameters.Add("@ZipCode", SqlDbType.Text).Value = customer.Address.ZipCode;
        //            cmd.Parameters.Add("@Email", SqlDbType.Text).Value = customer.Email;
        //            cmd.Parameters.Add("@phoneNumber", SqlDbType.Text).Value = customer.PhoneNumber;
        //            cmd.Parameters.Add("@StatementType", SqlDbType.Text).Value = "update";


        //            conn.Open();
        //            cmd.ExecuteNonQuery();
        //        }
        //        catch (SqlException exc)
        //        {
        //            switch (exc.Number)
        //            {
        //                case 2627:
        //                    throw new CustomerException("Er bestaat al een klant met dit email");

        //                case 547:
        //                    throw new CustomerException("Het email adres voldoet niet aan de eisen van een email");
        //                default:
        //                    throw new CustomerException(exc.Number.ToString());
        //            }
        //        }
        //    }
        //    public IEnumerable<Customer> GetCustomersByZipcodee(string zipcode)
        //    {
        //        conn = new SqlConnection(ConnectionString);
        //        try
        //        {
        //            customers = new List<Customer>();
        //            conn.Open();
        //            cmd = new SqlCommand($"SELECT * FROM funcCustomerByZipcode('{zipcode}')", conn);

        //            rdr = cmd.ExecuteReader();
        //            while (rdr.Read())
        //            {
        //                Address address = new Address(
        //                    streetName: rdr["street"].ToString(),
        //                    place: rdr["place"].ToString(),
        //                    zipCode: rdr["zipCode"].ToString()
        //                );

        //                customer = new Customer(
        //                    id: Convert.ToInt16(rdr["id"]),
        //                    firstName: rdr["firstname"].ToString(),
        //                    lastName: rdr["lastname"].ToString(),
        //                    email: rdr["email"].ToString(),
        //                    phoneNumber: rdr["phonenumber"].ToString(),
        //                    address: address
        //                );
        //                customers.Add(customer);
        //            }

        //            return customers;
        //        }
        //        catch (SqlException sqlException)
        //        {
        //            switch (sqlException.Number)
        //            {
        //                case 1:
        //                    throw new CustomerException("Er kon geen verbinding gemaakt worden");
        //                default:
        //                    throw new CustomerException(sqlException.Number.ToString());
        //            }
        //        }
        //        finally
        //        {
        //            conn.Close();
        //        }
        //    }
        //    public IEnumerable<Customer> GetCustomersByLastNamee(string lastname)
        //    {
        //        conn = new SqlConnection(ConnectionString);
        //        try
        //        {
        //            customers = new List<Customer>();
        //            conn.Open();
        //            cmd = new SqlCommand($"SELECT * FROM funcCustomerByLastName('{lastname}')", conn);

        //            rdr = cmd.ExecuteReader();
        //            while (rdr.Read())
        //            {
        //                Address address = new Address(
        //                    streetName: rdr["street"].ToString(),
        //                    place: rdr["place"].ToString(),
        //                    zipCode: rdr["zipCode"].ToString()
        //                );

        //                customer = new Customer(
        //                    id: Convert.ToInt16(rdr["id"]),
        //                    firstName: rdr["firstname"].ToString(),
        //                    lastName: rdr["lastname"].ToString(),
        //                    email: rdr["email"].ToString(),
        //                    phoneNumber: rdr["phonenumber"].ToString(),
        //                    address: address
        //                );
        //                customers.Add(customer);
        //            }

        //            return customers;
        //        }
        //        catch (SqlException sqlException)
        //        {
        //            switch (sqlException.Number)
        //            {
        //                case 1:
        //                    throw new CustomerException("Er kon geen verbinding gemaakt worden");
        //                default:
        //                    throw new CustomerException(sqlException.Number.ToString());
        //            }
        //        }
        //        finally
        //        {
        //            conn.Close();
        //        }
        //    }
        //}

        #endregion
    }
}

