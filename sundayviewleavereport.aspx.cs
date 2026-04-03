using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class sundayviewleavereport : System.Web.UI.Page
{
    string connStr = ConfigurationManager.ConnectionStrings["db"].ConnectionString;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadData();

        }
    }



    private void LoadData()
    {
        SqlConnection con = new SqlConnection(connStr);
        SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM SundayAttendanceLog WHERE YEAR(CreatedOn) = " + Request.QueryString["year"] + "   AND MONTH(CreatedOn) = " + Request.QueryString["month"] + " and empcode='" + Request.QueryString["EmpCode"] + "';", con);
        DataTable dt = new DataTable();
        da.Fill(dt);

        lvEmployees.DataSource = dt;
        lvEmployees.DataBind();
    }

}