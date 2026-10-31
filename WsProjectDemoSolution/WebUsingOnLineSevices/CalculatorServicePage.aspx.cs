using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebUsingOnLineSevices
{
    public partial class CalculatorServicePage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        private CalculatorService.Calculator CreateService()
        {
            return new CalculatorService.Calculator();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            Calculate("add");
        }

        protected void btnSub_Click(object sender, EventArgs e)
        {
            Calculate("sub");
        }

        protected void btnMul_Click(object sender, EventArgs e)
        {
            Calculate("mul");
        }

        protected void btnDiv_Click(object sender, EventArgs e)
        {
            Calculate("div");
        }

        private void Calculate(string operation)
        {
            lblResult.Text = "";

            try
            {
                int a = int.Parse(txtA.Text);
                int b = int.Parse(txtB.Text);

                var service = CreateService();
                int result = 0;

                switch (operation)
                {
                    case "add":
                        result = service.Add(a, b);
                        break;
                    case "sub":
                        result = service.Subtract(a, b);
                        break;
                    case "mul":
                        result = service.Multiply(a, b);
                        break;
                    case "div":
                        result = service.Divide(a, b);
                        break;
                }

                lblResult.Text = $"Result: {result}";
            }
            catch (DivideByZeroException)
            {
                lblResult.Text = "Error: Cannot divide by zero.";
            }
            catch (Exception ex)
            {
                lblResult.Text = "Error: " + Server.HtmlEncode(ex.Message);
            }
        }
    }
}
