using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebUsingOnLineSevices.CountryInfoService;

namespace WebUsingOnLineSevices
{
    public partial class CountryInfoServicePage : System.Web.UI.Page
    {
        protected void btnLoad_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "";
            lblDetails.Text = "";
            gvCountries.DataSource = null;
            gvCountries.DataBind();

            try
            {
                // Proxy created by "Add Web Reference" named: CountryInfo
                var svc = new CountryInfoService.CountryInfoService();

                // Returns an array of tCountryCodeAndName
                var list = svc.ListOfCountryNamesByName();

                string filter = (txtFilter.Text ?? "").Trim();

                var filtered = list;

                if (!string.IsNullOrEmpty(filter))
                {
                    filtered = list
                        .Where(c => (c.sName ?? "").IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                        .ToArray();
                }

                gvCountries.DataSource = filtered;
                gvCountries.DataBind();

                lblStatus.Text = $"Loaded {filtered.Length} countries (filtered from {list.Length}).";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error: " + Server.HtmlEncode(ex.Message);
            }
        }

        protected void btnDetails_Click(object sender, EventArgs e)
        {
            lblDetails.Text = "";
            try
            {
                var svc = new CountryInfoService.CountryInfoService();

                string iso = (txtIso.Text ?? "").Trim().ToUpperInvariant();
                if (iso.Length != 2)
                {
                    lblDetails.Text = "Please enter a valid 2-letter ISO code (example: IL, US, FR).";
                    return;
                }

                // These methods are part of the service
                string countryName = svc.CountryName(iso);
                string capital = svc.CapitalCity(iso);
                string currency = svc.CountryCurrency(iso)?.sName;   // returns a complex type
                string phoneCode = svc.CountryIntPhoneCode(iso);

                lblDetails.Text =
                    $"<b>{Server.HtmlEncode(countryName)}</b><br/>" +
                    $"Capital: {Server.HtmlEncode(capital)}<br/>" +
                    $"Currency: {Server.HtmlEncode(currency )}<br/>" +
                    $"Phone Code: +{Server.HtmlEncode(phoneCode)}" 
                    ;
            }
            catch (Exception ex)
            {
                lblDetails.Text = "Error: " + Server.HtmlEncode(ex.Message);
            }
        }
    }

}
