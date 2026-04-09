<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="finalSalaryReport.aspx.cs" Inherits="finalSalaryReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <style>
        .btn-modern {
    background: linear-gradient(135deg, #4f46e5, #6366f1);
    color: #fff;
    border: none;
    padding: 10px 18px;
    font-size: 14px;
    font-weight: 600;
    border-radius: 8px;
    cursor: pointer;
    transition: all 0.3s ease;
    box-shadow: 0 4px 12px rgba(79, 70, 229, 0.3);
}

.btn-modern:hover {
    transform: translateY(-2px);
    box-shadow: 0 6px 18px rgba(79, 70, 229, 0.4);
    background: linear-gradient(135deg, #4338ca, #4f46e5);
}

.btn-modern:active {
    transform: scale(0.97);
    box-shadow: 0 2px 6px rgba(0,0,0,0.2);
}.full-width-card {
    width: 100%;
}

.filter-bar {
    display: flex;
    gap: 12px;
    align-items: center;
    margin-bottom: 15px;
    flex-wrap: wrap;
}

.form-control {
    padding: 8px 12px;
    border-radius: 8px;
    border: 1px solid #ccc;
    min-width: 120px;
}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="page-wrapper">

    <%-- Alert --%>
    <div class="alert-wrap">
        <asp:Panel ID="pnlAlert" runat="server" Visible="false">
            <div class="alert alert-success">
                <i class="fa-solid fa-circle-check"></i>
                <asp:Label ID="lblAlert" runat="server" />
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlError" runat="server" Visible="false">
            <div class="alert alert-error">
                <i class="fa-solid fa-triangle-exclamation"></i>
                <asp:Label ID="lblError" runat="server" />
            </div>
        </asp:Panel>
    </div>

    <%-- 🔹 TOP FILTER BAR --%>
    <div class="filter-bar">

        <asp:DropDownList ID="ddlMonth" runat="server" CssClass="form-control"></asp:DropDownList>

<asp:DropDownList ID="ddlYear" runat="server" CssClass="form-control"></asp:DropDownList>
        <asp:TextBox ID="txtemptype" runat="server" CssClass="form-control" Text="1"></asp:TextBox>
          <asp:TextBox ID="txtempid" runat="server" CssClass="form-control" Text=""></asp:TextBox>
        <asp:Button ID="btnSalary" runat="server" Text="Check Salary"
            CssClass="btn-modern" OnClick="btnSalary_Click" />

         <asp:Button ID="Button1" runat="server" Text="Export Excel"
            CssClass="btn-modern" OnClick="Button1_Click1" />
          
    </div>
        <asp:Label ID="lblMessage" runat="server"  ForeColor="Red"></asp:Label>
    <%-- 🔹 FULL WIDTH REPORT --%>
    <div class="card full-width-card">

        <div class="card-header">
            <div class="card-header-icon"><i class="fa-solid fa-table-list"></i></div>
            <div>
                <div class="card-header-title">Final Salary Report</div>
            </div>
        </div>

        <div class="card-body">

            <div class="grid-stats">
                <span> Final Salary Report</span>
                <span class="stat-chip">
                    <i class="fa-solid fa-users"></i>
                  
                </span>
            </div>

            <asp:HiddenField ID="LblId" runat="server" />

            <div class="grid-scroll">

                <asp:ListView ID="lvEmployees" runat="server">

                    <LayoutTemplate>
                        <table class="emp-grid">
                            <thead>
                                <tr>
                                    <th>Emp Code</th>
                                    <th>Name</th>
                                    <th>Deduction Days</th>
                                    <th>Basic</th>

                                    <th>Sunday</th>

                                     <th>Tea</th>
                                      <th>Commission</th>
                                      <th>Extra Comm.</th>
                                    <th>Gross Salary</th>
                                       <th>Adv. Salary</th>
                                    <th>Ad. Amount</th>

                                      <th>Net Salary</th>


                                    <th>ESI Cut</th>
                                   
                                  
                                  
                                    
                                   
                                    
                                    
                                  
                                    <th>Due Advance.</th>
                                    <th>EMP Type</th>
                                    <th>Bank Name</th>
                                    <th>IFSC</th>
                                    <th>Account no.</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr runat="server" id="itemPlaceholder"></tr>
                            </tbody>
                        </table>
                    </LayoutTemplate>

                    <ItemTemplate>
    <tr>
        <td>
             <asp:Literal ID="LitEmpCode" runat="server" Text='<%# Eval("EmpCode") %>'></asp:Literal>
            
            </td>


        <td>
             <asp:Literal ID="LitEmpName" runat="server" Text='<%# Eval("Name") %>'></asp:Literal>
          


        </td>
        <td>
            <asp:Literal ID="LitDeductionDays" runat="server" Text='<%# Eval("deductionday") %>'></asp:Literal> <a href="viewleavereport.aspx?EmpCode=<%# Eval("EmpCode") %>&&year=<%# Eval("reportyear") %>&&month=<%# Eval("month") %>">check</a>
            
           </td>

        <td>
            <%# Eval("basicsalary") %>
        </td>
        <td>
                <asp:Literal ID="LitSundayWorked" runat="server" Text='<%# Eval("Sunday") %>'></asp:Literal>
          (Rs. <asp:Literal ID="LitSundayAmount" runat="server" Text='<%# Eval("sundayamountextra") %>'></asp:Literal>/-)<a href="sundayviewleavereport.aspx?EmpCode=<%# Eval("EmpCode") %>&&year=<%# Eval("reportyear") %>&&month=<%# Eval("month") %>">check</a>
         
        </td>
          <td>
            <%# Math.Round(Convert.ToDouble(Eval("tea"))) %>
        </td>
        <td>
            <asp:Literal ID="litCommission" runat="server" Text='<%# Math.Round(Convert.ToDouble(Eval("Commission"))) %>'></asp:Literal><a href="commsioncutreport.aspx?EmpCode=<%# Eval("EmpCode") %>&&year=<%# Eval("year") %>&&month=<%# Eval("month") %>">check</a>
        </td>
         <td>
          <%# Math.Round(Convert.ToDouble(Eval("extracommsion"))) %>
        </td>
         <td>
            <asp:Literal ID="litGrossSalary" runat="server" Text='<%# Math.Round(Convert.ToDouble(Eval("grosssalary"))) %>'></asp:Literal>
        </td>

          <td>
              <%# Math.Round(Convert.ToDouble(Eval("ExtraAmountLess"))) %>
           
        </td>
             <td>
       <%# Math.Round(Convert.ToDouble(Eval("advanceamountcut"))) %>
        </td>
              <td>
            <asp:Literal ID="litNetSalary" runat="server" Text='<%# Math.Round(Convert.ToDouble(Eval("NetSalary"))) %>'></asp:Literal>
        </td>                       
                                 
                                 
                                   









        <td>
          <%# Math.Round(Convert.ToDouble(Eval("esiCut"))) %>
        </td>
      
      
       
       
        
   
      
        <td>
            <asp:Literal ID="litDueAdvance" runat="server" Text='<%# Eval("dueadvance") %>'></asp:Literal>
        </td>
        <td><%# Eval("emptype") %></td>
        <td><%# Eval("bankname") %></td>
        <td><%# Eval("bankifsc") %></td>
        <td><%# Eval("bankaccount") %></td>
    </tr>
</ItemTemplate>

                    <EmptyDataTemplate>
                        <div class="empty-state">
                            No employees found
                        </div>
                    </EmptyDataTemplate>

                </asp:ListView>

            </div>

        </div>
    </div>

</div>
</asp:Content>

