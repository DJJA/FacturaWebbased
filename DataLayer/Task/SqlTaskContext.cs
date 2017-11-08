﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Models;

namespace DataLayer
{
    public class SqlTaskContext : SqlContext<Task>, ITaskContext
    {
        private SqlConnection conn;
        private SqlCommand cmd;
        private SqlDataReader rdr;
        private Task task;
        private List<Task> tasks;

        public override IEnumerable<Task> GetAll()
        {
            tasks = new List<Task>();

            conn = new SqlConnection(ConnectionString);
            try
            {
                conn.Open();
                cmd = new SqlCommand("SELECT * FROM Task", conn);

                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    task = new Task(Convert.ToInt16(rdr["id"]), rdr["description"].ToString());

                    tasks.Add(task);
                }

                return tasks;
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

        public override void Insert(Task entity)
        {
            conn = new SqlConnection(ConnectionString);

            try
            {

                cmd = new SqlCommand("spManageCustomer", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = 0;
                cmd.Parameters.Add("@description", SqlDbType.Text).Value = task.Description;
                cmd.Parameters.Add("@StatementType", SqlDbType.Text).Value = "insert";


                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (SqlException sqlException)
            {
                switch (sqlException.Number)
                {
                    case 2627:
                        throw new TaskException("Er bestaat al een klant met dit email");

                    case 547:
                        throw new TaskException("Het email adres voldoet niet aan de eisen van een email");
                    default:      
                        throw new TaskException(sqlException.Number.ToString());
                }
            }
        }
    }
}