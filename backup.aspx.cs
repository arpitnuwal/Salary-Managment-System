using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class backup : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        LoadInsertScript();
    }

 








    private void LoadInsertScript()
    {
        string conStr = ConfigurationManager.ConnectionStrings["db"].ConnectionString;

        string sql = @"SELECT STRING_AGG(
    'INSERT INTO Users (Id, Username, Password) VALUES (' +
    CAST(Id AS VARCHAR(50)) + ', ' +
    '''' + REPLACE(ISNULL(Username,''), '''', '''''') + ''', ' +
    '''' + REPLACE(ISNULL([Password],''), '''', '''''') + '''' +
    ');',
    CHAR(13) + CHAR(10)
) AS InsertQuery
FROM Users;";

        StringBuilder sb = new StringBuilder();

        using (SqlConnection con = new SqlConnection(conStr))
        using (SqlCommand cmd = new SqlCommand(sql, con))
        {
            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                sb.AppendLine(dr["InsertQuery"].ToString());
            }
        }

        txtEmpName.Text = sb.ToString();
    }
}