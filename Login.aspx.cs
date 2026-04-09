using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        lblErrorMsg.Text = "";
        pnlError.Visible = false;
    }
    
    protected void btnLogin_Click(object sender, EventArgs e)
    {
        string username = txtUsername.Text.Trim();
        string password = txtPassword.Text.Trim();

        string connStr = ConfigurationManager.ConnectionStrings["db"].ConnectionString;

        using (SqlConnection con = new SqlConnection(connStr))
        {
            string query = "SELECT COUNT(*) FROM Users WHERE Username=@Username AND Password=@Password";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Username", username);
            cmd.Parameters.AddWithValue("@Password", password);

            con.Open();
            int count = (int)cmd.ExecuteScalar();

            if (count > 0)
            {
                // Login success
                Session["Username"] = username;
                Response.Redirect("SalaryReport.aspx");
            }
            else
            {
                // Error show
                pnlError.Visible = true;
                lblErrorMsg.Text = "Invalid Username or Password ❌";
            }
        }
    }
}