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
using System.Xml.Linq;

namespace WindowsProgramDesign
{
    
    public partial class TraningAppointment : Form
    {
        int selected_appiontmentId { get; set; }

        public TraningAppointment()
        {
            InitializeComponent();
            LoadData();
        }

        private void UpdateGridView()
        {
            string query = "SELECT * FROM v_AppointmentDetails";

            using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection))
            {
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                dataGridView.DataSource = dataTable;
            }
        }


        private void LoadData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
                using (SqlDataAdapter dataAdapter = new SqlDataAdapter("select * from Members", connection))
                {
                    DataTable dataTableMembers = new DataTable();
                    dataAdapter.Fill(dataTableMembers);
                    comboBoxMember.DataSource = dataTableMembers;

                    comboBoxMember.DisplayMember = "Name"; // Display the 'Name' in the ComboBox
                    comboBoxMember.ValueMember = "MemberID"; // Use 'MemberID' as the selected value

                }

                using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
                using (SqlDataAdapter dataAdapter = new SqlDataAdapter("SELECT * FROM TrainingSessions", connection))
                {
                    DataTable dataTableSessions = new DataTable();
                    dataAdapter.Fill(dataTableSessions);

                    comboBoxsession.DataSource = dataTableSessions;
                    comboBoxsession.DisplayMember = "SessionName"; // Display the 'SessionName' in the ComboBox
                    comboBoxsession.ValueMember = "TrainingSessionID"; // Use 'TrainingSessionID' as the selected value
                }
                UpdateGridView();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Get the selected values from comboBox1 and comboBox2
            int selectedMemberID = (int)comboBoxMember.SelectedValue;
            int selectedSessionID = (int)comboBoxsession.SelectedValue;

       

            // SQL query to insert a new appointment
            string query = "INSERT INTO Appointment (MemberID, SessionID) VALUES (@MemberID, @SessionID)";

            // Using statement to handle the connection
            using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                // Adding parameters to prevent SQL injection
                command.Parameters.AddWithValue("@MemberID", selectedMemberID);
                command.Parameters.AddWithValue("@SessionID", selectedSessionID);

                // Open the connection
                connection.Open();

                // Execute the insert command
                command.ExecuteNonQuery();

                // Inform the user
                MessageBox.Show("Appointment added successfully.");
            }

            UpdateGridView();
        }


        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selected_appiontmentId > 0) // Ensure a valid appointment is selected
            {
                int selectedMemberID = (int)comboBoxMember.SelectedValue;
                int selectedSessionID = (int)comboBoxsession.SelectedValue;

                // SQL query to update the selected appointment with new session details
                string query = "UPDATE Appointment SET SessionID = @SessionID WHERE AppointmentID = @AppointmentID AND MemberID = @MemberID";

                using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Adding parameters to prevent SQL injection
                    command.Parameters.AddWithValue("@AppointmentID", selected_appiontmentId);
                    command.Parameters.AddWithValue("@MemberID", selectedMemberID);
                    command.Parameters.AddWithValue("@SessionID", selectedSessionID);

                    // Open the connection
                    connection.Open();

                    // Execute the update command
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // Inform the user
                        MessageBox.Show("Appointment updated successfully.");
                    }
                    else
                    {
                        // Inform the user if no rows were affected
                        MessageBox.Show("No matching appointment found to update.");
                    }
                }

                // Refresh the DataGridView to show the updated data
                UpdateGridView();
            }
            else
            {
                MessageBox.Show("Please select a valid appointment to update.");
            }
        }


        private void backmain_Click(object sender, EventArgs e)
        {
          
                this.Hide(); // Hide current form
                ManagerMain mainForm = new ManagerMain();
                mainForm.Show(); // Open the main form
        
        }

    

        private void btnViewAppointments_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView.Rows[e.RowIndex];
                selected_appiontmentId = Convert.ToInt32(row.Cells["AppointmentID"].Value);

                // SQL query to get the details of the selected appointment
                string query = "SELECT MemberID, SessionID FROM Appointment WHERE AppointmentID = @AppointmentID";

                using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@AppointmentID", selected_appiontmentId);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int memberID = reader.GetInt32(reader.GetOrdinal("MemberID"));
                            int sessionID = reader.GetInt32(reader.GetOrdinal("SessionID"));

                            // Load the values into the combo boxes
                            comboBoxMember.SelectedValue = memberID;
                            comboBoxsession.SelectedValue = sessionID;
                        }
                    }
                }
            }
        }


        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selected_appiontmentId > 0) // Ensure a valid appointment is selected
            {
                // SQL query to delete the selected appointment
                string query = "DELETE FROM Appointment WHERE AppointmentID = @AppointmentID";

                using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Adding parameter to prevent SQL injection
                    command.Parameters.AddWithValue("@AppointmentID", selected_appiontmentId);

                    // Open the connection
                    connection.Open();

                    // Execute the delete command
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // Inform the user
                        MessageBox.Show("Appointment deleted successfully.");
                    }
                    else
                    {
                        // Inform the user if no rows were affected
                        MessageBox.Show("No matching appointment found to delete.");
                    }
                }

                // Refresh the DataGridView to show the updated data
                UpdateGridView();
            }
            else
            {
                MessageBox.Show("Please select a valid appointment to delete.");
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();

            // Construct the SQL query with search parameters
            string query = @"SELECT a.AppointmentID, m.Name AS MemberName, ts.SessionName, ts.TrainerName, ts.DateAndTime
                     FROM Appointment a
                     INNER JOIN Members m ON a.MemberID = m.MemberID
                     INNER JOIN TrainingSessions ts ON a.SessionID = ts.TrainingSessionID
                     WHERE 
                        a.AppointmentID LIKE @SearchText OR
                        m.Name LIKE @SearchText OR
                        ts.TrainerName LIKE @SearchText";

            using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                // Add parameter with wildcard characters for partial matching
                command.Parameters.AddWithValue("@SearchText", "%" + searchText + "%");

                DataTable dataTable = new DataTable();

                using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                {
                    dataAdapter.Fill(dataTable);
                }

                // Update DataGridView with the search results
                dataGridView.DataSource = dataTable;
            }
        }

    }
}
