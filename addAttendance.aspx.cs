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

public partial class addAttendance : System.Web.UI.Page
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
        if (!FileUpload1.HasFile)
        {
            Label1.Text = "Please select Excel file.";
            return;
        }
  
        string path = Server.MapPath("~/AttendanceUploads/" + FileUpload1.FileName);
        FileUpload1.SaveAs(path);

        DataTable dt = new DataTable();

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using (ExcelPackage package = new ExcelPackage(new FileInfo(path)))
        {
            Response.Write("Sheet Count = " + package.Workbook.Worksheets.Count + "<br>");

            foreach (var s in package.Workbook.Worksheets)
            {
                Response.Write("Sheet Name = " + s.Name + "<br>");
            }

            if (package.Workbook.Worksheets.Count == 0)
            {
                Label1.Text = "No worksheet found";
                return;
            }

            ExcelWorksheet ws = package.Workbook.Worksheets.First();

            int totalRows = ws.Dimension.End.Row;
            int totalCols = ws.Dimension.End.Column;
        }
        using (ExcelPackage package = new ExcelPackage(new FileInfo(path)))
        {
            ExcelWorksheet ws = package.Workbook.Worksheets.FirstOrDefault();

            int totalRows = ws.Dimension.End.Row;
            int totalCols = ws.Dimension.End.Column;

            for (int col = 1; col <= totalCols; col++)
            {
                dt.Columns.Add("Col" + col);
            }

            for (int row = 1; row <= totalRows; row++)
            {
                DataRow dr = dt.NewRow();

                for (int col = 1; col <= totalCols; col++)
                {
                    dr[col - 1] = ws.Cells[row, col].Text;
                }

                dt.Rows.Add(dr);
            }
        }

        using (SqlConnection sql = new SqlConnection("Data Source=mssql2017.adnshost.com,1533;Initial Catalog=testdb;User ID=testdb;Password=testdb@2575"))
        {
            sql.Open();

            SqlCommand cmddelete = new SqlCommand(
                "DELETE FROM Attendance WHERE YEAR(rts)=@y AND MONTH(rts)=@m", sql);

            cmddelete.Parameters.AddWithValue("@y", ddlYear.SelectedValue);
            cmddelete.Parameters.AddWithValue("@m", ddlMonth.SelectedValue);
            cmddelete.ExecuteNonQuery();

            string empCode = "";
            string empName = "";

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    string val = dt.Rows[i][c].ToString().Trim();

                    if (val.Equals("Empcode", StringComparison.OrdinalIgnoreCase))
                    {
                        empCode = dt.Rows[i][c + 1].ToString().Trim();
                        empName = dt.Rows[i][c + 3].ToString().Trim();
                    }
                }

                string dateCell = dt.Rows[i][0].ToString().Trim();

                DateTime d;
                double oa;

                if (double.TryParse(dateCell, out oa))
                {
                    d = DateTime.FromOADate(oa);
                }
                else if (!DateTime.TryParseExact(
                    dateCell,
                    new string[] { "dd/MM/yyyy", "d/M/yyyy", "MM/dd/yyyy" },
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out d))
                {
                    continue;
                }

                if (string.IsNullOrEmpty(empCode))
                    continue;

                string shift = dt.Rows[i][1].ToString().Trim();
                string inTime = dt.Rows[i][2].ToString().Trim();

                string outTime = dt.Columns.Count > 17 ? dt.Rows[i][17].ToString().Trim() : "";
                string breakTime = dt.Columns.Count > 20 ? dt.Rows[i][20].ToString().Trim() : "";

                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Attendance(EmpCode,AttDate,Shift,InTime,OutTime,BreakTime,rts) " +
                    "VALUES(@EmpCode,@Date,@Shift,@In,@Out,@Break,DATEFROMPARTS(@Year,@Month,1))", sql);

                cmd.Parameters.AddWithValue("@EmpCode", empCode);
                cmd.Parameters.AddWithValue("@Date", d);
                cmd.Parameters.AddWithValue("@Shift", shift);
                cmd.Parameters.AddWithValue("@In", inTime);
                cmd.Parameters.AddWithValue("@Out", outTime);
                cmd.Parameters.AddWithValue("@Break", breakTime);
                cmd.Parameters.AddWithValue("@Year", ddlYear.SelectedValue);
                cmd.Parameters.AddWithValue("@Month", ddlMonth.SelectedValue);

                cmd.ExecuteNonQuery();
            }
        }

        Label1.Text = "Excel Import Successful";
    }
}