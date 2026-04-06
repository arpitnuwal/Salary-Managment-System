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
        string conStr = "Data Source=mssql2017.adnshost.com,1533;Initial Catalog=testdb;User ID=testdb;Password=testdb@2575";

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

                if (!decimal.TryParse(row["Qty"] != DBNull.Value ? row["Qty"].ToString() : "0", out qty))
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

                decimal dividedCommission = totalcommission / count;

                foreach (string code in cleanCodes)
                {
                    string empcode = code.PadLeft(4, '0');

                    SqlCommand sql = new SqlCommand(
                        "INSERT INTO Commission(EmpCode, Amount, CommissionAmount,CommissionDate, CreatedDate,sinleamount) " +
                        "VALUES(@EmpCode, @Amount, @CommissionAmount,'" + createdDate + "', DATEFROMPARTS(@Year,@Month,1),'" + mrp + "')",
                        con);

                    sql.Parameters.AddWithValue("@EmpCode", empcode);
                    sql.Parameters.AddWithValue("@Amount", amount);
                    sql.Parameters.AddWithValue("@CommissionAmount", dividedCommission);
                    sql.Parameters.AddWithValue("@Year", ddlYear.SelectedValue);
                    sql.Parameters.AddWithValue("@Month", ddlMonth.SelectedValue);

                    sql.ExecuteNonQuery();
                }
            }
        }

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

    return amount < 0 ? -commission : commission;
}
}