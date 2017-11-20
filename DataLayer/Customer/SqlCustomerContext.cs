using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace DataLayer
{
    public class SqlCustomerContext : SqlContext<Customer>, ICustomerContext
    {
        private SqlConnection conn;
        private SqlCommand cmd;
        private SqlDataReader rdr;
        private Customer customer;
        private List<Customer> customers;
        //TODO: kijken of de SqlDbTypes in de sp queries goed zijn

        public override IEnumerable<Customer> GetAll()
        {
            customers = new List<Customer>();

            conn = new SqlConnection(ConnectionString);
            try
            {
                conn.Open();
                cmd = new SqlCommand("SELECT * FROM CUSTOMER", conn);

                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Address address = new Address(
                        streetName: rdr["street"].ToString(),
                        place: rdr["place"].ToString(),
                        zipCode: rdr["zipCode"].ToString()
                    );

                    customer = new Customer(
                        id: Convert.ToInt16(rdr["id"]),
                        firstName: rdr["firstname"].ToString(),
                        preposition: rdr["prefix"].ToString(),
                        lastName: rdr["lastname"].ToString(),
                        email: rdr["email"].ToString(),
                        phoneNumber: rdr["phonenumber"].ToString(),
                        address: address
                    );
                    customers.Add(customer);
                }

                return customers;
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

        public override void Insert(Customer customer)
        {
            conn = new SqlConnection(ConnectionString);

            try
            {

                cmd = new SqlCommand("spManageCustomer", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = 5;
                cmd.Parameters.Add("@FirstName", SqlDbType.Text).Value = customer.FirstName;
                cmd.Parameters.Add("@LastName", SqlDbType.Text).Value = customer.LastName;
                cmd.Parameters.Add("@Prefix", SqlDbType.Text).Value = customer.Preposition;
                cmd.Parameters.Add("@Street", SqlDbType.Text).Value = customer.Address.StreetName;
                cmd.Parameters.Add("@Place", SqlDbType.Text).Value = customer.Address.Place;
                cmd.Parameters.Add("@ZipCode", SqlDbType.Text).Value = customer.Address.ZipCode;
                cmd.Parameters.Add("@Email", SqlDbType.Text).Value = customer.Email;
                cmd.Parameters.Add("@phoneNumber", SqlDbType.Text).Value = customer.PhoneNumber;
                cmd.Parameters.Add("@StatementType", SqlDbType.Text).Value = "insert";


                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (SqlException sqlException)
            {
                switch (sqlException.Number)
                {
                    case 2627:
                        throw new CustomerException("Er bestaat al een klant met dit email");

                    case 547:
                        throw new CustomerException("Het email adres voldoet niet aan de eisen van een email");
                    default:
                        throw new CustomerException(sqlException.Number.ToString());
                }
            }
        }

        public override void Update(Customer customer)
        {
            conn = new SqlConnection(ConnectionString);

            try
            {
                cmd = new SqlCommand("spManageCustomer", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = customer.ID;
                cmd.Parameters.Add("@FirstName", SqlDbType.Text).Value = customer.FirstName;
                cmd.Parameters.Add("@LastName", SqlDbType.Text).Value = customer.LastName;
                cmd.Parameters.Add("@Prefix", SqlDbType.Text).Value = customer.Preposition;
                cmd.Parameters.Add("@Street", SqlDbType.Text).Value = customer.Address.StreetName;
                cmd.Parameters.Add("@Place", SqlDbType.Text).Value = customer.Address.Place;
                cmd.Parameters.Add("@ZipCode", SqlDbType.Text).Value = customer.Address.ZipCode;
                cmd.Parameters.Add("@Email", SqlDbType.Text).Value = customer.Email;
                cmd.Parameters.Add("@phoneNumber", SqlDbType.Text).Value = customer.PhoneNumber;
                cmd.Parameters.Add("@StatementType", SqlDbType.Text).Value = "update";


                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (SqlException exc)
            {
                switch (exc.Number)
                {
                    case 2627:
                        throw new CustomerException("Er bestaat al een klant met dit email");

                    case 547:
                        throw new CustomerException("Het email adres voldoet niet aan de eisen van een email");
                    default:
                        throw new CustomerException(exc.Number.ToString());
                }
            }
        }

        public override Customer GetById(int id)
        {
            conn = new SqlConnection(ConnectionString);
            try
            {
                conn.Open();
                cmd = new SqlCommand($"SELECT * FROM CustomerById({id})", conn);

                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Address address = new Address(
                        streetName: rdr["street"].ToString(),
                        place: rdr["place"].ToString(),
                        zipCode: rdr["zipCode"].ToString()
                    );

                    customer = new Customer(
                        id: Convert.ToInt16(rdr["id"]),
                        firstName: rdr["firstname"].ToString(),
                        preposition: rdr["prefix"].ToString(),
                        lastName: rdr["lastname"].ToString(),
                        email: rdr["email"].ToString(),
                        phoneNumber: rdr["phonenumber"].ToString(),
                        address: address
                    );
                }

                return customer;
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

        public IEnumerable<Customer> GetCustomersByZipcode(string zipcode)
        {
            conn = new SqlConnection(ConnectionString);
            try
            {
                customers = new List<Customer>();
                conn.Open();
                cmd = new SqlCommand($"SELECT * FROM funcCustomerByZipcode('{zipcode}')", conn);

                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Address address = new Address(
                        streetName: rdr["street"].ToString(),
                        place: rdr["place"].ToString(),
                        zipCode: rdr["zipCode"].ToString()
                    );

                    customer = new Customer(
                        id: Convert.ToInt16(rdr["id"]),
                        firstName: rdr["firstname"].ToString(),
                        lastName: rdr["lastname"].ToString(),
                        email: rdr["email"].ToString(),
                        phoneNumber: rdr["phonenumber"].ToString(),
                        address: address
                    );
                    customers.Add(customer);
                }

                return customers;
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

        public IEnumerable<Customer> GetCustomersByLastName(string lastname)
        {
            conn = new SqlConnection(ConnectionString);
            try
            {
                customers = new List<Customer>();
                conn.Open();
                cmd = new SqlCommand($"SELECT * FROM funcCustomerByLastName('{lastname}')", conn);

                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Address address = new Address(
                        streetName: rdr["street"].ToString(),
                        place: rdr["place"].ToString(),
                        zipCode: rdr["zipCode"].ToString()
                    );

                    customer = new Customer(
                        id: Convert.ToInt16(rdr["id"]),
                        firstName: rdr["firstname"].ToString(),
                        lastName: rdr["lastname"].ToString(),
                        email: rdr["email"].ToString(),
                        phoneNumber: rdr["phonenumber"].ToString(),
                        address: address
                    );
                    customers.Add(customer);
                }

                return customers;
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

