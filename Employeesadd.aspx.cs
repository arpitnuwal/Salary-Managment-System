using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class Employeesadd : System.Web.UI.Page
{
    string connStr = ConfigurationManager.ConnectionStrings["db"].ConnectionString;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack) { LoadData(); }
     
    }

    // SAVE (Insert / Update)
    protected void btnSave_Click(object sender, EventArgs e)
    {
        int id = Convert.ToInt32(hdnEditID.Value);
        string code = txtEmpCode.Text.Trim();
        string name = txtEmpName.Text.Trim();
        decimal salary = Convert.ToDecimal(txtEmpSalary.Text.Trim());

        SqlConnection con = new SqlConnection(connStr);
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = con;

        string esi="",sundayamountgive="",gender="";
        if(rdESIyes.Checked==true) {  esi="1"; } else{ esi="0"; }

         if(rdsundayyes.Checked==true) {  sundayamountgive="1"; } else{ sundayamountgive="0"; }

         if(rdgenderyes.Checked==true) {  gender="M"; } else{ gender="F"; }
        
        if (id == 0)
        {
            cmd.CommandText = @"
    INSERT INTO [emplist]
    (id, empcode, empname, empsalary, esi, sundayamountgive, gender, rts, status, advanceamount, emptype, bankname, bankifsc, bankaccount, folder)
    SELECT
        ISNULL(MAX(id), 0) + 1,
        @EmpCode,
        @EmpName,
        @EmpSalary,
        @Esi,
        @SundayAmountGive,
        @Gender,
        GETDATE(),
        1,
        @AdvanceAmount,
        @EmpType,
        @BankName,
        @BankIfsc,
        @BankAccount,
        @Folder
    FROM [emplist]";

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EmpCode", txtEmpCode.Text);
            cmd.Parameters.AddWithValue("@EmpName", txtEmpName.Text);
            cmd.Parameters.AddWithValue("@EmpSalary", txtEmpSalary.Text);
            cmd.Parameters.AddWithValue("@Esi", esi);
            cmd.Parameters.AddWithValue("@SundayAmountGive", sundayamountgive);
            cmd.Parameters.AddWithValue("@Gender", gender);
            cmd.Parameters.AddWithValue("@AdvanceAmount", txtadvance.Text);
            cmd.Parameters.AddWithValue("@EmpType", txtemptpye.Text);
            cmd.Parameters.AddWithValue("@BankName", txtbankname.Text);
            cmd.Parameters.AddWithValue("@BankIfsc", txtifsc.Text);
            cmd.Parameters.AddWithValue("@BankAccount", txtaccount.Text);
            cmd.Parameters.AddWithValue("@Folder", txtfolder.Text);

           
        
}
        else
        {
            cmd.CommandText = "UPDATE emplist SET folder='"+txtfolder.Text+"',emptype='" + txtemptpye.Text + "',bankname='" + txtbankname.Text + "',bankifsc='" + txtifsc.Text + "',bankaccount='" + txtaccount.Text + "', empcode='" + txtEmpCode.Text + "', EmpName='" + txtEmpName.Text + "', EmpSalary='" + txtEmpSalary.Text + "',esi='" + esi + "',sundayamountgive='" + sundayamountgive + "',gender='" + gender + "',advanceamount='" + txtadvance.Text + "' WHERE id=" + id + "";
        
        }

       
        con.Open();
        cmd.ExecuteNonQuery();
        con.Close();

        Clear();
        LoadData();
    }

    // DELETE
    protected void lvEmployees_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        

        if (e.CommandName == "DeleteEmp")
        {
            LblId.Value = ((HiddenField)e.Item.FindControl("HdnID")).Value;
            SqlConnection con = new SqlConnection(connStr);
            SqlCommand cmd = new SqlCommand("DELETE FROM emplist WHERE ID=" + LblId.Value + "", con);
          

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            LoadData();
        }
        else if (e.CommandName == "EditEmp")
        {
            LblId.Value = ((HiddenField)e.Item.FindControl("HdnID")).Value;
            hdnEditID.Value = LblId.Value;



            txtbankname.Text = ((Label)e.Item.FindControl("lblbankname")).Text;
            txtemptpye.Text = ((Label)e.Item.FindControl("lblemptype")).Text;
            txtifsc.Text = ((Label)e.Item.FindControl("lblbankifsc")).Text;
            txtaccount.Text = ((Label)e.Item.FindControl("lblbankaccount")).Text;

            txtfolder.Text = ((Label)e.Item.FindControl("lblfolder")).Text;



            txtEmpCode.Text = ((Label)e.Item.FindControl("lblempcode")).Text;
            txtEmpName.Text = ((Label)e.Item.FindControl("lblempname")).Text;
            txtEmpSalary.Text = ((Label)e.Item.FindControl("lblempsalary")).Text;
  txtadvance.Text = ((Label)e.Item.FindControl("lbladvanceamount")).Text;
  string esi = ((Label)e.Item.FindControl("lblesi")).Text;
  string sundayamountgive = ((Label)e.Item.FindControl("lblsundayamountgive")).Text;

  string gender = ((Label)e.Item.FindControl("lbgender")).Text;

  if (esi == "True") { rdESIyes.Checked = true; } else { rdESIno.Checked = true; }
  if (sundayamountgive == "True") { rdsundayyes.Checked = true; } else { rdsundayno.Checked = true; }
  if (gender == "M") { rdgenderyes.Checked = true; } else { rdgenderno.Checked = true; }
        }
    }

    // LOAD DATA
    private void LoadData()
    {
        SqlConnection con = new SqlConnection(connStr);
        SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM [emplist] ORDER BY id  DESC", con);
        DataTable dt = new DataTable();
        da.Fill(dt);

        lvEmployees.DataSource = dt;
        lvEmployees.DataBind();

        lblCount.Text = dt.Rows.Count.ToString();
    }

    // CLEAR FORM
    private void Clear()
    {
        txtEmpCode.Text = "";
        txtEmpName.Text = "";
        txtEmpSalary.Text = "";
        hdnEditID.Value = "0";
    }
}