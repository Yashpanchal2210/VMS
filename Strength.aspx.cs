using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace VMS_1
{
    public partial class Strength : System.Web.UI.Page
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
                // Initialize ViewState["DataTable"] if it's null
                if (ViewState["DataTable"] == null)
                {
                    ViewState["DataTable"] = new DataTable();
                }
            }
        }

        protected void SubmitButton_Click(object sender, EventArgs e)
        {
            try
            {
                //string connStr = "Data Source=PIYUSH-JHA\\SQLEXPRESS;Initial Catalog=InsProj;Integrated Security=True;Encrypt=False";
                string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
                // Get data from the form
                string[] dates = Request.Form.GetValues("date");
                string[] vegOfficers = Request.Form.GetValues("vegOfficer");
                string[] nonVegOfficers = Request.Form.GetValues("nonVegOfficer");
                string[] rikOfficers = Request.Form.GetValues("rikOfficer");
                string[] vegStaffs = Request.Form.GetValues("vegStaff");
                string[] nonVegStaffs = Request.Form.GetValues("nonVegStaff");
                string[] rikStaffs = Request.Form.GetValues("rikStaff");
                string[] nonEntitledOfficers = Request.Form.GetValues("nonEntitledOfficer");
                string[] nonEntitledStaffs = Request.Form.GetValues("nonEntitledStaff");
                string[] civilians = Request.Form.GetValues("civilian");

                // Connect to the database
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    // Iterate through each row and insert data into the database
                    for (int i = 0; i < dates.Length; i++)
                    {
                        SqlCommand cmd = new SqlCommand("InsertOrUpdateStrength", conn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Date", dates[i]);
                        cmd.Parameters.AddWithValue("@Veg_Officer", vegOfficers[i]);
                        cmd.Parameters.AddWithValue("@NonVeg_Officer", nonVegOfficers[i]);
                        cmd.Parameters.AddWithValue("@RIK_Officer", rikOfficers[i]);
                        cmd.Parameters.AddWithValue("@Veg_Sailor", vegStaffs[i]);
                        cmd.Parameters.AddWithValue("@NonVeg_Sailor", nonVegStaffs[i]);
                        cmd.Parameters.AddWithValue("@RIK_Sailor", rikStaffs[i]);
                        cmd.Parameters.AddWithValue("@NonEntitled_Officer", nonEntitledOfficers[i]);
                        cmd.Parameters.AddWithValue("@NonEntitled_Sailor", nonEntitledStaffs[i]);
                        cmd.Parameters.AddWithValue("@Civilian", civilians[i]);

                        cmd.ExecuteNonQuery();
                    }
                }

                lblStatus.Text = "Data entered successfully.";

                // Refresh the GridView after data insertion
                BindGridView();

                // Bind the total GridView after data insertion
                BindTotalGridView((DataTable)ViewState["DataTable"]);
            }
            catch (Exception ex)
            {
                // Handle exceptions
                lblStatus.Text = "An error occurred: " + ex.Message;
            }
        }


        // Method to bind data to the GridView

        private void BindGridView()
        {
            try
            {
                //string connStr = "Data Source=PIYUSH-JHA\\SQLEXPRESS;Initial Catalog=InsProj;Integrated Security=True;Encrypt=False";
                string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
                string firstSubmittedDate = Request.Form.GetValues("date")[0];
                DateTime dateTime = DateTime.Parse(firstSubmittedDate);
                string monthFilter = dateTime.ToString("yyyy-MM");

                string query = "SELECT * FROM Strength WHERE CONVERT(VARCHAR(7), Date, 120) = @Month";

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Month", monthFilter);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Store DataTable in ViewState
                    ViewState["DataTable"] = dt;


                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                lblStatus.Text = "An error occurred while fetching data: " + ex.Message;
            }
        }

        private void BindTotalGridView(DataTable dt)
        {
            // Calculate totals for each column except "Date"
            DataRow totalRow = dt.NewRow();
            foreach (DataColumn column in dt.Columns)
            {
                if (column.DataType == typeof(int) && column.ColumnName != "Date")
                {
                    totalRow[column.ColumnName] = dt.Compute($"SUM([{column.ColumnName}])", "");
                }
            }

            // Add the total row to the DataTable
            dt.Rows.Add(totalRow);

            // Bind the totals to the second GridView
            GridViewStrength.DataSource = dt;
            GridViewStrength.DataBind();
        }
    }
}