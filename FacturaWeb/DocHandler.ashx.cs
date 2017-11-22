using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FacturaWeb
{
    /// <summary>
    /// Summary description for DocHandler
    /// </summary>
    public class DocHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string id = "1";
            string sConn = @"Data Source=facturasrv.database.windows.net;Initial Catalog=FacturaDB;Persist Security Info=True;User ID=daphnevandelaar;Password=HnUVN21994";
            SqlConnection objConn = new SqlConnection(sConn);
            objConn.Open();
            string sTSQL = "select Name_File,Extension,ContentType,FileData,FileSize from Documents where SNo=@ID";
            SqlCommand objCmd = new SqlCommand(sTSQL, objConn);
            objCmd.CommandType = CommandType.Text;
            objCmd.Parameters.AddWithValue("@ID", id);
            SqlDataAdapter ada = new SqlDataAdapter(objCmd);
            DataTable file = new DataTable();
            ada.Fill(file);
            objConn.Close();
            objCmd.Dispose();
            if (file.Rows.Count > 0)
            {
                DataRow row = file.Rows[0];
                string name = (string)row["Name_File"];
                string contentType = (string)row["ContentType"];
                Byte[] data = (Byte[])row["FileData"];
                int FileSize = Convert.ToInt32(row["FileSize"].ToString());
                // Send the file to the browser  
                context.Response.AddHeader("Content-type", contentType);
                context.Response.AddHeader("Content-   Disposition", "attachment; filename=" + name);
                context.Response.OutputStream.Write(data, 0, FileSize);
                context.Response.Flush();
                context.Response.End();
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}