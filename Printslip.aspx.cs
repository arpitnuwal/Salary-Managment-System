using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Printslip : System.Web.UI.Page
{
    string connStr = ConfigurationManager.ConnectionStrings["db"].ConnectionString;
    protected void Page_Load(object sender, EventArgs e)
    {
        LoadData();
    }

    private void LoadData()
    {
        string empid = "";

        empid = "and empcode in (" + Request.QueryString["EmpCode"] + ")";

        
        SqlConnection con = new SqlConnection(connStr);
        string sqlq = "select a.*,(select emptype from emplist where empcode=a.empcode) as emptype,(select bankname from emplist where empcode=a.empcode) as bankname,(select bankifsc from emplist where empcode=a.empcode) as bankifsc,(select bankaccount from emplist where empcode=a.empcode) as bankaccount from finalsalaryreport   as a  where a.reportmonth='" + Request.QueryString["month"] + "' and	a.reportyear='" + Request.QueryString["year"] + "' and a.empcode in (select empcode from  emplist where  1=1  " + empid + " )";
        SqlDataAdapter da = new SqlDataAdapter(sqlq, con);
        DataTable dt = new DataTable();
        da.Fill(dt);


        lblname.Text=dt.Rows[0]["name"].ToString();
        lbladvacneamt.Text = dt.Rows[0]["advanceamountcut"].ToString();

        lblbasicpay.Text = dt.Rows[0]["basicsalary"].ToString();

        lbldue.Text = dt.Rows[0]["dueadvance"].ToString();

        lblextracomm.Text = dt.Rows[0]["extracommsion"].ToString();

        lblgrosssalary.Text = dt.Rows[0]["grosssalary"].ToString();

        lbllaeve.Text = dt.Rows[0]["deductionday"].ToString();

        lblmmonthname.Text = dt.Rows[0]["reportmonth"].ToString();

        lblesi.Text = Math.Round(Convert.ToDecimal(dt.Rows[0]["esicut"]), 0).ToString();
      

      //  lblnetsalary2.Text = dt.Rows[0]["NetSalary"].ToString();
     
        lblsundayamount.Text = dt.Rows[0]["sundayamountextra"].ToString();
        lblsundaycount.Text = dt.Rows[0]["sunday"].ToString();



        lbltotalcomm.Text = dt.Rows[0]["Commission"].ToString();

        lblsalaryadv.Text = dt.Rows[0]["ExtraAmountLess"].ToString();
      
        lbltea.Text = dt.Rows[0]["tea"].ToString();
    
      //lbltotaldesduct  .Text = dt.Rows[0]["name"].ToString();


        lbltotalextra.Text = dt.Rows[0]["grosssalary"].ToString();

     

        lblyearname.Text = dt.Rows[0]["reportyear"].ToString();

        int adv = 0, saladv = 0, esi = 0;

        int.TryParse(lbladvacneamt.Text, out adv);
        int.TryParse(lblsalaryadv.Text, out saladv);
        int.TryParse(lblesi.Text, out esi);

        lbltotaldesduct.Text = (adv + saladv + esi).ToString();
        lblnetsalary.Text = (Convert.ToInt32(lblgrosssalary.Text) - Convert.ToInt32(lbltotaldesduct.Text)).ToString();
    }
}