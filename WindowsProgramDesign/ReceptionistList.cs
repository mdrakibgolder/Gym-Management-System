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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace WindowsProgramDesign
{
    public partial class ReceptionistList : Form
    {
        public ReceptionistList()
        {
            InitializeComponent();
        }
        
        private void ReceptionistList_Load(object sender, EventArgs e)
        {
            LoadData(); 
        }
        
        private void LoadData()
        {
            try
            {
                string query = "SELECT * FROM Receptionists";

                using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(query, connection))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dataGridView.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                string name = txtName.Text;
                string phoneNumber = txtPhoneNumber.Text;
                string email = txtEmail.Text;
                string homeAddress = txtHomeAddress.Text;
                string employmentStatus = cboxEmploymentStatus.SelectedItem?.ToString();

                // Check for empty fields before inserting
                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(phoneNumber) ||
                    string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(homeAddress) ||
                    string.IsNullOrWhiteSpace(employmentStatus))
                {
                    MessageBox.Show("Please fill all the fields.");
                    return;
                }

                string query = "INSERT INTO Receptionists (Name, PhoneNumber, Email, HomeAddress, EmploymentStatus) " +
                               "VALUES (@Name, @PhoneNumber, @Email, @HomeAddress, @EmploymentStatus)";

                using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", name);
                        command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@HomeAddress", homeAddress);
                        command.Parameters.AddWithValue("@EmploymentStatus", employmentStatus);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Receptionist added successfully!");
                LoadData(); // Refresh data
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void dataGridViewforuser_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView.Rows[e.RowIndex];
                txtName.Text = row.Cells["Name"].Value.ToString();
                txtReceptionistID.Text = row.Cells["ReceptionistID"].Value.ToString();
                txtPhoneNumber.Text = row.Cells["PhoneNumber"].Value.ToString();
                txtEmail.Text = row.Cells["Email"].Value.ToString();
                txtHomeAddress.Text = row.Cells["HomeAddress"].Value.ToString();
                cboxEmploymentStatus.Text = row.Cells["EmploymentStatus"].Value.ToString();
            }
        }


        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string searchKeyword = txtSearch.Text;

                if (string.IsNullOrWhiteSpace(searchKeyword))
                {
                    MessageBox.Show("Please enter a keyword to search.");
                    return;
                }

                string query = "SELECT * FROM Receptionists WHERE ReceptionistID LIKE @SearchKeyword OR Name LIKE @SearchKeyword";

                using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(query, connection))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@SearchKeyword", "%" + searchKeyword + "%");
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dataGridView.DataSource = dt; // Bind search results to DataGridView
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void btnupdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtReceptionistID.Text))
                {
                    MessageBox.Show("Please select a receptionist to update.");
                    return;
                }

                int receptionistID = int.Parse(txtReceptionistID.Text); // Ensure ReceptionistID is filled
                string name = txtName.Text;
                string phoneNumber = txtPhoneNumber.Text;
                string email = txtEmail.Text;
                string homeAddress = txtHomeAddress.Text;
                string employmentStatus = cboxEmploymentStatus.SelectedItem?.ToString(); // Handle null

                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(phoneNumber) ||
                    string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(homeAddress) ||
                    string.IsNullOrWhiteSpace(employmentStatus))
                {
                    MessageBox.Show("Please fill all the fields.");
                    return;
                }

                string query = "UPDATE Receptionists SET Name = @Name, PhoneNumber = @PhoneNumber, Email = @Email, " +
                               "HomeAddress = @HomeAddress, EmploymentStatus = @EmploymentStatus WHERE ReceptionistID = @ReceptionistID";

                using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@ReceptionistID", receptionistID);
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@HomeAddress", homeAddress);
                        cmd.Parameters.AddWithValue("@EmploymentStatus", employmentStatus);

                        connection.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Receptionist updated successfully!");
                        }
                        else
                        {
                            MessageBox.Show("No records were updated. Please check if the ReceptionistID exists.");
                        }
                    }
                }

                LoadData(); // Refresh data
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void btndelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtReceptionistID.Text))
                {
                    MessageBox.Show("Please select a receptionist to delete.");
                    return;
                }

                int receptionistID = int.Parse(txtReceptionistID.Text); // Ensure ReceptionistID is filled

                string query = "DELETE FROM Receptionists WHERE ReceptionistID = @ReceptionistID";

                using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@ReceptionistID", receptionistID);

                        connection.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Receptionist deleted successfully!");
                LoadData(); // Refresh data
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void btnClearReceptionistFields_Click(object sender, EventArgs e)
        {
            txtReceptionistID.Clear();
            txtName.Clear();
            txtPhoneNumber.Clear();
            txtEmail.Clear();
            txtHomeAddress.Clear();
            cboxEmploymentStatus.SelectedIndex = -1;
            txtReceptionistID.Focus();
        }

        private void btnbacktomain_Click(object sender, EventArgs e)
        {
             this.Hide(); // Hide current form
             ManagerMain mainForm = new ManagerMain();
             mainForm.Show(); // Open the main form
            
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}

