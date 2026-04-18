using OfficeOpenXml;
using OfficeOpenXml.DataValidation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class addcommission : System.Web.UI.Page
{
    string conStr = ConfigurationManager.ConnectionStrings["db"].ConnectionString;
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


protected void btnSave_Click(object sender, EventArgs e)
{
    try
    {
       // string conStr = "Data Source=mssql2017.adnshost.com,1533;Initial Catalog=testdb;User ID=testdb;Password=testdb@2575";

        using (SqlConnection newcon = new SqlConnection(conStr))
        {
            newcon.Open();

            SqlCommand cmddelete = new SqlCommand(
                "DELETE FROM Commission WHERE YEAR(CreatedDate)=@Year AND MONTH(CreatedDate)=@Month",
                newcon);

            cmddelete.Parameters.AddWithValue("@Year", ddlYear.SelectedValue);
            cmddelete.Parameters.AddWithValue("@Month", ddlMonth.SelectedValue);

            cmddelete.ExecuteNonQuery();
        }

        // File Save
        string path = Server.MapPath("~/commissionUpload/" + FileUpload1.FileName);
        FileUpload1.SaveAs(path);

        DataTable dt = new DataTable();

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using (ExcelPackage package = new ExcelPackage(new FileInfo(path)))
        {
            var ws = package.Workbook.Worksheets.FirstOrDefault();

            if (ws == null)
            {
                Response.Write("❌ Worksheet not found");
                return;
            }

            int totalRows = ws.Dimension.End.Row;
            int totalCols = ws.Dimension.End.Column;

            // Header row
            for (int col = 1; col <= totalCols; col++)
            {
                dt.Columns.Add(ws.Cells[1, col].Text.Trim());
            }

            // Data rows
            for (int row = 2; row <= totalRows; row++)
            {
                DataRow dr = dt.NewRow();

                for (int col = 1; col <= totalCols; col++)
                {
                    dr[col - 1] = ws.Cells[row, col].Text.Trim();
                }

                dt.Rows.Add(dr);
            }
        }

        using (SqlConnection con = new SqlConnection(conStr))
        {
            con.Open();

            foreach (DataRow row in dt.Rows)
            {
               string type = row["Type"] != DBNull.Value ? row["Type"].ToString().Trim() : "";

                if (string.IsNullOrEmpty(type))
                    continue;

                string narration = "";

                if (dt.Columns.Contains("Commission"))
                    narration = row["Commission"].ToString().Trim();
                else if (dt.Columns.Contains("Commission "))
                    narration = row["Commission "].ToString().Trim();

                narration = narration
                    .Replace("\r", "")
                    .Replace("\n", "")
                    .Replace(" ", "");

                if (string.IsNullOrEmpty(narration))
                    continue;

                DateTime createdDate;

                string excelDate = row["Date"] != DBNull.Value
                    ? row["Date"].ToString().Trim()
                    : "";

                if (!DateTime.TryParseExact(
                        excelDate,
                        new string[] { "dd-MM-yyyy", "dd/MM/yyyy", "d-M-yyyy", "d/M/yyyy" },
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out createdDate))
                {
                    continue;
                }


                decimal amount;

                if (!decimal.TryParse(row["Amount"] != DBNull.Value ? row["Amount"].ToString() : "0", out amount))
                    continue;

               


                decimal mrp;

                if (!decimal.TryParse(row["Mrp"] != DBNull.Value ? row["Mrp"].ToString() : "0", out mrp))
                    continue;
                decimal commission = GetCommission(mrp);

                decimal qty;

                if (!decimal.TryParse(row["Qty."] != DBNull.Value ? row["Qty."].ToString() : "0", out qty))
                    continue;
              
                decimal totalcommission = commission * qty;

               
                string[] empcodes = narration.Split(',');

                List<string> cleanCodes = new List<string>();

                foreach (string code in empcodes)
                {
                    string c = code.Trim();

                    if (!string.IsNullOrEmpty(c))
                        cleanCodes.Add(c);
                }

                int count = cleanCodes.Count;

                if (count == 0)
                    continue;

                List<string> folderEmpCodes = new List<string>();
                List<string> normalEmpCodes = new List<string>();

                foreach (string code in cleanCodes)
                {
                    string empcode = code.PadLeft(5, '0');

                    SqlCommand checkCmd = new SqlCommand(
                        "SELECT folder FROM emplist WHERE empcode=@EmpCode", con);

                    checkCmd.Parameters.AddWithValue("@EmpCode", empcode);

                    object result = checkCmd.ExecuteScalar();

                    if (result != null && result.ToString() == "1")
                    {
                        folderEmpCodes.Add(empcode);
                    }
                    else
                    {
                        normalEmpCodes.Add(empcode);
                    }
                }

                decimal folderCommission = 0m;
                decimal remainingCommission = 0m;

                // अगर folder वाले employee हैं तभी 20% cut होगा
                if (folderEmpCodes.Count > 0)
                {
                    folderCommission = totalcommission * 0.20m;
                    remainingCommission = totalcommission - folderCommission;
                }
                else
                {
                    // अगर कोई folder employee नहीं है तो पूरा amount normal को
                    remainingCommission = totalcommission;
                }

                // folder वाले employees में बराबर बाँटो
                decimal folderShare = folderEmpCodes.Count > 0
                    ? folderCommission / folderEmpCodes.Count
                    : 0m;

                // normal employees में बराबर बाँटो
                decimal normalShare = normalEmpCodes.Count > 0
                    ? remainingCommission / normalEmpCodes.Count
                    : 0m;

                // Insert folder employees
                foreach (string empcode in folderEmpCodes)
                {
                    SqlCommand sql = new SqlCommand(
      "INSERT INTO Commission(id, EmpCode, Amount, CommissionAmount, CommissionDate, CreatedDate, sinleamount) " +
      "SELECT ISNULL(MAX(id),0)+1, @EmpCode, @Amount, @CommissionAmount, @CommissionDate, CONVERT(DATETIME,CAST(@Year AS VARCHAR(4)) + '-' + RIGHT('0' + CAST(@Month AS VARCHAR(2)), 2) + '-01'), @SingleAmount " +
      "FROM Commission",
      con);

                    sql.Parameters.AddWithValue("@EmpCode", empcode);
                    sql.Parameters.AddWithValue("@Amount", amount);
                    sql.Parameters.AddWithValue("@CommissionAmount", folderShare);
                    sql.Parameters.AddWithValue("@CommissionDate", createdDate);
                    sql.Parameters.AddWithValue("@Year", ddlYear.SelectedValue);
                    sql.Parameters.AddWithValue("@Month", ddlMonth.SelectedValue);
                    sql.Parameters.AddWithValue("@SingleAmount", mrp);

                    sql.ExecuteNonQuery();
                }

                // Insert normal employees
                foreach (string empcode in normalEmpCodes)
                {
                    SqlCommand sql = new SqlCommand(
     "INSERT INTO Commission(id, EmpCode, Amount, CommissionAmount, CommissionDate, CreatedDate, sinleamount) " +
     "SELECT ISNULL(MAX(id),0)+1, @EmpCode, @Amount, @CommissionAmount, @CommissionDate, CONVERT(DATETIME,CAST(@Year AS VARCHAR(4)) + '-' + RIGHT('0' + CAST(@Month AS VARCHAR(2)), 2) + '-01'), @SingleAmount " +   "FROM Commission",
     con);

                    sql.Parameters.AddWithValue("@EmpCode", empcode);
                    sql.Parameters.AddWithValue("@Amount", amount);
                    sql.Parameters.AddWithValue("@CommissionAmount", normalShare);
                    sql.Parameters.AddWithValue("@CommissionDate", createdDate);
                    sql.Parameters.AddWithValue("@Year", ddlYear.SelectedValue);
                    sql.Parameters.AddWithValue("@Month", ddlMonth.SelectedValue);
                    sql.Parameters.AddWithValue("@SingleAmount", mrp);

                    sql.ExecuteNonQuery();
                }
            }
        }
        Label1.Text = "✅ Commission Imported Successfully";
        Response.Write("✅ Commission Imported Successfully");
    }
    catch (Exception ex)
    {
        Response.Write("❌ Error: " + ex.Message);
    }
}


public decimal GetCommission(decimal amount)
{
    decimal absAmount = Math.Abs(amount);
    decimal commission = 0;

    if (absAmount <= 1000)
        commission = 1;
    else if (absAmount <= 2000)
        commission = 2;
    else if (absAmount <= 3000)
        commission = 3;
    else if (absAmount <= 4000)
        commission = 4;
    else if (absAmount <= 5000)
        commission = 5;
    else if (absAmount <= 6000)
        commission = 6;
    else if (absAmount <= 7000)
        commission = 7;
    else if (absAmount <= 8000)
        commission = 8;
    else if (absAmount <= 9000)
        commission = 9;
    else if (absAmount <= 10000)
        commission = 10;
    else if (absAmount <= 11000)
        commission = 11;
    else if (absAmount <= 12000)
        commission = 12;
    else if (absAmount <= 13000)
        commission = 13;
    else if (absAmount <= 14000)
        commission = 14;
    else if (absAmount <= 15000)
        commission = 15;
    else if (absAmount <= 16000)
        commission = 16;
    else if (absAmount <= 17000)
        commission = 17;
    else if (absAmount <= 18000)
        commission = 18;
    else if (absAmount <= 19000)
        commission = 19;
    else if (absAmount <= 20000)
        commission = 20;

    else if (absAmount <= 21000)
        commission = 21;


    else if (absAmount <= 22000)
        commission = 22;

    else if (absAmount <= 23000)
        commission = 23;

    else if (absAmount <= 24000)
        commission = 24;

    else if (absAmount <= 25000)
        commission = 25;

    else if (absAmount <= 26000)
        commission = 26;

    else if (absAmount <= 27000)
        commission = 27;
    else if (absAmount <= 28000)
        commission = 28;

    else if (absAmount <= 29000)
        commission = 29;

    else if (absAmount <= 30000)
        commission = 30;
    else if (absAmount <= 31000)
        commission = 31;
    else if (absAmount <= 32000)
        commission = 32;
    else if (absAmount <= 33000)
        commission = 33;
    else if (absAmount <= 34000)
        commission = 34;
    else if (absAmount <= 35000)
        commission = 35;
    else if (absAmount <= 36000)
        commission = 36;
    else if (absAmount <= 37000)
        commission = 37;
    else if (absAmount <= 38000)
        commission = 38;
    else if (absAmount <= 39000)
        commission = 39;
    else if (absAmount <= 40000)
        commission = 40;

    else if (absAmount <= 41000)
        commission = 41;
    else if (absAmount <= 42000)
        commission = 42;
    else if (absAmount <= 43000)
        commission = 43;
    else if (absAmount <= 44000)
        commission = 44;
    else if (absAmount <= 45000)
        commission = 45;
    else if (absAmount <= 46000)
        commission = 46;
    else if (absAmount <= 47000)
        commission = 47;
    else if (absAmount <= 48000)
        commission = 48;
    else if (absAmount <= 49000)
        commission = 49;
    else if (absAmount <= 50000)
        commission = 50;
    else if (absAmount <= 51000)
        commission = 51;
    else if (absAmount <= 52000)
        commission = 52;
    else if (absAmount <= 53000)
        commission = 53;
    else if (absAmount <= 54000)
        commission = 54;
    else if (absAmount <= 55000)
        commission = 55;
    else if (absAmount <= 56000)
        commission = 56;
    else if (absAmount <= 57000)
        commission = 57;
    else if (absAmount <= 58000)
        commission = 58;
    else if (absAmount <= 59000)
        commission = 59;
    else if (absAmount <= 60000)
        commission = 60;
    else if (absAmount <= 61000)
        commission = 61;
    else if (absAmount <= 62000)
        commission = 62;
    else if (absAmount <= 63000)
        commission = 63;
    else if (absAmount <= 64000)
        commission = 64;
    else if (absAmount <= 65000)
        commission = 65;
    else if (absAmount <= 66000)
        commission = 66;
    else if (absAmount <= 67000)
        commission = 67;
    else if (absAmount <= 68000)
        commission = 68;
    else if (absAmount <= 69000)
        commission = 69;
    else if (absAmount <= 70000)
        commission = 70;
    else if (absAmount <= 71000)
        commission = 71;
    else if (absAmount <= 72000)
        commission = 72;
    else if (absAmount <= 73000)
        commission = 73;
    else if (absAmount <= 74000)
        commission = 74;
    else if (absAmount <= 75000)
        commission = 75;
    else if (absAmount <= 76000)
        commission = 76;
    else if (absAmount <= 77000)
        commission = 77;
    else if (absAmount <= 78000)
        commission = 78;
    else if (absAmount <= 79000)
        commission = 79;
    else if (absAmount <= 80000)
        commission = 80;
    else if (absAmount <= 81000)
        commission = 81;
    else if (absAmount <= 82000)
        commission = 82;
    else if (absAmount <= 83000)
        commission = 83;
    else if (absAmount <= 84000)
        commission = 84;
    else if (absAmount <= 85000)
        commission = 85;
    else if (absAmount <= 86000)
        commission = 86;
    else if (absAmount <= 87000)
        commission = 87;
    else if (absAmount <= 88000)
        commission = 88;
    else if (absAmount <= 89000)
        commission = 89;
    else if (absAmount <= 90000)
        commission = 90;
    else if (absAmount <= 91000)
        commission = 91;
    else if (absAmount <= 92000)
        commission = 92;
    else if (absAmount <= 93000)
        commission = 93;
    else if (absAmount <= 94000)
        commission = 94;
    else if (absAmount <= 95000)
        commission = 95;
    else if (absAmount <= 96000)
        commission = 96;
    else if (absAmount <= 97000)
        commission = 97;
    else if (absAmount <= 98000)
        commission = 98;
    else if (absAmount <= 99000)
        commission = 99;
    else if (absAmount <= 100000)
        commission = 100;

    return amount < 0 ? -commission : commission;
}
}