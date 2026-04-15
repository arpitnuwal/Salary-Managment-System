using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
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

    private bool IsAbsentDay(string empCode, DateTime checkDate)
    {
        using (SqlConnection con = new SqlConnection(
            @"Data Source=mssql2017.adnshost.com,1533;Initial Catalog=testdb;User ID=testdb;Password=testdb@2575"))
        {
            con.Open();
            string sql = "SELECT COUNT(*) FROM Attendance "
                + "WHERE EmpCode="+empCode+" " +
                "AND CAST(AttDate AS DATE)='" + checkDate.Date + "' " +
                "AND InTime IS NOT NULL " +
                "AND LTRIM(RTRIM(InTime)) <> '' " +
                "AND LTRIM(RTRIM(InTime)) <> '--:--'";
            SqlCommand cmd = new SqlCommand(sql , con);

         

            int count = Convert.ToInt32(cmd.ExecuteScalar());

            return count == 0;
        }
    }

    protected void btnSalary_Click(object sender, EventArgs e)
    { 
        pnlError.Visible = false;
        if (ddlMonth.SelectedValue == "")
        {
            lblError.Text = "Select Month";

            pnlError.Visible = true;
            return;
        
        }

        if (ddlYear.SelectedValue == "")
        {
            lblError.Text = "Select Year"; pnlError.Visible = true; return;

        }

        string empid = "",empid2 = "";
        if (txtempid.Text != "")
        {
            empid = "and empcode in (" + txtempid.Text + ")";
            empid2 = "and emcode in (" + txtempid.Text + ")";

        }
        SqlConnection con = new SqlConnection("Data Source=mssql2017.adnshost.com,1533;Initial Catalog=testdb;User ID=testdb;Password=testdb@2575");

        con.Open();
        SqlCommand cmddelete = new SqlCommand("delete  from AttendanceDeductionLog   where   YEAR(CreatedOn) = " + ddlYear.SelectedValue + "   AND MONTH(CreatedOn) = " + ddlMonth.SelectedValue + " " + empid + ";delete  from commsioncut   where   YEAR(CreatedOn) = " + ddlYear.SelectedValue + "   AND MONTH(CreatedOn) = " + ddlMonth.SelectedValue + " " + empid2 + ";delete  from SundayAttendanceLog   where   YEAR(CreatedOn) = " + ddlYear.SelectedValue + "   AND MONTH(CreatedOn) = " + ddlMonth.SelectedValue + " " + empid + "", con);
        cmddelete.ExecuteNonQuery();


        string emptype = "";



        DataTable salaryTable = new DataTable();

        salaryTable.Columns.Add("EmpCode");
        salaryTable.Columns.Add("EmpName");
        salaryTable.Columns.Add("Salary"); // NEW
        salaryTable.Columns.Add("ESI"); // NEW
        salaryTable.Columns.Add("SundayAmount"); // NEW
        salaryTable.Columns.Add("Gender"); // NEW
        salaryTable.Columns.Add("Advance"); // NEW
        salaryTable.Columns.Add("ESICut"); // NEW
        salaryTable.Columns.Add("DeductionDays");
        salaryTable.Columns.Add("SundayWorked");
        salaryTable.Columns.Add("Commission");
        salaryTable.Columns.Add("gsalary");

        salaryTable.Columns.Add("FinalSalary");


        salaryTable.Columns.Add("year");
        salaryTable.Columns.Add("month");
        salaryTable.Columns.Add("basicsalary");

        //where EmpCode='0007' 



        SqlCommand empCmd = new SqlCommand("SELECT emptype,ID,empcode,empname,empsalary,esi,sundayamountgive,gender,rts,status,advanceamount FROM [emplist]  where 1=1   " + empid + "    ORDER BY id  DESC", con);
        SqlDataReader empDr = empCmd.ExecuteReader();
        //  
        List<dynamic> employees = new List<dynamic>();

        while (empDr.Read())
        {
            employees.Add(new
            {
                EmpCode = empDr["empcode"].ToString().Trim(),
                EmpName = empDr["empname"].ToString(),
                Salary = Convert.ToDouble(empDr["empsalary"]),
                ESI = empDr["esi"].ToString(),
                SundayAmount =  empDr["sundayamountgive"].ToString(),
                Gender = empDr["gender"].ToString(),
                Advance = Convert.ToDouble(empDr["advanceamount"]),
                SundayAllowed = empDr["sundayamountgive"].ToString(),
                emptypehe = empDr["emptype"].ToString()
            });
        }



       
        empDr.Close();
        Dictionary<DateTime, bool> officeClosedDayCache = new Dictionary<DateTime, bool>();
       // ट्रेन में जोम्बी का आतंक 
        foreach (var emp in employees)
        {
            string empCode = emp.EmpCode;
            string empName = emp.EmpName;
            string gender = emp.Gender;
            double monthlySalary = emp.Salary; // 🔥 Dynamic salary
            double sundayRate = monthlySalary/30;
            double advance = emp.Advance;
            emptype = emp.emptypehe;
            SqlCommand cmd = new SqlCommand("select * from Attendance where EmpCode=@EmpCode and  YEAR(rts) = " + ddlYear.SelectedValue + "   AND MONTH(rts) = " + ddlMonth.SelectedValue + ";", con);
            cmd.Parameters.AddWithValue("@EmpCode", empCode);

            SqlDataReader dr = cmd.ExecuteReader();

            double totalDeductionDays = 0;
            int sundayWorked = 0;
            int daysInMonth = 30;

            int lateDays = 0;

            double totalcommissiondaily = 0;
            

            while (dr.Read())
            {
                double holiday = 0;
                string chkdate = dr["AttDate"].ToString();
                DateTime attDate;
                DateTime.TryParse(dr["AttDate"].ToString(), out attDate);

                string intime = dr["InTime"].ToString();
                string outtime = dr["OutTime"].ToString();
                string breaktime = dr["BreakTime"].ToString();


                // ===== FAST OFFICE CLOSED DAY CHECK =====
                bool isOfficeClosedDay = false;

                if (!officeClosedDayCache.ContainsKey(attDate.Date))
                {
                    using (SqlConnection conCheck = new SqlConnection(
                        @"Data Source=mssql2017.adnshost.com,1533;Initial Catalog=testdb;User ID=testdb;Password=testdb@2575"))
                    {
                        conCheck.Open();

                        SqlCommand cmdCheck = new SqlCommand(
                            "SELECT COUNT(*) FROM Attendance " +
                            "WHERE CAST(AttDate AS DATE)=@Date " +
                            "AND InTime IS NOT NULL " +
                            "AND LTRIM(RTRIM(InTime)) <> '' " +
                            "AND LTRIM(RTRIM(InTime)) <> '--:--'",
                            conCheck);

                        cmdCheck.Parameters.AddWithValue("@Date", attDate.Date);

                        int totalPunch = Convert.ToInt32(cmdCheck.ExecuteScalar());

                        isOfficeClosedDay = (totalPunch == 0);

                        // Cache save
                        officeClosedDayCache.Add(attDate.Date, isOfficeClosedDay);
                    }
                }
                else
                {
                    isOfficeClosedDay = officeClosedDayCache[attDate.Date];
                }

                // skip deduction
                if (isOfficeClosedDay)
                {
                    DateTime prevDay = attDate.AddDays(-1);
                    DateTime nextDay = attDate.AddDays(1);

                    bool isPrevLeave = IsAbsentDay(empCode, prevDay);
                    bool isNextLeave = IsAbsentDay(empCode, nextDay);

                    bool isSandwichHoliday = isPrevLeave && isNextLeave;

                    if (isSandwichHoliday)
                    {
                        holiday += 1;   // holiday day salary cut
                        //yha dekhna he
                        string inTimeValue = intime.ToString();
                        string outTimeValue = outtime.ToString();
                        string breakTimeValue = breaktime.ToString();
                        using (SqlConnection conInsert = new SqlConnection(
                            @"Data Source=mssql2017.adnshost.com,1533;Initial Catalog=testdb;User ID=testdb;Password=testdb@2575"))
                        {
                            conInsert.Open();

                            SqlCommand cmdextra = new SqlCommand(
                                "INSERT INTO AttendanceDeductionLog " +
                                "(EmpCode, AttDate, InTime, OutTime, BreakTime, Deduction, DeductionType, Reason, CreatedOn) " +
                                "VALUES ('" + empCode + "', '" + attDate + "', '" + inTimeValue + "', '" + outTimeValue + "', '" + breakTimeValue + "', '" + holiday + "', 'Full Day', 'SandwichHoliday  cut', DATEFROMPARTS("+ddlYear.SelectedValue+","+ddlMonth.SelectedValue+",1))",
                                conInsert);

                        
                           
                          

                            cmdextra.ExecuteNonQuery();
                        }


                    }

                    totalDeductionDays += holiday;
                    continue;
                }


                TimeSpan minInTime = new TimeSpan(0, 0, 0);
                using (SqlConnection conMin = new SqlConnection("Data Source=mssql2017.adnshost.com,1533;Initial Catalog=testdb;User ID=testdb;Password=testdb@2575"))
                {
                    conMin.Open();

                    SqlCommand cmdMin = new SqlCommand(
                        "SELECT MIN(InTime) FROM Attendance WHERE CAST(AttDate  AS DATE)=@Date  and EmpCode in (select empcode from  [emplist] where emptype='"+emptype+"') AND InTime IS NOT NULL",
                        conMin);

                    cmdMin.Parameters.AddWithValue("@Date", attDate.Date);

                    object result = cmdMin.ExecuteScalar();

                    if (result != DBNull.Value)
                    {
                        TimeSpan.TryParse(result.ToString(), out minInTime);
                    }
                }
                bool isLateOpenDay = false;

                // अगर minimum entry ही 2:30 या 3 बजे के बाद है
                if (minInTime > new TimeSpan(14, 30, 0))
                {
                    isLateOpenDay = true;
                    if (isLateOpenDay == true)
                    {
                        SqlCommand cmdlateopen = new SqlCommand("insert into LateOpenDay values ('" + chkdate + "',DATEFROMPARTS("+ddlYear.SelectedValue+","+ddlMonth.SelectedValue+",1))", con);
                        cmdlateopen.ExecuteNonQuery();

                    }

                }

          

                string chkdateggggggg = dr["AttDate"].ToString();
                TimeSpan inTime, outTime, breakTime;

                bool inOk = TimeSpan.TryParse(intime, out inTime);
                bool outOk = TimeSpan.TryParse(outtime, out outTime);
                bool breakOk = TimeSpan.TryParse(breaktime, out breakTime);

                string isFemale = "F";

                string gggg = chkdate;

                string sundayremark = "";
                // Sunday rule
                if (attDate.DayOfWeek == DayOfWeek.Sunday)
                {


                    if (inOk)
                    {




                     

                        string inTimeValue = inOk ? inTime.ToString() : "LEAVE";
                        string outTimeValue = outOk ? outTime.ToString() : "";
                        string breakTimeValue = breakOk ? breakTime.ToString() : "";



                        DateTime prevDay = attDate.AddDays(-1); // Saturday
                        DateTime nextDay = attDate.AddDays(1);  // Monday

                        bool isPrevLeave = IsAbsentDay(empCode, prevDay);
                        bool isNextLeave = IsAbsentDay(empCode, nextDay);

                        bool isSandwichLeave = isPrevLeave && isNextLeave;

                        if (isSandwichLeave)
                        {
                            if (inOk)
                            {
                                sundayWorked++;
                            }
                            else
                            {
                                sundayremark = "SandwichLeave Apply";
                                // Sunday amount bhi cut
                                holiday += 1;
                            }
                        }

                        else
                        {


                            TimeSpan halfDayTime = new TimeSpan(14, 0, 0); // 2 PM
                            TimeSpan noCutTime = new TimeSpan(17, 0, 0);   // 5.30 PM

                            if (inOk && outOk)
                            {
                                sundayWorked++;

                                if (outTime < halfDayTime)
                                {
                                    // 2 PM se pehle gaya = Full cut
                                    holiday += 1;
                                }
                                else if (outTime >= halfDayTime && outTime < noCutTime)
                                {
                                    // 2 PM ke baad aur 7 PM se pehle = Half cut
                                    holiday += 0.50;
                                }
                                else
                                {
                                    // 7 PM ya uske baad = No cut
                                }
                            }
                            else
                            {
                                // Aaya hi nahi = Full cut
                                holiday += 1;
                            }
                        }

                        using (SqlConnection conSunday = new SqlConnection(
                            @"Data Source=mssql2017.adnshost.com,1533;Initial Catalog=testdb;User ID=testdb;Password=testdb@2575"))
                        {
                            conSunday.Open();

                            SqlCommand cmdSunday = new SqlCommand(
                                "INSERT INTO SundayAttendanceLog " +
                                "(EmpCode, AttDate, InTime, OutTime, BreakTime, CreatedOn,remark) " +
                                "VALUES (@EmpCode, @AttDate, @InTime, @OutTime, @BreakTime, DATEFROMPARTS(@Year,@Month,1),'" + sundayremark + "')",
                                conSunday);

                            cmdSunday.Parameters.AddWithValue("@EmpCode", empCode);
                            cmdSunday.Parameters.AddWithValue("@AttDate", attDate);
                            cmdSunday.Parameters.AddWithValue("@InTime", inTimeValue);
                            cmdSunday.Parameters.AddWithValue("@OutTime", outTimeValue);
                            cmdSunday.Parameters.AddWithValue("@BreakTime", breakTimeValue);
                            cmdSunday.Parameters.AddWithValue("@Year", ddlYear.SelectedValue);
                            cmdSunday.Parameters.AddWithValue("@Month", ddlMonth.SelectedValue);
                            cmdSunday.ExecuteNonQuery();
                        }
                    }

                    else
                    {
                        string inTimeValuesunday = inOk ? inTime.ToString() : "LEAVE";
                        string outTimeValuesunday = outOk ? outTime.ToString() : "";
                        string breakTimeValusundaye = breakOk ? breakTime.ToString() : "";
                       
                        if (emp.SundayAmount.ToString().ToLower() == "true" || emp.SundayAmount.ToString() == "1")
                        {
                            DateTime prevDay = attDate.AddDays(-1); // Saturday
                            DateTime nextDay = attDate.AddDays(1);  // Monday
                            bool isPrevLeave = IsAbsentDay(empCode, prevDay);
                            bool isNextLeave = IsAbsentDay(empCode, nextDay);

                            bool isSandwichLeave = isPrevLeave && isNextLeave;

                            if (isSandwichLeave)
                            {
                                if (inOk)
                                {

                                }
                                else
                                {
                                    sundayremark = "SandwichLeave Apply";
                                    // Sunday amount bhi cut
                                    holiday += 1;
                                }
                            }



                            //


                            if (sundayremark != "SandwichLeave Apply")
                            {
                                string inTimeValue = inOk ? inTime.ToString() : "";
                                string outTimeValue = outOk ? outTime.ToString() : "";
                                string breakTimeValue = breakOk ? breakTime.ToString() : "";

                                using (SqlConnection conInsert = new SqlConnection(
                                    @"Data Source=mssql2017.adnshost.com,1533;Initial Catalog=testdb;User ID=testdb;Password=testdb@2575"))
                                {
                                    conInsert.Open();

                                    SqlCommand cmdextra = new SqlCommand(
                                        "INSERT INTO AttendanceDeductionLog " +
                                        "(EmpCode, AttDate, InTime, OutTime, BreakTime, Deduction, DeductionType, Reason, CreatedOn) " +
                                        "VALUES (@EmpCode, @AttDate, @InTime, @OutTime, @BreakTime, @Deduction, @DeductionType, @Reason, DATEFROMPARTS(@Year,@Month,1))",
                                        conInsert);

                                    cmdextra.Parameters.AddWithValue("@EmpCode", empCode);
                                    cmdextra.Parameters.AddWithValue("@AttDate", attDate);
                                    cmdextra.Parameters.AddWithValue("@InTime", inTimeValue);
                                    cmdextra.Parameters.AddWithValue("@OutTime", outTimeValue);
                                    cmdextra.Parameters.AddWithValue("@BreakTime", breakTimeValue);
                                    cmdextra.Parameters.AddWithValue("@Deduction", holiday);
                                    cmdextra.Parameters.AddWithValue("@DeductionType", "Sunday Leave");
                                    cmdextra.Parameters.AddWithValue("@Reason", sundayremark);
                                    cmdextra.Parameters.AddWithValue("@Year", ddlYear.SelectedValue);
                                    cmdextra.Parameters.AddWithValue("@Month", ddlMonth.SelectedValue);

                                    cmdextra.ExecuteNonQuery();
                                }
                                totalDeductionDays += holiday;
                            }


                }

               
                            //
                        

                        else

                        {
                            sundayremark = "Sunday Leave";
                            using (SqlConnection conSunday = new SqlConnection(
                              @"Data Source=mssql2017.adnshost.com,1533;Initial Catalog=testdb;User ID=testdb;Password=testdb@2575"))
                            {
                                conSunday.Open();

                                SqlCommand cmdSunday = new SqlCommand(
                                    "INSERT INTO SundayAttendanceLog " +
                                    "(EmpCode, AttDate, InTime, OutTime, BreakTime, CreatedOn,remark) " +
                                    "VALUES (@EmpCode, @AttDate, @InTime, @OutTime, @BreakTime, DATEFROMPARTS(@Year,@Month,1),'" + sundayremark + "')",
                                    conSunday);

                                cmdSunday.Parameters.AddWithValue("@EmpCode", empCode);
                                cmdSunday.Parameters.AddWithValue("@AttDate", attDate);
                                cmdSunday.Parameters.AddWithValue("@InTime", inTimeValuesunday);
                                cmdSunday.Parameters.AddWithValue("@OutTime", outTimeValuesunday);
                                cmdSunday.Parameters.AddWithValue("@BreakTime", breakTimeValusundaye);
                                cmdSunday.Parameters.AddWithValue("@Year", ddlYear.SelectedValue);
                                cmdSunday.Parameters.AddWithValue("@Month", ddlMonth.SelectedValue);
                                cmdSunday.ExecuteNonQuery();


                                holiday += 1;
                            }
                        
                        }


                      
                    }
                    //  sunday me nhi aaya
                }
                else
                {
                    // Full absent
                    if (!inOk)
                    {
                       
                        
                            holiday += 1;
                        
                      
                    }
                    else
                    {
                        if (!isLateOpenDay)
                        {
                            // Late entry rule
                            if (inTime > new TimeSpan(10, 40, 0) &&
                                inTime <= new TimeSpan(12, 15, 0))
                            {
                                holiday += 0.25;
                            }
                            else if (inTime > new TimeSpan(12, 0, 0) &&
                                     inTime <= new TimeSpan(18, 0, 0))
                            {
                                holiday += 0.50;
                            }
                            else if (inTime > new TimeSpan(18, 0, 0))
                            {
                                holiday += 1;
                            }

                            // Out time rules
                            if (outOk)
                            {
                                if (outTime < new TimeSpan(12, 0, 0))
                                {
                                    holiday += 1;
                                }
                                else if (outTime >= new TimeSpan(12, 0, 0) &&
                                         outTime < new TimeSpan(15, 0, 0))
                                {
                                    holiday += 0.75;
                                }
                                else if (outTime >= new TimeSpan(15, 0, 0) &&
                                         outTime < new TimeSpan(17, 30, 0))
                                {
                                    holiday += 0.50;
                                }

                                else if (outTime >= new TimeSpan(17, 30, 0) &&
           outTime < (isFemale.ToLower() == "f"
                       ? new TimeSpan(19, 0, 0)
                       : new TimeSpan(20, 0, 0)))
                                {
                                    holiday += 0.25;
                                }
                            }

                            // Break time rules
                            if (breakOk)
                            {
                                if (breakTime > new TimeSpan(2, 05, 0))
                                {
                                    holiday += 0.25;
                                }
                            }
                        }
                    }
                }
                //
                TimeSpan firstPunchTime = TimeSpan.Zero;
                TimeSpan lateLimit = TimeSpan.Zero;

                using (SqlConnection conlateee = new SqlConnection(@"Data Source=mssql2017.adnshost.com,1533;Initial Catalog=testdb;User ID=testdb;Password=testdb@2575"))
                {
                    conlateee.Open();
                    string sql = @"";
                    if (emptype == "1")
                    {
                        sql = @"SELECT MIN(InTime) FROM Attendance WHERE CAST(AttDate AS DATE) = '" + chkdate + "'   AND InTime IS NOT NULL   AND LTRIM(RTRIM(InTime)) <> ''   AND LTRIM(RTRIM(InTime)) <> '--:--' and EmpCode in (select empcode from  [emplist] where emptype='1')";
                    }
                    else
                    {

                        sql = @"SELECT MIN(InTime) FROM Attendance WHERE CAST(AttDate AS DATE) = '" + chkdate + "'   AND InTime IS NOT NULL   AND LTRIM(RTRIM(InTime)) <> ''   AND LTRIM(RTRIM(InTime)) <> '--:--' and EmpCode in (select empcode from  [emplist] where emptype='2')";
                    }

                    SqlCommand cmdconlateee = new SqlCommand(sql, conlateee);

                    cmdconlateee.Parameters.AddWithValue("@Date", chkdate);

                    object result = cmdconlateee.ExecuteScalar();

                    if (result != DBNull.Value)
                    {
                        firstPunchTime = TimeSpan.Parse(result.ToString());

                        TimeSpan checkTime = emptype == "1"
                            ? new TimeSpan(10, 18, 0)
                            : new TimeSpan(10, 30, 0);

                        lateLimit = firstPunchTime;

                        if (firstPunchTime > checkTime)
                        {
                            lateLimit = firstPunchTime.Add(TimeSpan.FromMinutes(5));
                        }
                    }
                
                }

                TimeSpan finalLateLimit = new TimeSpan(10, 18, 0);

                if (emptype == "2")
                {
                    finalLateLimit = new TimeSpan(10, 30, 0);
                }

                // Agar first punch fixed late time ke baad hai
                if (firstPunchTime >= finalLateLimit)
                {
                    finalLateLimit = firstPunchTime.Add(new TimeSpan(0, 5, 0));
                }

                Response.Write(holiday);
                if (firstPunchTime >= new TimeSpan(12, 0, 0))
                {
                    finalLateLimit = finalLateLimit.Add(new TimeSpan(0, 30, 0));
                  
                  
                    if(holiday>0){
                        if (holiday == 0.25)
                        {
                            holiday -= 0.25;
                        }
                        else if (holiday == 0.50)
                        {
                            holiday -= 0.50;
                        
                        }
                    }
                    string breakTimeValue = breakOk ? breakTime.ToString(@"hh\:mm") : "";

                    if (breakOk && breakTime >= new TimeSpan(2, 0, 0))
                    {
                        holiday += 0.25;
                    }

                }

                if (attDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    finalLateLimit = new TimeSpan(11, 20, 0);
                }
                if (inOk && inTime > finalLateLimit)
                {
                    lateDays++;
                }
                // Late count
                

                  else
    
                {
                    double commissiondailytesrt = 0; 
                    using (SqlConnection conInsert = new SqlConnection(
                       @"Data Source=mssql2017.adnshost.com,1533;Initial Catalog=testdb;User ID=testdb;Password=testdb@2575"))
                    {
                        conInsert.Open();

                        SqlCommand commissioncmdgtdaily = new SqlCommand(
       "SELECT ISNULL(SUM(CommissionAmount),0) AS FinalTotal FROM Commission " +
       "WHERE      EmpCode=" + empCode +
       " AND CommissionDate='" + chkdate + "'" +
       " AND YEAR(CreatedDate)=" + ddlYear.SelectedValue +
       " AND MONTH(CreatedDate)=" + ddlMonth.SelectedValue,
       conInsert);
                        double commissiondaily = Convert.ToDouble(commissioncmdgtdaily.ExecuteScalar());
                        conInsert.Close();
                        totalcommissiondaily += commissiondaily;
                        commissiondailytesrt = commissiondaily;
                    }

                    using (SqlConnection conInsert = new SqlConnection(
                     @"Data Source=mssql2017.adnshost.com,1533;Initial Catalog=testdb;User ID=testdb;Password=testdb@2575"))
                    {
                        conInsert.Open();

                        SqlCommand commissioncmdgtdailyexcute = new SqlCommand("insert into commsioncut values ('" + empCode + "','" + chkdate + "','" + Convert.ToInt32(commissiondailytesrt) + "','" + inTime + "', DATEFROMPARTS(" + ddlYear.SelectedValue + "," + ddlMonth.SelectedValue + ",1))", conInsert);
                        commissioncmdgtdailyexcute.ExecuteNonQuery();
                        conInsert.Close();
                    }
                }

                // Max deduction = 1 day
                if (holiday > 1)
                    holiday = 1;

                string deductionType = "";
                string reason = "";

                if (holiday == 1)
                {
                    deductionType = "Full Day";
                    reason = "Absent / Very Late / Left Early";
                }
                else if (holiday == 0.75)
                {
                    deductionType = "3/4 Day";
                    reason = "Late + Out Time";
                }
                else if (holiday == 0.5)
                {
                    deductionType = "Half Day";
                    reason = "Late / Break > 3 hrs";
                }
                else if (holiday == 0.25)
                {
                    deductionType = "Quarter Day";
                    reason = "Late / Early Leave / Break";
                }

                if (holiday > 0)
                {
                    string inTimeValue = inOk ? inTime.ToString() : "";
                    string outTimeValue = outOk ? outTime.ToString() : "";
                    string breakTimeValue = breakOk ? breakTime.ToString() : "";

                    DateTime prevDay = attDate.AddDays(-1); // Saturday
                    DateTime nextDay = attDate.AddDays(1);  // Monday
                    bool isPrevLeave = IsAbsentDay(empCode, prevDay);
                    bool isNextLeave = IsAbsentDay(empCode, nextDay);

                    bool isSandwichLeave = isPrevLeave && isNextLeave;

                    if (isSandwichLeave)
                    {
                        if (inOk)
                        {

                        }
                        else
                        {
                            sundayremark = "SandwichLeave Apply";
                            reason = "SandwichLeave Apply";
                        }
                    }

                    
                    using (SqlConnection conInsert = new SqlConnection(
                        @"Data Source=mssql2017.adnshost.com,1533;Initial Catalog=testdb;User ID=testdb;Password=testdb@2575"))
                    {
                        conInsert.Open();

                        SqlCommand cmdextra = new SqlCommand(
                            "INSERT INTO AttendanceDeductionLog " +
                            "(EmpCode, AttDate, InTime, OutTime, BreakTime, Deduction, DeductionType, Reason, CreatedOn) " +
                            "VALUES (@EmpCode, @AttDate, @InTime, @OutTime, @BreakTime, @Deduction, @DeductionType, @Reason, DATEFROMPARTS(@Year,@Month,1))",
                            conInsert);

                        cmdextra.Parameters.AddWithValue("@EmpCode", empCode);
                        cmdextra.Parameters.AddWithValue("@AttDate", attDate);
                        cmdextra.Parameters.AddWithValue("@InTime", inTimeValue);
                        cmdextra.Parameters.AddWithValue("@OutTime", outTimeValue);
                        cmdextra.Parameters.AddWithValue("@BreakTime", breakTimeValue);
                        cmdextra.Parameters.AddWithValue("@Deduction", holiday);
                        cmdextra.Parameters.AddWithValue("@DeductionType", deductionType);
                        cmdextra.Parameters.AddWithValue("@Reason", reason);
                        cmdextra.Parameters.AddWithValue("@Year", ddlYear.SelectedValue);
                        cmdextra.Parameters.AddWithValue("@Month", ddlMonth.SelectedValue);

                        cmdextra.ExecuteNonQuery();
                    }
                }

                totalDeductionDays += holiday;
            }
          
            dr.Close();

            // Commission
           

            

            // Deduct for late days


         
            //

            double perDaySalary = monthlySalary / daysInMonth;
         //   double deductionAmount = 0;
         double deductionAmount = totalDeductionDays * perDaySalary;
            double sundayBonus = sundayWorked * sundayRate;
           // Label1.Text="deductionAmount :" + deductionAmount + " and  monthlySalary: " + monthlySalary + "  sundayBonus: " + sundayBonus + "   perDaySalary : " + perDaySalary + " totalDeductionDays: " + totalDeductionDays + "";

            if (emp.SundayAmount.ToString().ToLower() == "true" || emp.SundayAmount.ToString() == "1")
            {
                sundayBonus = sundayWorked * sundayRate;
            }
            else
            {
                sundayBonus = 0;
            
            }
            //

            double basicsalary = monthlySalary - deductionAmount;
            double finalSalary = (monthlySalary - deductionAmount) + sundayBonus ;

            double grosssalary = (monthlySalary - deductionAmount) + sundayBonus ;
            double esiCut = 0;


            if (emp.ESI.ToString().ToLower() == "true" || emp.ESI.ToString() == "1")
            {
                esiCut = monthlySalary * 0.0075; // 0.25%
                finalSalary = finalSalary - esiCut;
            }

            // 🔥 FINAL ADD (ALL COLUMNS)

            string year=ddlYear.SelectedValue;
             string month=ddlMonth.SelectedValue;
            salaryTable.Rows.Add(
                empCode,
                empName,
                monthlySalary,
                emp.ESI,
                sundayBonus,
                emp.Gender,
                advance,
                  esiCut, // NEW
                totalDeductionDays,
                sundayWorked,
                totalcommissiondaily,
              grosssalary,
                finalSalary,
                year,
                month,
                basicsalary

            );
        }
      
        lvEmployees.DataSource = salaryTable;
        lvEmployees.DataBind();

        con.Close();
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        DateTime attendanceDate = DateTime.Now; // example, replace with your variable

        // Get full month name
        string monthName =ddlMonth.SelectedItem.ToString(); // e.g., "March"

        // Get year as string
        string year =ddlYear.SelectedItem.ToString();
        using (SqlConnection con = new SqlConnection("Data Source=mssql2017.adnshost.com,1533;Initial Catalog=testdb;User ID=testdb;Password=testdb@2575"))
        {



            con.Open();
            


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

                int Litbasicsalary = 0;

                Literal lit = (Literal)item.FindControl("Litbasicsalary");

                if (lit != null && int.TryParse(lit.Text.Trim(), out Litbasicsalary))
                {
                    // value successfully converted
                }
                else
                {
                    Litbasicsalary = 0; // default value
                }
              
                int lessamount = Convert.ToInt32(extraLess) + Convert.ToInt32(advance);


              
               
                double esiCutnew = 0;

                //  double grosssalary = (monthlySalary - deductionAmount) + sundayBonus + totalcommissiondaily;

                double grosssalarycheck = grossSalary + Convert.ToDouble(tea) + Convert.ToDouble(extraComm) + Convert.ToDouble(commission);
                double finalSalarycheck = 0;

                if (Convert.ToInt32(esiCut)>0)
                {
                    esiCutnew = grosssalarycheck * 0.0075; // 0.25%
                    finalSalarycheck = grosssalarycheck - esiCut;
                }


                finalSalarycheck = finalSalarycheck - lessamount;

                dueAdvance = dueAdvance - advance;


                SqlCommand updateanddelete = new SqlCommand(@"BEGIN TRANSACTION; UPDATE emplist SET advanceamount = ISNULL(advanceamount, 0) + ( SELECT ISNULL(SUM(advanceamountcut), 0)   FROM finalsalaryreport     WHERE empcode = '" + empCode + "'  AND reportmonth = '" + monthName + "'       AND reportyear = '" + year + "' ) WHERE empcode = '" + empCode + "'; DELETE FROM finalsalaryreport WHERE empcode = '" + empCode + "'   AND reportmonth = '" + monthName + "'   AND reportyear =  '" + year + "';  COMMIT TRANSACTION; ", con);

                updateanddelete.ExecuteNonQuery();




                SqlCommand cmd = new SqlCommand(@"INSERT INTO finalsalaryreport
                (empcode,Name,deductionday,sunday,sundayamountextra,esicut,tea,extracommsion,grosssalary,Commission,advanceamountcut,dueadvance,NetSalary,reportmonth,reportyear,rts,ExtraAmountLess,basicsalary)
                VALUES
                ('" + empCode + "','" + empName + "','" + deductionDays + "','" + sundayWorked + "'," + sundayText + ",'" + esiCutnew + "','" + tea + "','" + extraComm + "'," + grosssalarycheck + ",'" + commission + "','" + advance + "','" + dueAdvance + "','" + finalSalarycheck + "','" + monthName + "','" + year + "',getdate(),'" + extraLess + "','" + Litbasicsalary + "')", con);

          
               
              
                cmd.ExecuteNonQuery();



                SqlCommand cmdadv = new SqlCommand("update emplist set advanceamount=advanceamount-" + advance + " where empcode='" + empCode + "'", con);
                cmdadv.ExecuteNonQuery();
            }

            lblMessage.Text = "Salaries saved successfully!";
        }

    }
}