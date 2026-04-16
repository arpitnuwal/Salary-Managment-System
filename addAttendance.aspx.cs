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
    if (!FileUpload1.HasFile)
    {
        Label1.Text = "Please select Excel file.";
        return;
    }

    try
    {
        string folderPath = Server.MapPath("~/AttendanceUploads/");
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string filePath = Path.Combine(folderPath, FileUpload1.FileName);
        FileUpload1.SaveAs(filePath);

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using (SqlConnection sql = new SqlConnection(conStr))
        {
            sql.Open();

            // Purana month data delete
            SqlCommand deleteCmd = new SqlCommand(
                "DELETE FROM Attendance WHERE YEAR(rts)=@Year AND MONTH(rts)=@Month", sql);

            deleteCmd.Parameters.AddWithValue("@Year", Convert.ToInt32(ddlYear.SelectedValue));
            deleteCmd.Parameters.AddWithValue("@Month", Convert.ToInt32(ddlMonth.SelectedValue));
            deleteCmd.ExecuteNonQuery();

            using (ExcelPackage package = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelWorksheet ws = package.Workbook.Worksheets.FirstOrDefault();

                if (ws == null)
                {
                    Label1.Text = "No worksheet found.";
                    return;
                }

                int totalRows = ws.Dimension.End.Row;
                int totalCols = ws.Dimension.End.Column;

                string currentEmpCode = "";
                string currentEmpName = "";

                for (int row = 1; row <= totalRows; row++)
                {
                    // Employee details detect
                    for (int col = 1; col <= totalCols; col++)
                    {
                        string cellText = ws.Cells[row, col].Text.Trim();

                        if (cellText.ToLower().Contains("emp"))
                        {
                            for (int nextCol = col + 1; nextCol <= col + 3 && nextCol <= totalCols; nextCol++)
                            {
                                string val = ws.Cells[row, nextCol].Text.Trim();
                                if (!string.IsNullOrWhiteSpace(val))
                                {
                                    currentEmpCode = val;
                                    break;
                                }
                            }
                        }

                        if (cellText.ToLower().Contains("name"))
                        {
                            for (int nextCol = col + 1; nextCol <= col + 5 && nextCol <= totalCols; nextCol++)
                            {
                                string val = ws.Cells[row, nextCol].Text.Trim();
                                if (!string.IsNullOrWhiteSpace(val))
                                {
                                    currentEmpName = val;
                                    break;
                                }
                            }
                        }
                    }

                    // Date read
                    string dateText = ws.Cells[row, 1].Text.Trim();

                    if (string.IsNullOrWhiteSpace(dateText))
                        continue;

                    DateTime attDate;
                    double oaDate;

                    if (double.TryParse(dateText, out oaDate))
                    {
                        attDate = DateTime.FromOADate(oaDate);
                    }
                    else if (!DateTime.TryParseExact(
                        dateText,
                        new string[] { "dd/MM/yyyy", "d/M/yyyy", "MM/dd/yyyy" },
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out attDate))
                    {
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(currentEmpCode))
                        continue;

                    // Column mapping
                    string shift = totalCols >= 3 ? ws.Cells[row, 3].Text.Trim() : "";
                    string inTime = totalCols >= 4 ? ws.Cells[row, 4].Text.Trim() : "";
                    string outTime = totalCols >= 19 ? ws.Cells[row, 19].Text.Trim() : "";

                    // Break calculation
                    TimeSpan totalBreak = TimeSpan.Zero;

                    // Out1 -> In2 -> Out2 -> In3 ...
                    for (int col = 5; col < totalCols; col += 2)
                    {
                        string prevOut = ws.Cells[row, col].Text.Trim();       // Out1, Out2
                        string nextIn = ws.Cells[row, col + 1].Text.Trim();    // In2, In3

                        DateTime outDT, inDT;

                        if (DateTime.TryParse(prevOut, out outDT) &&
                            DateTime.TryParse(nextIn, out inDT))
                        {
                            if (inDT > outDT)
                            {
                                totalBreak += (inDT - outDT);
                            }
                        }
                    }

                    string breakTime = totalBreak.ToString(@"hh\:mm");

                    SqlCommand insertCmd = new SqlCommand(
      @"INSERT INTO Attendance
      (id, EmpCode, AttDate, Shift, InTime, OutTime, BreakTime, rts)
      SELECT 
          ISNULL(MAX(id),0) + 1,
          @EmpCode,
          @AttDate,
          @Shift,
          @InTime,
          @OutTime,
          @BreakTime,
          DATEFROMPARTS(@Year, @Month, 1)
      FROM Attendance", sql);

                    insertCmd.Parameters.AddWithValue("@EmpCode", currentEmpCode);
                    insertCmd.Parameters.AddWithValue("@AttDate", attDate);
                    insertCmd.Parameters.AddWithValue("@Shift", shift);
                    insertCmd.Parameters.AddWithValue("@InTime", inTime);
                    insertCmd.Parameters.AddWithValue("@OutTime", outTime);
                    insertCmd.Parameters.AddWithValue("@BreakTime", breakTime);
                    insertCmd.Parameters.AddWithValue("@Year", Convert.ToInt32(ddlYear.SelectedValue));
                    insertCmd.Parameters.AddWithValue("@Month", Convert.ToInt32(ddlMonth.SelectedValue));

                    insertCmd.ExecuteNonQuery();
                }
            }
        }

        Label1.Text = "Excel Import Successful";
    }
    catch (Exception ex)
    {
        Label1.Text = "Error: " + ex.Message;
    }
}






}