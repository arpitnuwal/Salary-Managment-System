<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="backup.aspx.cs" Inherits="backup" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
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
      

                <%-- ── LEFT: Entry Form ────────────────────────── --%>
                <div class="card" style="animation-delay:0s">
                    <div class="card-header">
                        <div class="card-header-icon"><i class="fa-solid fa-user-plus"></i></div>
                        <div>
                            <div class="card-header-title">Backup</div>
                       
                        </div>
                    </div>
                    <div class="card-body">

                        <%-- Edit mode indicator --%>
                        <asp:Panel ID="pnlEditMode" runat="server" Visible="false" CssClass="edit-mode-bar">
                         
                          
                        </asp:Panel>

                        <%-- Hidden field to track edit ID --%>
                        <asp:HiddenField ID="hdnEditID" runat="server" Value="0" />

                        <%-- Employee Code --%>
                  

                        <%-- Employee Name --%>
                    
                            <label for="txtEmpName">Query</label>
                         
                                
                                <asp:TextBox ID="txtEmpName"
                                             runat="server"
                                            TextMode="MultiLine" Rows="10" Style="height:500px;width:1000px"  />
                            
                            
                      

                        <%-- Employee Salary --%>
                        


                            
                         

                      

                        

                           

                          
                          

                          

                         
                        <%-- Buttons --%>
                        
                    </div>
                </div>

                <%-- ── RIGHT: Employee List ─────────────────────── --%>
                

           
        </div>
</asp:Content>

