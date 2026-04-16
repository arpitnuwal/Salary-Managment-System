<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="addcommission.aspx.cs" Inherits="addcommission" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">  <style>
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

            <%-- ── Alert banner ───────────────────────────────── --%>
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

            <%-- ── Split layout ────────────────────────────────── --%>
            <div class="split">

                <%-- ── LEFT: Entry Form ────────────────────────── --%>
                <div class="card" style="animation-delay:0s">
                    <div class="card-header">
                        <div class="card-header-icon"><i class="fa-solid fa-user-plus"></i></div>
                        <div>
                            <div class="card-header-title">Commission Entry</div>
                           
                        </div>
                    </div>
                    <div class="card-body">

                        <%-- Edit mode indicator --%>
                        <asp:Panel ID="pnlEditMode" runat="server" Visible="false" CssClass="edit-mode-bar">
                            <i class="fa-solid fa-pen-to-square"></i>
                            Editing record &mdash; make changes and click <strong>Update</strong>
                        </asp:Panel>

                        <%-- Hidden field to track edit ID --%>
                        <asp:HiddenField ID="hdnEditID" runat="server" Value="0" />

                        <%-- Employee Code --%>
                        <div class="field">  <asp:DropDownList ID="ddlMonth" runat="server" CssClass="form-control"></asp:DropDownList>

<asp:DropDownList ID="ddlYear" runat="server" CssClass="form-control"></asp:DropDownList><br />
                            <label for="txtEmpCode">Commission Sheet Upload  (format .xls)</label>
                            <div class="input-wrap">
                                <i class="fa-solid fa-hashtag input-icon"></i>
                             <asp:FileUpload ID="FileUpload1" runat="server" />
                            </div>
                            
                        </div>

                        <%-- Employee Name --%>
                        

                        <%-- Employee Salary --%>
                        


                             
                          
                        

                        
                        <%-- Buttons --%>
                        <div class="btn-row">

                            <asp:Label ID="Label1" runat="server" Text="" ForeColor="Red"></asp:Label><br />

                            <asp:Button ID="btnSave"
                                        runat="server"
                                        Text="Upload Excel"
                                        CssClass="btn btn-primary"
                                        OnClick="btnSave_Click"
                                        ClientIDMode="Static" />
                          
                        </div>
                    </div>
                </div>

                <%-- ── RIGHT: Employee List ─────────────────────── --%>
                <div class="card" style="animation-delay:0.08s">
                  
                 
                </div>

            </div><!-- /split -->
        </div>
</asp:Content>

