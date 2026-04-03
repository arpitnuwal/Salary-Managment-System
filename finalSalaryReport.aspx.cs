using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class finalSalaryReport : System.Web.UI.Page
{
    string connStr = ConfigurationManager.ConnectionStrings["db"].ConnectionString;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindMonth();
            BindYear();
        }
    }

    private void BindMonth()
    {
        ddlMonth.Items.Clear();
        ddlMonth.Items.Add(new ListItem("Month", ""));

        for (int i = 1; i <= 12; i++)
        {
            string monthName = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i);
            ddlMonth.Items.Add(new ListItem(monthName, i.ToString()));
        }
    }




    private void BindYear()
    {
        ddlYear.Items.Clear();
        ddlYear.Items.Add(new ListItem("Year", ""));

        int startYear = 2026;
        int currentYear = DateTime.Now.Year;

        for (int i = startYear; i <= currentYear; i++)
        {
            ddlYear.Items.Add(new ListItem(i.ToString(), i.ToString()));
        }
    }


    private void LoadData()
    {
        SqlConnection con = new SqlConnection(connStr);
        SqlDataAdapter da = new SqlDataAdapter("select a.*,"+ddlMonth.SelectedValue+" as month from finalsalaryreport   as a  where a.reportmonth='" + ddlMonth.SelectedItem + "' and	a.reportyear='" + ddlYear.SelectedItem + "'", con);
        DataTable dt = new DataTable();
        da.Fill(dt);

        lvEmployees.DataSource = dt;
        lvEmployees.DataBind();
    }
   
    protected void Button1_Click(object sender, EventArgs e)
    {
        DateTime attendanceDate = DateTime.Now; // example, replace with your variable

        // Get full month name
        string monthName = attendanceDate.ToString("MMMM"); // e.g., "March"

        // Get year as string
        string year = attendanceDate.Year.ToString();
        using (SqlConnection con = new SqlConnection("Data Source=mssql2017.adnshost.com,1533;Initial Catalog=testdb;User ID=testdb;Password=testdb@2575"))
        {
            con.Open();
            SqlCommand cmddelete = new SqlCommand("delete  from finalsalaryreport   where reportmonth='"+monthName+"' and	reportyear='"+year+"'", con);
            cmddelete.ExecuteNonQuery();













            foreach (ListViewItem item in lvEmployees.Items)
            {
                string empCode = ((Literal)item.FindControl("litEmpCode")).Text;
                string empName = ((Literal)item.FindControl("LitEmpName")).Text;
                double deductionDays = Convert.ToDouble(((Literal)item.FindControl("LitDeductionDays")).Text);

                string sundayText = ((Literal)item.FindControl("LitSundayAmount")).Text;
                int sundayWorked = Convert.ToInt32(((Literal)item.FindControl("LitSundayWorked")).Text);
                
                double esiCut = Convert.ToDouble(((Literal)item.FindControl("litESICut")).Text);
                double grossSalary = Convert.ToDouble(((Literal)item.FindControl("litGrossSalary")).Text);
                double commission = Convert.ToDouble(((Literal)item.FindControl("litCommission")).Text);
                double netSalary = Convert.ToDouble(((Literal)item.FindControl("litNetSalary")).Text);
                double dueAdvance = Convert.ToDouble(((Literal)item.FindControl("litDueAdvance")).Text);

                double tea = Convert.ToDouble(((TextBox)item.FindControl("txtTea")).Text);
                double extraLess = Convert.ToDouble(((TextBox)item.FindControl("txtExtraAmountLess")).Text);
                double extraComm = Convert.ToDouble(((TextBox)item.FindControl("txtExtraCommission")).Text);
                double advance = Convert.ToDouble(((TextBox)item.FindControl("txtAdvance")).Text);


                int addamt = Convert.ToInt32(tea) + Convert.ToInt32(extraComm);
                int lessamount = Convert.ToInt32(extraLess) + Convert.ToInt32(advance);


                netSalary = netSalary + addamt;
                netSalary = netSalary - lessamount;
                dueAdvance = dueAdvance - advance;

                SqlCommand cmd = new SqlCommand(@"INSERT INTO finalsalaryreport
                (empcode,Name,deductionday,sunday,sundayamountextra,esicut,tea,extracommsion,grosssalary,Commission,advanceamountcut,dueadvance,NetSalary,reportmonth,reportyear,rts)
                VALUES
                ('" + empCode + "','" + empName + "','" + deductionDays + "','" + sundayWorked + "'," + sundayText + ",'" + esiCut + "','" + tea + "','" + extraComm + "'," + grossSalary + ",'"+commission+"','"+advance+"','"+dueAdvance+"','"+netSalary+"','" + monthName + "','" + year + "',getdate())", con);

          
               
              
                cmd.ExecuteNonQuery();



                SqlCommand cmdadv = new SqlCommand("update emplist set advanceamount=advanceamount-" + advance + " where empcode='" + empCode + "'", con);
                cmdadv.ExecuteNonQuery();
            }

            lblMessage.Text = "Salaries saved successfully!";
        }

    }
    protected void btnSalary_Click(object sender, EventArgs e)
    {
        LoadData();
    }
}