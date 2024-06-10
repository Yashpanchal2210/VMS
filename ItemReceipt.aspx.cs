using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI.WebControls;

namespace VMS_1
{
    public partial class ItemReceipt : System.Web.UI.Page
    {
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
                if (ViewState["DataTable"] == null)
                {
                    ViewState["DataTable"] = new DataTable();
                }
                BindGridView();
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string[] GetItemNames()
        {
            HashSet<string> itemNames = new HashSet<string>();
            //string connStr = "Data Source=PIYUSH-JHA\\SQLEXPRESS;Initial Catalog=InsProj;Integrated Security=True;Encrypt=False";
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            string query = "SELECT AltItemName FROM AlternateItem ORDER BY AltItemName ASC";
            string itemquery = "SELECT ItemName FROM Items ORDER BY ItemName ASC";

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    SqlCommand command = new SqlCommand(query, conn);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        itemNames.Add(reader["AltItemName"].ToString());
                    }
                    reader.Close();
                }

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    SqlCommand command = new SqlCommand(itemquery, conn);
                    SqlDataReader itemReader = command.ExecuteReader();

                    while (itemReader.Read())
                    {
                        itemNames.Add(itemReader["ItemName"].ToString());
                    }
                    itemReader.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while loading item names: " + ex.Message);
            }

            return itemNames.ToArray();
        }


        protected void SubmitButton_Click(object sender, EventArgs e)
        {
            try
            {
                //string connStr = "Data Source=PIYUSH-JHA\\SQLEXPRESS;Initial Catalog=InsProj;Integrated Security=True;Encrypt=False";
                string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

                string[] itemNames = Request.Form.GetValues("itemname");
                string[] quantities = Request.Form.GetValues("qty");
                string[] denominations = Request.Form.GetValues("denom");
                string[] receivedFrom = Request.Form.GetValues("rcvdfrom");
                string[] otherReceivedFrom = Request.Form.GetValues("otherReceivedFrom");
                string[] referenceNos = Request.Form.GetValues("ref");
                string[] dates = Request.Form.GetValues("date");

                List<string> missingFields = new List<string>();

                if (itemNames == null) missingFields.Add("itemname");
                if (quantities == null) missingFields.Add("qty");
                if (denominations == null) missingFields.Add("denom");
                if (receivedFrom == null) missingFields.Add("rcvdfrom");
                if (referenceNos == null) missingFields.Add("ref");
                if (dates == null) missingFields.Add("date");

                if (missingFields.Count > 0)
                {
                    lblStatus.Text = "The following form values are missing: " + string.Join(", ", missingFields);
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    for (int i = 0; i < dates.Length; i++)
                    {
                        DateTime date = DateTime.Parse(dates[i]);
                        string itemName = itemNames[i];
                        decimal quantity = decimal.Parse(quantities[i]);
                        int month = date.Month;
                        int year = date.Year;

                        string receivedFromValue = receivedFrom[i];
                        if (receivedFromValue == "Others" && otherReceivedFrom != null && otherReceivedFrom.Length > i)
                        {
                            receivedFromValue = otherReceivedFrom[i];
                        }

                        SqlCommand cmd = new SqlCommand("INSERT INTO ReceiptMaster (itemNames, quantities, denominations, receivedFrom, referenceNos, Dates) VALUES (@ItemName, @Quantity, @Denomination, @ReceivedFrom, @ReferenceNo, @Date)", conn);
                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.AddWithValue("@ItemName", itemName);
                        cmd.Parameters.AddWithValue("@Quantity", decimal.Parse(quantities[i]));
                        cmd.Parameters.AddWithValue("@Denomination", denominations[i]);
                        cmd.Parameters.AddWithValue("@ReceivedFrom", receivedFromValue);
                        cmd.Parameters.AddWithValue("@ReferenceNo", referenceNos[i]);
                        cmd.Parameters.AddWithValue("@Date", DateTime.Parse(dates[i]));

                        cmd.ExecuteNonQuery();

                        // Update PresentStockMaster table if ItemName exists
                        SqlCommand checkItemCmd = new SqlCommand("SELECT COUNT(*) FROM PresentStockMaster WHERE ItemName = @ItemName", conn);
                        checkItemCmd.Parameters.AddWithValue("@ItemName", itemName);

                        int itemCount = (int)checkItemCmd.ExecuteScalar();

                        if (itemCount > 0)
                        {
                            // If item exists, update the quantity
                            SqlCommand updatePresentStockCmd = new SqlCommand("UPDATE PresentStockMaster SET Qty = Qty + @Quantity WHERE ItemName = @ItemName", conn);
                            updatePresentStockCmd.Parameters.AddWithValue("@ItemName", itemName);
                            updatePresentStockCmd.Parameters.AddWithValue("@Quantity", quantity);
                            updatePresentStockCmd.Parameters.AddWithValue("@Denos", denominations[i]);

                            updatePresentStockCmd.ExecuteNonQuery();
                        }
                        else
                        {
                            // If item does not exist, insert a new record
                            SqlCommand insertPresentStockCmd = new SqlCommand("INSERT INTO PresentStockMaster (ItemName, Qty, Denos) VALUES (@ItemName, @Quantity, @Denos)", conn);
                            insertPresentStockCmd.Parameters.AddWithValue("@ItemName", itemName);
                            insertPresentStockCmd.Parameters.AddWithValue("@Quantity", quantity);
                            insertPresentStockCmd.Parameters.AddWithValue("@Denos", denominations[i]);

                            insertPresentStockCmd.ExecuteNonQuery();
                        }


                        SqlCommand checkCmd = new SqlCommand(
                        "SELECT COUNT(*) FROM MonthEndStockMaster WHERE MONTH(Date) = @Month AND YEAR(Date) = @Year AND ItemName = @ItemName", conn);
                        checkCmd.Parameters.AddWithValue("@Month", month);
                        checkCmd.Parameters.AddWithValue("@Year", year);
                        checkCmd.Parameters.AddWithValue("@ItemName", itemName);

                        int count = (int)checkCmd.ExecuteScalar();

                        if (count > 0)
                        {
                            SqlCommand updateCmd = new SqlCommand(
                                "UPDATE MonthEndStockMaster SET Qty = Qty + @Quantity WHERE MONTH(Date) = @Month AND YEAR(Date) = @Year AND ItemName = @ItemName", conn);
                            updateCmd.Parameters.AddWithValue("@Month", month);
                            updateCmd.Parameters.AddWithValue("@Year", year);
                            updateCmd.Parameters.AddWithValue("@ItemName", itemName);
                            updateCmd.Parameters.AddWithValue("@Quantity", quantity);

                            updateCmd.ExecuteNonQuery();
                        }
                        else
                        {
                            SqlCommand insertCmd = new SqlCommand(
                                "INSERT INTO MonthEndStockMaster (Date, ItemName, Qty, Type) VALUES (@Date, @ItemName, @Quantity, @Type)", conn);
                            insertCmd.Parameters.AddWithValue("@Date", date); // Use the full date here
                            insertCmd.Parameters.AddWithValue("@ItemName", itemName);
                            insertCmd.Parameters.AddWithValue("@Quantity", quantity);
                            insertCmd.Parameters.AddWithValue("@Type", "Receipt"); // Set the correct parameter name for Type

                            insertCmd.ExecuteNonQuery();
                        }
                    }
                }

                lblStatus.Text = "Data entered successfully.";
                BindGridView();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred: " + ex.Message;
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static List<string[]> GetFilteredData(string monthYear)
        {
            List<string[]> data = new List<string[]>();
            //string connStr = "Data Source=PIYUSH-JHA\\SQLEXPRESS;Initial Catalog=InsProj;Integrated Security=True;Encrypt=False";
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT * FROM ReceiptMaster WHERE CONVERT(VARCHAR(7), Dates, 120) = @MonthYear", conn);
                    cmd.Parameters.AddWithValue("@MonthYear", monthYear);

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        data.Add(new string[]
                        {
                            reader["itemNames"].ToString(),
                            reader["quantities"].ToString(),
                            reader["denominations"].ToString(),
                            reader["receivedFrom"].ToString(),
                            reader["referenceNos"].ToString(),
                            DateTime.Parse(reader["Dates"].ToString()).ToString("yyyy-MM-dd")
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching data: " + ex.Message);
            }

            return data;
        }

        private void BindGridView()
        {
            //string connStr = "Data Source=PIYUSH-JHA\\SQLEXPRESS;Initial Catalog=InsProj;Integrated Security=True;Encrypt=False";
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM ReceiptMaster", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    //GridView1.DataSource = dt;
                    //GridView1.DataBind();
                    GridView.DataSource = dt;
                    GridView.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred while binding the grid view: " + ex.Message;
            }
        }
    }
}