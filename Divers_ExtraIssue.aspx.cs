using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VMS_1
{
    public partial class Divers_ExtraIssue : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
            }
            LoadGridView();
        }

        protected void SubmitButton_Click(object sender, EventArgs e)
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

                string[] date = Request.Form.GetValues("date");
                string[] name = Request.Form.GetValues("name");
                string[] rank = Request.Form.GetValues("rank");
                string[] pno = Request.Form.GetValues("pno");
                string[] days = Request.Form.GetValues("days");
                string[] chocolate = Request.Form.GetValues("chocolate");
                string[] horlicks = Request.Form.GetValues("horlicks");
                string[] eggs = Request.Form.GetValues("eggs");
                string[] milk = Request.Form.GetValues("milk");
                string[] gnut = Request.Form.GetValues("gnut");
                string[] butter = Request.Form.GetValues("butter");
                string[] sugar = Request.Form.GetValues("sugar");

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    // Iterate through each row and insert data into the database
                    for (int i = 0; i < name.Length; i++)
                    {
                        int dayVal = int.Parse(days[i]);

                        if (chocolate[i] == "Chocolate (50 gms)")
                        {
                            double calChoco = dayVal * 0.005;
                            chocolate[i] = calChoco.ToString();
                        }

                        if (horlicks[i] == "Complan/ Horlicks (50 gms)")
                        {
                            double calHorlicks = dayVal * 0.005;
                            horlicks[i] = calHorlicks.ToString();
                        }

                        if (eggs[i] == "Eggs (2 Nos)")
                        {
                            double calEggs = dayVal * 2;
                            eggs[i] = calEggs.ToString();
                        }
                        else if (eggs[i] == "Milk Fresh (150 ml)")
                        {
                            double calEggs = dayVal * 0.150;
                            eggs[i] = calEggs.ToString();
                        }
                        else if (eggs[i] == "Milk Tinned (55 gms)")
                        {
                            double calEggs = dayVal * 0.055;
                            eggs[i] = calEggs.ToString();
                        }
                        else if (eggs[i] == "Milk Powder (20 gms)")
                        {
                            double calEggs = dayVal * 0.020;
                            eggs[i] = calEggs.ToString();
                        }
                        else if (eggs[i] == "Cheese Tinned (50 gms)")
                        {
                            double calEggs = dayVal * 0.050;
                            eggs[i] = calEggs.ToString();
                        }


                        if (milk[i] == "Milk Fresh (200 ml)")
                        {
                            double calMilk = dayVal * 0.200;
                            milk[i] = calMilk.ToString();
                        }
                        else if (milk[i] == "Milk Tinned (80 gms)")
                        {
                            double calMilk = dayVal * 0.08;
                            milk[i] = calMilk.ToString();
                        }
                        else if (milk[i] == "Milk Powder (28 gms)")
                        {
                            double calMilk = dayVal * 0.028;
                            milk[i] = calMilk.ToString();
                        }


                        if (gnut[i] == "Ground-nut (50 gins)")
                        {
                            double calGnut = dayVal * 0.05;
                            gnut[i] = calGnut.ToString();
                        }

                        if (butter[i] == "Butter Fresh/Tinned (50 gms)")
                        {
                            double calButter = dayVal * 0.05;
                            butter[i] = calButter.ToString();
                        }

                        if (sugar[i] == "Sugar (50 gms)")
                        {
                            double calSugar = dayVal * 0.05;
                            sugar[i] = calSugar.ToString();
                        }

                        SqlCommand cmd = new SqlCommand("INSERT INTO ExtraIssue (Name, Rank, PNO, Days, Chocolate, Horlicks, Eggs, Milk, Gnut, Butter, Sugar, Date) VALUES (@Name, @Rank, @PNO, @Days, @Chocolate, @Horlicks, @Eggs, @Milk, @Gnut, @Butter, @Sugar, @Date)", conn);

                        cmd.Parameters.AddWithValue("@Name", name[i]);
                        cmd.Parameters.AddWithValue("@Rank", rank[i]);
                        cmd.Parameters.AddWithValue("@PNO", pno[i]);
                        cmd.Parameters.AddWithValue("@Days", int.Parse(days[i]));
                        cmd.Parameters.AddWithValue("@Chocolate", chocolate[i]);
                        cmd.Parameters.AddWithValue("@Horlicks", horlicks[i]);
                        cmd.Parameters.AddWithValue("@Eggs", eggs[i]);
                        cmd.Parameters.AddWithValue("@Milk", milk[i]);
                        cmd.Parameters.AddWithValue("@Gnut", gnut[i]);
                        cmd.Parameters.AddWithValue("@Butter", butter[i]);
                        cmd.Parameters.AddWithValue("@Sugar", sugar[i]);
                        cmd.Parameters.AddWithValue("@Date", date[i]);

                        cmd.ExecuteNonQuery();
                    }
                }
                lblStatus.Text = "Data entered successfully.";

                LoadGridView();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error: " + ex.Message;
            }
        }

        private void LoadGridView()
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM ExtraIssue", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    GridViewExtraIssueDivers.DataSource = dt;
                    GridViewExtraIssueDivers.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred while binding the grid view: " + ex.Message;
            }
        }

    }
}