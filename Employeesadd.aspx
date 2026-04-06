<%@ Page Title="" Language="C#"  enableEventValidation="false" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Employeesadd.aspx.cs" Inherits="Employeesadd" %>

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
            <div class="split">

                <%-- ── LEFT: Entry Form ────────────────────────── --%>
                <div class="card" style="animation-delay:0s">
                    <div class="card-header">
                        <div class="card-header-icon"><i class="fa-solid fa-user-plus"></i></div>
                        <div>
                            <div class="card-header-title">Employee Entry</div>
                            <div class="card-header-sub">Add or update employee records</div>
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
                        <div class="field">
                            <label for="txtEmpCode">Employee Code</label>
                            <div class="input-wrap">
                                <i class="fa-solid fa-hashtag input-icon"></i>
                                <asp:TextBox ID="txtEmpCode"
                                             runat="server"
                                             placeholder="e.g. EMP-001"
                                             MaxLength="20"
                                             ClientIDMode="Static" />
                            </div>
                            
                        </div>

                        <%-- Employee Name --%>
                        <div class="field">
                            <label for="txtEmpName">Employee Name</label>
                            <div class="input-wrap">
                                <i class="fa-solid fa-user input-icon"></i>
                                <asp:TextBox ID="txtEmpName"
                                             runat="server"
                                             placeholder="Full name"
                                             MaxLength="100"
                                             ClientIDMode="Static" />
                            </div>
                            
                        </div>

                        <%-- Employee Salary --%>
                        <div class="field">
                            <label for="txtEmpSalary">Employee Salary (₹)</label>
                            <div class="input-wrap">
                                <i class="fa-solid fa-indian-rupee-sign input-icon"></i>
                                <asp:TextBox ID="txtEmpSalary"
                                             runat="server"
                                             placeholder="e.g. 50000"
                                             MaxLength="15"
                                             ClientIDMode="Static" />
                            </div>
                            
                          
                        </div>


                             <div class="field">
                            <label for="txtEmpSalary">ESI</label>
                            <div class="input-wrap">
                              <asp:RadioButton ID="rdESIyes" runat="server" Text="Yes"  GroupName="esi" Checked="true"/>&nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:RadioButton ID="rdESIno" runat="server" Text="No"  GroupName="esi"/>
                            </div>

                                 </div>
                          <div class="field">
                            <label for="txtEmpSalary">Sunday Amount Exclude</label>
                            <div class="input-wrap">
                              <asp:RadioButton ID="rdsundayyes" runat="server" Text="Yes"  GroupName="sunday" Checked="true"/>&nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:RadioButton ID="rdsundayno" runat="server" Text="No"  GroupName="sunday" />
                            </div>

                                 </div>

                        <div class="field">
                            <label for="txtEmpSalary">Gender</label>
                            <div class="input-wrap">
                              <asp:RadioButton ID="rdgenderyes" runat="server" Text="Male"  GroupName="Gender" Checked="true"/>&nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:RadioButton ID="rdgenderno" runat="server" Text="Fe-male"  GroupName="Gender" />
                            </div>

                                 </div>

                        <div class="field">
                            <label for="txtEmpSalary">Advance Amount (₹)</label>
                            <div class="input-wrap">
                                <i class="fa-solid fa-indian-rupee-sign input-icon"></i>
                                <asp:TextBox ID="txtadvance"
                                             runat="server"
                                             placeholder="e.g. 50000"
                                             MaxLength="15" Text="0"
                                           />
                            </div>
                            
                         
                        </div>

                           <div class="field">
                            <label for="txtEmpSalary">EMP Type</label>
                            <div class="input-wrap">
                                <i class="fa-solid fa-indian-rupee-sign input-icon"></i>
                                <asp:TextBox ID="txtemptpye"
                                             runat="server"
                                             placeholder="e.g. 1"
                                             MaxLength="15" Text="0"
                                           />
                            </div>
                            
                         
                        </div>

                          <div class="field">
                            <label for="txtEmpSalary">Bank Name</label>
                            <div class="input-wrap">
                                <i class="fa-solid fa-indian-rupee-sign input-icon"></i>
                                <asp:TextBox ID="txtbankname"
                                             runat="server"
                                             placeholder="e.g. BOB"
                                             MaxLength="15" Text="0"
                                           />
                            </div>
                            
                         
                        </div>

                          <div class="field">
                            <label for="txtEmpSalary">Bank IFSC</label>
                            <div class="input-wrap">
                                <i class="fa-solid fa-indian-rupee-sign input-icon"></i>
                                <asp:TextBox ID="txtifsc"
                                             runat="server"
                                             placeholder="e.g. 12346"
                                             MaxLength="15" Text="0"
                                           />
                            </div>
                            
                         
                        </div>

                         <div class="field">
                            <label for="txtEmpSalary">Bank Acoount no</label>
                            <div class="input-wrap">
                                <i class="fa-solid fa-indian-rupee-sign input-icon"></i>
                                <asp:TextBox ID="txtaccount"
                                             runat="server"
                                             placeholder="e.g. 12121212121"
                                             MaxLength="15" Text="0"
                                           />
                            </div>
                            
                         
                        </div>
                        <%-- Buttons --%>
                        <div class="btn-row">
                            <asp:Button ID="btnSave"
                                        runat="server"
                                        Text="💾  Save Employee"
                                        CssClass="btn btn-primary"
                                        OnClick="btnSave_Click"
                                        ClientIDMode="Static" />
                          
                        </div>
                    </div>
                </div>

                <%-- ── RIGHT: Employee List ─────────────────────── --%>
                <div class="card" style="animation-delay:0.08s">
                    <div class="card-header">
                        <div class="card-header-icon"><i class="fa-solid fa-table-list"></i></div>
                        <div>
                            <div class="card-header-title">Employee Directory</div>
                            <div class="card-header-sub">All registered employees</div>
                        </div>
                    </div>
                    <div class="card-body">

                        <div class="grid-stats">
                            <span>Showing all employees</span>
                            <span class="stat-chip">
                                <i class="fa-solid fa-users"></i>
                                <asp:Label ID="lblCount" runat="server" Text="0" /> records
                            </span>
                        </div>
                                        <asp:HiddenField ID="LblId" runat="server" />
                        <div class="grid-scroll">

    <asp:ListView ID="lvEmployees" runat="server" OnItemCommand="lvEmployees_ItemCommand">

        <LayoutTemplate>
            <table class="emp-grid">
                <thead>
                    <tr>
                        <th>#</th>
                        <th>Emp Code</th>
                        <th>Name</th>
                        <th>Salary</th>
                        <th>Action</th>
                    </tr>
                </thead>
                <tbody>
                    <tr runat="server" id="itemPlaceholder"></tr>
                </tbody>
            </table>
        </LayoutTemplate>

        <ItemTemplate>
            <tr>
                 <asp:HiddenField ID="HdnID" runat="server" Value='<%# Eval("id") %>' />
              <asp:Label ID="lblempcode" runat="server" Text='<%# Eval("empcode") %>' Visible="false"></asp:Label>
         <asp:Label ID="lblempname" runat="server" Text='<%# Eval("empname") %>' Visible="false"></asp:Label>

                    <asp:Label ID="lblempsalary" runat="server" Text='<%# Eval("empsalary") %>' Visible="false"></asp:Label>
                    <asp:Label ID="lblesi" runat="server" Text='<%# Eval("esi") %>' Visible="false"></asp:Label>
                    <asp:Label ID="lblsundayamountgive" runat="server" Text='<%# Eval("sundayamountgive") %>' Visible="false"></asp:Label>
                    <asp:Label ID="lbgender" runat="server" Text='<%# Eval("gender") %>' Visible="false"></asp:Label>
                    <asp:Label ID="lbladvanceamount" runat="server" Text='<%# Eval("advanceamount") %>' Visible="false"></asp:Label>
        
                 <asp:Label ID="lblemptype" runat="server" Text='<%# Eval("emptype") %>' Visible="false"></asp:Label>
        
                 <asp:Label ID="lblbankname" runat="server" Text='<%# Eval("bankname") %>' Visible="false"></asp:Label>
        
                 <asp:Label ID="lblbankifsc" runat="server" Text='<%# Eval("bankifsc") %>' Visible="false"></asp:Label>
        
                 <asp:Label ID="lblbankaccount" runat="server" Text='<%# Eval("bankaccount") %>' Visible="false"></asp:Label>

        


                <td><%# Container.DataItemIndex + 1 %></td>

                <td><%# Eval("empcode") %></td>
                <td><%# Eval("empname") %></td>

                <td>₹*****</td>

                <td>
                    <asp:Button ID="btnEdit" runat="server"
                        CommandName="EditEmp"
                       
                        Text="Edit" CssClass="btn-edit" />

                    <asp:Button ID="btnDelete" runat="server"
                        CommandName="DeleteEmp"
                      
                        Text="Delete"
                        CssClass="btn-delete"
                        />
                </td>
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

            </div><!-- /split -->
        </div>
</asp:Content>

