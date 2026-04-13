<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Printslip.aspx.cs" Inherits="Printslip" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">

<button onclick="printDiv()">Print</button>

    <script>
        function printDiv() {
            var divContent = document.getElementById("printArea").innerHTML;
            var originalContent = document.body.innerHTML;

            document.body.innerHTML = divContent;
            window.print();
            document.body.innerHTML = originalContent;
            location.reload();
        }
    </script>
   <div style="border:1px solid #000;" id="printArea">

    <!-- Header -->
    <div style="text-align:center; border-bottom:1px solid #000; padding:10px;">
        <h1 style="margin:0; font-size:48px; font-weight:bold;">Dangi Fashion</h1>
        <h2 style="margin:5px 0 0; font-size:28px;">
            Salary Pay Slip For The Month Of <asp:Label ID="lblmmonthname" runat="server" Text="Label"></asp:Label>  <asp:Label ID="lblyearname" runat="server" Text="Label"></asp:Label> 
        </h2>
    </div>

    <!-- Date -->
    <div style="text-align:right; padding:5px 20px; border-bottom:1px solid #000; font-size:20px;">
     DATE : <b><%= DateTime.Now.ToString("dd-MM-yyyy") %></b>
    </div>

    <!-- Main Table -->
    <table style="width:100%; border-collapse:collapse; font-size:18px;">
        <tr>
            <td style="border:1px solid #000; padding:8px;"><b>Name:</b></td>
            <td style="border:1px solid #000; padding:8px;">Mrs. <asp:Label ID="lblname" runat="server" Text="Label"></asp:Label></td>
            <td style="border:1px solid #000; padding:8px;"><b>Designation</b></td>
            <td style="border:1px solid #000; padding:8px;"><b>Other</b></td>
        </tr>

        <tr>
            <td colspan="2" style="border:1px solid #000; vertical-align:top; padding:0;">
                <table style="width:100%; border-collapse:collapse;">
                    <tr><td style="border:1px solid #000; padding:5px;">I Basic Payment</td><td style="border:1px solid #000; padding:5px;">₹  <asp:Label ID="lblbasicpay" runat="server" Text="Label"></asp:Label></td></tr>
                    <tr><td style="border:1px solid #000; padding:5px;">II SUNDAY BONUS</td><td style="border:1px solid #000; padding:5px;">₹  <asp:Label ID="lblsundayamount" runat="server" Text="Label"></asp:Label></td></tr>
                    <tr><td style="border:1px solid #000; padding:5px;">III OTHERS</td><td style="border:1px solid #000; padding:5px;">₹ 0</td></tr>
                    <tr><td style="border:1px solid #000; padding:5px;">IV STAFF WELFARE EXPENSE</td><td style="border:1px solid #000; padding:5px;">₹  <asp:Label ID="lbltea" runat="server" Text="Label"></asp:Label></td></tr>
                    <tr><td style="border:1px solid #000; padding:5px;">V COMMISSION</td><td style="border:1px solid #000; padding:5px;">₹  <asp:Label ID="lbltotalcomm" runat="server" Text="Label"></asp:Label></td></tr>
                    <tr><td style="border:1px solid #000; padding:5px;">VI OTHERS</td><td style="border:1px solid #000; padding:5px;">₹  <asp:Label ID="lblextracomm" runat="server" Text="Label"></asp:Label></td></tr>
                    <tr><td style="border:1px solid #000; padding:5px;">Sub Total</td><td style="border:1px solid #000; padding:5px;"><b>₹  <asp:Label ID="lbltotalextra" runat="server" Text="Label"></asp:Label></b></td></tr>
                    <tr><td style="border:1px solid #000; padding:5px;">Total Gross Salary</td><td style="border:1px solid #000; padding:5px;"><b>₹  <asp:Label ID="lblgrosssalary" runat="server" Text="Label"></asp:Label></b></td></tr>
                </table>
            </td>

            <td style="border:1px solid #000; vertical-align:top; padding:10px;">
                <b></b>
            </td>

            <td style="border:1px solid #000; vertical-align:top; padding:0;">
                <table style="width:100%; border-collapse:collapse;">
                    <tr><td style="border:1px solid #000; padding:5px;">A DAYS OF THE MONTH</td><td style="border:1px solid #000; padding:5px;">30</td></tr>
                    <tr><td style="border:1px solid #000; padding:5px;">B TOTAL LEAVE DAYS</td><td style="border:1px solid #000; padding:5px;"> <asp:Label ID="lbllaeve" runat="server" Text="Label"></asp:Label></td></tr>
                    <tr><td style="border:1px solid #000; padding:5px;">Total Salary Days</td><td style="border:1px solid #000; padding:5px;"> <asp:Label ID="lbltotalsalaryday" runat="server" Text="Label"></asp:Label></td></tr>
                    <tr><td style="border:1px solid #000; padding:5px;">Sunday Bonus</td><td style="border:1px solid #000; padding:5px;"> <asp:Label ID="lblsundaycount" runat="server" Text="Label"></asp:Label></td></tr>
                    <tr><td colspan="2" style="border:1px solid #000; padding:15px; text-align:center; font-size:22px;">
                        <b>Staff Clo. Bal.  <asp:Label ID="lbldue" runat="server" Text="Label"></asp:Label> DR</b>
                    </td></tr>
                </table>
            </td>
        </tr>
    </table>

    <!-- Deductions -->
    <table style="width:100%; border-collapse:collapse; font-size:18px;">
        <tr>
            <td style="border:1px solid #000; padding:5px;">Advance Loan</td>
            <td style="border:1px solid #000; padding:5px;">₹  <asp:Label ID="lbladvacneamt" runat="server" Text="Label"></asp:Label></td>
        </tr>
        <tr>
            <td style="border:1px solid #000; padding:5px;">Salary Advance</td>
            <td style="border:1px solid #000; padding:5px;">₹  <asp:Label ID="lblsalaryadv" runat="server" Text="Label"></asp:Label></td>
        </tr>
        <tr>
            <td style="border:1px solid #000; padding:5px;"><b>Total Deductions</b></td>
            <td style="border:1px solid #000; padding:5px;"><b>₹  <asp:Label ID="lbltotaldesduct" runat="server" Text="Label"></asp:Label></b></td>
        </tr>
        <tr>
            <td style="border:1px solid #000; padding:5px;"><b>SALARY AFTER DEDUCTIONS</b></td>
            <td style="border:1px solid #000; padding:5px;"><b>₹  <asp:Label ID="lblnetsalary" runat="server" Text="Label"></asp:Label></b></td>
        </tr>
        <tr>
            <td style="border:1px solid #000; padding:5px;"><b>NET TAKE HOME</b></td>
            <td style="border:1px solid #000; padding:5px;"><b>₹ <asp:Label ID="lblnetsalary2" runat="server" Text="Label"></asp:Label></b></td>
        </tr>
    </table>

    <!-- Footer -->
    <div style="padding:15px; text-align:right; font-size:18px;">
        Three Thousand Seven Hundred Forty Only
    </div>

</div>
    </form>
</body>
</html>
