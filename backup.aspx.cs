using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class backup : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
       
    }

 


     string conStr = ConfigurationManager.ConnectionStrings["db"].ConnectionString;

      protected void btnBackup_Click(object sender, EventArgs e)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["db"].ConnectionString;
        StringBuilder sb = new StringBuilder();

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();

            // Get all user tables
            SqlCommand cmdTables = new SqlCommand(
                "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE' ",
                con);

            SqlDataReader drTables = cmdTables.ExecuteReader();

            DataTable dtTables = new DataTable();
            dtTables.Load(drTables);

            foreach (DataRow tableRow in dtTables.Rows)
            {
                string tableName = tableRow["TABLE_NAME"].ToString();

                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM [" + tableName + "]", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
              
                Label1.Text = dt.Rows.Count.ToString();
                foreach (DataRow row in dt.Rows)
               
            {
                // INSERT INTO [TableName] (
                sb.Append("INSERT INTO [" + tableName + "] (");

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    sb.Append("[" + dt.Columns[i].ColumnName + "]");

                    if (i < dt.Columns.Count - 1)
                        sb.Append(", ");
                }

                sb.Append(") VALUES (");

                // Values
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    object val = row[i];

                    if (val == DBNull.Value)
                    {
                        sb.Append("NULL");
                    }
                    else if (dt.Columns[i].DataType == typeof(string) ||
                             dt.Columns[i].DataType == typeof(DateTime))
                    {
                        sb.Append("'" + val.ToString().Replace("'", "''") + "'");
                    }
                    else
                    {
                        sb.Append(val.ToString());
                    }

                    if (i < dt.Columns.Count - 1)
                        sb.Append(", ");
                }

                sb.AppendLine(");");
            }

            sb.AppendLine();
        }

              
            
        }

        // Output in textbox / download file
       // txtOutput.Text = sb.ToString();
        Response.Clear();
        Response.ContentType = "text/plain";
        Response.AddHeader("content-disposition", "attachment;filename=backup.sql");
        Response.Write(sb.ToString());
        Response.End();
    }
      protected void Button1_Click(object sender, EventArgs e)
      {
          
          string connectionString = ConfigurationManager.ConnectionStrings["db"].ConnectionString;
         

          // file se insert script read
          string insertScript = txtOutput.Text;

          using (SqlConnection con = new SqlConnection(connectionString))
          {
              con.Open();

              SqlTransaction tran = con.BeginTransaction();

              try
              {
                  // 1) sab tables ka data delete
                  string deleteQuery = @"
                EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL';
                EXEC sp_MSforeachtable 'DELETE FROM ?';
                EXEC sp_MSforeachtable 'ALTER TABLE ? CHECK CONSTRAINT ALL';
            ";

                  SqlCommand cmdDelete = new SqlCommand(deleteQuery, con, tran);
                  cmdDelete.ExecuteNonQuery();

                  // 2) backup script execute
                  SqlCommand cmdInsert = new SqlCommand(insertScript, con, tran);
                  cmdInsert.CommandTimeout = 0; // large data ke liye
                  cmdInsert.ExecuteNonQuery();

                  tran.Commit();

                  lblMsg.Text = "Data restored successfully.";
              }
              catch (Exception ex)
              {
                  tran.Rollback();
                  lblMsg.Text = ex.Message;
              }
          }
      }
}



   
