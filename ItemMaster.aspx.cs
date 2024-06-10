using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace VMS_1
{
    public partial class ItemMaster : System.Web.UI.Page
    {
        //private string connStr = "Data Source=PIYUSH-JHA\\SQLEXPRESS;Initial Catalog=InsProj;Integrated Security=True;Encrypt=False";
        private string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
            }
            if (!IsPostBack)
            {
                LoadGridView();
            }
        }

        private void LoadGridView()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT i.ItemID, i.ItemName, i.RationScaleOfficer, i.RationScaleSailor, a.AltItemName, a.AltRationScaleOfficer, a.AltRationScaleSailor 
                    FROM Items i 
                    LEFT JOIN AlternateItem a ON i.ItemID = a.ItemID";
                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                GridView1.DataSource = reader;
                GridView1.DataBind();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                // Get main item details
                string itemName = Request.Form["itemname"];
                decimal officerScale = decimal.Parse(Request.Form["officerScale"]);
                decimal sailorScale = decimal.Parse(Request.Form["sailorScale"]);

                // Get alternate item details
                string[] alternateItemNames = Request.Form.GetValues("alternateitemname");
                string[] equivalentOfficerScales = Request.Form.GetValues("equivalentofficerScale");
                string[] equivalentSailorScales = Request.Form.GetValues("equivalentsailorScale");

                // Connect to the database
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    // Insert or update main item details
                    SqlCommand mainItemCmd = new SqlCommand("UpsertItemWithAlternates", conn);
                    mainItemCmd.CommandType = CommandType.StoredProcedure;
                    mainItemCmd.Parameters.AddWithValue("@ItemName", itemName);
                    mainItemCmd.Parameters.AddWithValue("@RationScaleOfficer", officerScale);
                    mainItemCmd.Parameters.AddWithValue("@RationScaleSailor", sailorScale);
                    mainItemCmd.Parameters.AddWithValue("@AlternateItems", null); // Placeholder for TVP
                    mainItemCmd.ExecuteNonQuery();

                    // Get the newly inserted ItemID or existing ItemID
                    object itemID;
                    using (SqlCommand getIdCmd = new SqlCommand("SELECT ItemID FROM Items WHERE ItemName = @ItemName", conn))
                    {
                        getIdCmd.Parameters.AddWithValue("@ItemName", itemName);
                        itemID = getIdCmd.ExecuteScalar();
                    }

                    // Insert alternate items
                    if (alternateItemNames != null)
                    {
                        for (int i = 0; i < alternateItemNames.Length; i++)
                        {
                            // Check if the alternate item already exists in PresentStockMaster
                            SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM PresentStockMaster WHERE ItemName = @AltItemName", conn);
                            checkCmd.Parameters.AddWithValue("@AltItemName", alternateItemNames[i]);
                            int Presentcount = (int)checkCmd.ExecuteScalar();

                            SqlCommand checkCmdIssue = new SqlCommand("SELECT COUNT(*) FROM AlternateItem WHERE AltItemName = @AltItemName", conn);
                            checkCmdIssue.Parameters.AddWithValue("@AltItemName", alternateItemNames[i]);
                            int Issuecount = (int)checkCmd.ExecuteScalar();


                            if (Presentcount == 0)
                            {
                                // Insert into PresentStockMaster if it doesn't exist
                                SqlCommand insertCmd = new SqlCommand("INSERT INTO PresentStockMaster (ItemName, Qty) VALUES (@AltItemName, @Qty)", conn);
                                insertCmd.Parameters.AddWithValue("@AltItemName", alternateItemNames[i]);
                                insertCmd.Parameters.AddWithValue("@Qty", "0");
                                insertCmd.ExecuteNonQuery();
                            }

                            if (Issuecount == 0)
                            {
                                SqlCommand altItemCmd = new SqlCommand("INSERT INTO AlternateItem (AltItemName, AltRationScaleOfficer, AltRationScaleSailor, ItemID) VALUES (@AltItemName, @AltRationScaleOfficer, @AltRationScaleSailor, @ItemID)", conn);
                                altItemCmd.Parameters.AddWithValue("@AltItemName", alternateItemNames[i]);
                                altItemCmd.Parameters.Add("@AltRationScaleOfficer", SqlDbType.Decimal).Value = Convert.ToDecimal(equivalentOfficerScales[i]);
                                altItemCmd.Parameters.Add("@AltRationScaleSailor", SqlDbType.Decimal).Value = Convert.ToDecimal(equivalentSailorScales[i]);
                                altItemCmd.Parameters.AddWithValue("@ItemID", itemID);
                                altItemCmd.ExecuteNonQuery();
                            }
                        }
                    }
                }

                lblStatus.Text = "Data entered successfully.";
            }
            catch (Exception ex)
            {
                // Handle exceptions
                lblMessage.Text = "An error occurred: " + ex.Message;
            }

            LoadGridView();
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int itemId = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Value);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string deleteAlternateItemsQuery = "DELETE FROM AlternateItem WHERE ItemID = @ItemID";
                SqlCommand deleteAlternateCmd = new SqlCommand(deleteAlternateItemsQuery, conn);
                deleteAlternateCmd.Parameters.AddWithValue("@ItemID", itemId);

                string deleteItemQuery = "DELETE FROM Items WHERE ItemID = @ItemID";
                SqlCommand deleteItemCmd = new SqlCommand(deleteItemQuery, conn);
                deleteItemCmd.Parameters.AddWithValue("@ItemID", itemId);

                conn.Open();
                deleteAlternateCmd.ExecuteNonQuery();
                deleteItemCmd.ExecuteNonQuery();
            }

            LoadGridView();
        }
    }
}