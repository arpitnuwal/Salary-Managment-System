using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class test : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //Response.Write(GetCommission());
    }

    public decimal GetCommission(decimal amount)
    {
        decimal absAmount = Math.Abs(amount);

        decimal commission = Math.Ceiling(absAmount / 1000);

        return amount < 0 ? -commission : commission;
    }
}