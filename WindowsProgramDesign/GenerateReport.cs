using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsProgramDesign
{

    public partial class GenerateReport : Form
    {
        
        public GenerateReport()
        {
            InitializeComponent();
            LoadReportTypes();
        }

        private void LoadReportTypes()
        {
            comboBoxReportType.Items.Clear(); // Clear any existing items
            comboBoxReportType.Items.Add("Appointment");
            comboBoxReportType.Items.Add("Members");
            comboBoxReportType.Items.Add("Trainers");
            comboBoxReportType.Items.Add("Products");
            comboBoxReportType.Items.Add("Receptionists");
            comboBoxReportType.Items.Add("Training Sessions");
            comboBoxReportType.SelectedIndex = 0; // Set default selection
        }

        private void btnGenerateReport_Click(object sender, EventArgs e)
        {
            string selectedReportType = comboBoxReportType.SelectedItem.ToString();

            try
            {
                string query = "";

                // SQL queries for different report types
                switch (selectedReportType)
                {
                    case "Appointment":
                        query = @"
            SELECT 
                A.AppointmentID, 
                A.MemberID, 
                M.Name AS MemberName, 
                A.SessionID, 
                TS.SessionName
            FROM Appointment A
            INNER JOIN Members M ON A.MemberID = M.MemberID
            INNER JOIN TrainingSessions TS ON A.SessionID = TS.TrainingSessionID";
                        break;

                    case "Members":
                        query = @"
                            SELECT 
                                MemberID, 
                                Name AS MemberName, 
                                PhoneNumber, 
                                Email, 
                                Address
                            FROM Members";
                        break;

                    case "Trainers":
                        query = @"
                            SELECT 
                                TrainerID, 
                                Name AS TrainerName, 
                                PhoneNumber, 
                                Email, 
                                Address, 
                                EmploymentStatus
                            FROM Trainers";
                        break;

                    case "Products":
                        query = @"
                            SELECT 
                                ProductID, 
                                ProductName, 
                                ProductCategory, 
                                UnitsOrdered, 
                                UnitsInStock, 
                                UnitsSold, 
                                SellingPrice
                            FROM Products";
                        break;

                    case "Receptionists":
                        query = @"
                            SELECT 
                                ReceptionistID, 
                                Name AS ReceptionistName, 
                                PhoneNumber, 
                                Email, 
                                HomeAddress, 
                                EmploymentStatus
                            FROM Receptionists";
                        break;

                    case "Training Sessions":
                        query = @"
                            SELECT 
                                TrainingSessionID, 
                                SessionName, 
                                Price, 
                                Category, 
                                TrainerID, 
                                TrainerName, 
                                DateAndTime
                            FROM TrainingSessions";
                        break;

                    default:
                        MessageBox.Show("Invalid report type selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                }

                using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
                using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                {
                    DataTable reportTable = new DataTable();
                    adapter.Fill(reportTable);

                    if (reportTable.Rows.Count > 0)
                    {
                        dataGridViewReport.DataSource = reportTable;
                    }
                    else
                    {
                        MessageBox.Show("No data found for the selected report type.", "Report Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        dataGridViewReport.DataSource = null; // Clear previous data
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating the report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

