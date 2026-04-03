<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="viewleavereport.aspx.cs" Inherits="viewleavereport" %>

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
<style>
/* GridView container */
.gridview-style {
    width: 100%;
    border-collapse: collapse;
    font-family: Arial, sans-serif;
    box-shadow: 0 2px 8px rgba(0,0,0,0.1);
}

/* Header style */
.gridview-header {
    background-color: #4CAF50;
    color: white;
    font-weight: bold;
    text-align: left;
    padding: 10px;
    border-bottom: 2px solid #ddd;
}

/* Normal row */
.gridview-row {
    background-color: #ffffff;
    color: #333;
    padding: 8px;
    border-bottom: 1px solid #ddd;
}

/* Alternating row */
.gridview-alternate {
    background-color: #f9f9f9;
}

/* Hover effect */
.gridview-style tr:hover {
    background-color: #f1f1f1;
    cursor: pointer;
}

/* Adjust column padding */
.gridview-style td {
    padding: 8px 12px;
}
</style>
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
  
        <asp:Label ID="lblMessage" runat="server"  ForeColor="Red"></asp:Label>
    <%-- 🔹 FULL WIDTH REPORT --%>
    <div class="card full-width-card">

        <div class="card-header">
            <div class="card-header-icon"><i class="fa-solid fa-table-list"></i></div>
            <div>
                <div class="card-header-title">Leave Report</div>
            </div>
        </div>

        <div class="card-body">

            <div class="grid-stats">
                <span>Leave Report</span>
                <span class="stat-chip">
                    <i class="fa-solid fa-users"></i>
                  
                </span>
            </div>

            <asp:HiddenField ID="LblId" runat="server" />

            <div class="grid-scroll">

                <asp:GridView ID="lvEmployees" runat="server" CssClass="gridview-style" >

                    
                   

                </asp:GridView>

            </div>

        </div>
    </div>

</div>
</asp:Content>

