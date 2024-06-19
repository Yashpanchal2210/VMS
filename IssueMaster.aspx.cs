using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI.WebControls;

namespace VMS_1
{
    public partial class IssueMaster : System.Web.UI.Page
    {
        private string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
            }
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            Response.AppendHeader("Pragma", "no-cache");

            if (!IsPostBack)
            {
                LoadGridView();
                GetItemCategories();
            }
        }

        [WebMethod]
        public static string GetInLiueItemsByCategory(string Category)
        {
            string categoryVal = string.Empty;

            if (Category == "Wardroom")
            {
                categoryVal = "Officer";
            }
            else if (Category == "Galley")
            {
                categoryVal = "Sailor";
            }

            List<string> itemNames = new List<string>();

            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            string query = "SELECT Id, InLieuItem FROM InLieuItems WHERE Category = @category";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Category", categoryVal);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            itemNames.Add(reader["InLieuItem"].ToString());
                            itemNames.Add(reader["Id"].ToString());
                        }
                    }
                }
            }
            return JsonConvert.SerializeObject(itemNames); ;
        }

        [WebMethod]
        public static string GetItemDenom(string ItemVal)
        {

            var result = new
            {
                Denomination = string.Empty,
                VegScale = string.Empty,
                NonVegScale = string.Empty
            };

            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT Denomination, VegScale, NonVegScale  FROM InLieuItems WHERE Id = @BasicItem";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@BasicItem", ItemVal);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    result = new
                    {
                        Denomination = reader["Denomination"].ToString(),
                        VegScale = reader["VegScale"].ToString(),
                        NonVegScale = reader["NonVegScale"].ToString()
                    };
                }
            }

            return new JavaScriptSerializer().Serialize(result);
        }

        [WebMethod]
        public static List<object> GetItemCategories()
        {
            //string connStr = "Data Source=PIYUSH-JHA\\SQLEXPRESS;Initial Catalog=InsProj;Integrated Security=True;Encrypt=False";
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            string query = "SELECT * FROM Items";
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            var categories = new List<object>();
            foreach (DataRow row in dt.Rows)
            {
                categories.Add(new
                {
                    Text = row["ItemName"].ToString(),
                    Value = row["ItemID"].ToString(),
                    ScaleAmount = row["RationScaleOfficer"].ToString()
                });
            }

            return categories;
        }

        [WebMethod]
        public static string GetItemNamesByCategory(string category)
        {
            List<string> itemNames = new List<string>();

            // Your SQL query to fetch item names based on the selected category
            //string connStr = "Data Source=PIYUSH-JHA\\SQLEXPRESS;Initial Catalog=InsProj;Integrated Security=True;Encrypt=False";
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            string query = "SELECT * FROM AlternateItem where ItemID = @Category";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Category", category);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            itemNames.Add(reader["AltItemName"].ToString());
                        }
                    }
                }
            }
            return JsonConvert.SerializeObject(itemNames); ;
        }


        protected void SubmitButton_Click(object sender, EventArgs e)
        {
            try
            {
                //string connStr = "Data Source=PIYUSH-JHA\\SQLEXPRESS;Initial Catalog=InsProj;Integrated Security=True;Encrypt=False";
                string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

                string[] date = Request.Form.GetValues("date");
                //string[] itemcategory = Request.Form.GetValues("itemcategory");
                string[] itemname = Request.Form.GetValues("itemname");
                string[] enterstrength = Request.Form.GetValues("Strength");
                string[] qtyentitled = Request.Form.GetValues("entitledstrength");
                string[] qtyissued = Request.Form.GetValues("Qtyissued");
                //string[] denomination = Request.Form.GetValues("denom");
                string[] role = Request.Form.GetValues("userrole");

                string[] denomination = new string[itemname.Length];

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    int maxLength = Math.Max(date.Length, Math.Max(itemname.Length, Math.Max(enterstrength.Length, Math.Max(qtyentitled.Length, Math.Max(qtyissued.Length, Math.Max(denomination.Length, role.Length))))));

                    for (int i = 0; i < maxLength; i++)
                    {
                        DateTime parsedDate = DateTime.Parse(date[i]);
                        string itemName = itemname[i];
                        decimal qtyIssued = decimal.Parse(qtyissued[i]);

                        // Retrieve the Denomination value from the AlternateItem table
                        SqlCommand getDenominationCmd = new SqlCommand(
                            "SELECT Denomination FROM AlternateItem WHERE AltItemName = @ItemName", conn);
                        getDenominationCmd.Parameters.AddWithValue("@ItemName", itemName);

                        object denomResult = getDenominationCmd.ExecuteScalar();
                        if (denomResult != null && denomResult != DBNull.Value)
                        {
                            denomination[i] = denomResult.ToString();
                        }
                        else
                        {
                            denomination[i] = string.Empty;
                        }

                        SqlCommand checkReceiptCmd = new SqlCommand(
                            "SELECT SUM(Qty) FROM PresentStockMaster WHERE ItemName = @ItemName", conn);
                        checkReceiptCmd.Parameters.AddWithValue("@ItemName", itemName);

                        object result = checkReceiptCmd.ExecuteScalar();
                        if (result == null || result == DBNull.Value)
                        {
                            lblStatus.Text = $"No data found for item {itemName} in PresentStockMaster.";
                            continue; // Skip this iteration
                        }

                        decimal availableQty = Convert.ToDecimal(result);

                        if (availableQty < qtyIssued)
                        {
                            lblStatus.Text = $"Insufficient quantity for item {itemName}.";
                            continue;
                        }

                        SqlCommand cmd = new SqlCommand("InsertIssue", conn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Date", i < date.Length ? date[i] : date[0]);
                        //cmd.Parameters.AddWithValue("@ItemCategoryId", i < itemcategory.Length ? itemcategory[i] : itemcategory[0]);
                        cmd.Parameters.AddWithValue("@ItemName", itemname[i]);
                        cmd.Parameters.AddWithValue("@Strength", enterstrength[i]);
                        cmd.Parameters.AddWithValue("@QtyEntitled", qtyentitled[i]);
                        cmd.Parameters.AddWithValue("@QtyIssued", qtyissued[i]);
                        cmd.Parameters.AddWithValue("@Denomination", denomination[i]);
                        cmd.Parameters.AddWithValue("@Role", i < role.Length ? role[i] : role[0]);
                        cmd.ExecuteNonQuery();

                        // Update PresentStockMaster table if ItemName exists
                        SqlCommand updatePresentStockCmd = new SqlCommand(
                        "UPDATE PresentStockMaster SET Qty = Qty - @Quantity WHERE ItemName = @ItemName", conn);
                        updatePresentStockCmd.Parameters.AddWithValue("@ItemName", itemName);
                        updatePresentStockCmd.Parameters.AddWithValue("@Quantity", qtyIssued);
                        updatePresentStockCmd.Parameters.AddWithValue("@Denos", denomination[i]);
                        updatePresentStockCmd.ExecuteNonQuery();

                        // Extract month and year from the date
                        int month = parsedDate.Month;
                        int year = parsedDate.Year;

                        // Check if a record exists for the same item in the same month and year
                        SqlCommand checkCmd = new SqlCommand(
                            "SELECT COUNT(*) FROM MonthEndStockMaster WHERE MONTH(Date) = @Month AND YEAR(Date) = @Year AND ItemName = @ItemName", conn);
                        checkCmd.Parameters.AddWithValue("@Month", month);
                        checkCmd.Parameters.AddWithValue("@Year", year);
                        checkCmd.Parameters.AddWithValue("@ItemName", itemName);

                        int count = (int)checkCmd.ExecuteScalar();

                        if (count > 0)
                        {
                            // If data exists, update the existing row
                            SqlCommand updateCmd = new SqlCommand(
                                "UPDATE MonthEndStockMaster SET Qty = CASE WHEN Qty - @Quantity >= 0 THEN Qty - @Quantity ELSE Qty END " +
                                "WHERE MONTH(Date) = @Month AND YEAR(Date) = @Year AND ItemName = @ItemName", conn);
                            updateCmd.Parameters.AddWithValue("@Month", month);
                            updateCmd.Parameters.AddWithValue("@Year", year);
                            updateCmd.Parameters.AddWithValue("@ItemName", itemName);
                            updateCmd.Parameters.AddWithValue("@Quantity", qtyIssued);

                            updateCmd.ExecuteNonQuery();
                        }
                        else
                        {
                            // If no data exists, insert a new row
                            SqlCommand insertCmd = new SqlCommand(
                                "INSERT INTO MonthEndStockMaster (Date, ItemName, Qty, Type) VALUES (@Date, @ItemName, @Quantity, @Type)", conn);
                            insertCmd.Parameters.AddWithValue("@Date", parsedDate); // Use the full date here
                            insertCmd.Parameters.AddWithValue("@ItemName", itemName);
                            insertCmd.Parameters.AddWithValue("@Quantity", qtyIssued);
                            insertCmd.Parameters.AddWithValue("@Type", "Issue"); // Set the correct parameter name for Type

                            insertCmd.ExecuteNonQuery();
                        }

                        lblStatus.Text = "Data entered successfully.";
                    }
                }

                LoadGridView();

            }
            catch (Exception)
            {

                throw;
            }
        }

        protected void ddlMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadGridView();
        }

        private void LoadGridView()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter("SELECT im.Date, it.ItemName AS CategoryItemName, im.ItemName AS IssueItemName, im.QtyIssued, im.Denomination FROM IssueMaster im INNER JOIN Items it ON im.ItemCategoryId = it.ItemID", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    GridViewIssue.DataSource = dt;
                    GridViewIssue.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred while binding the grid view: " + ex.Message;
            }
        }
    }
}
