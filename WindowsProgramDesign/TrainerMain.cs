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
    public partial class Trainer : Form
    {
        public Trainer()
        {
            InitializeComponent();   
        }

        private void Trainer_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM Trainers"; // Ensure table name is "Trainers"
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);
                    dataGridView.DataSource = dataTable; // Bind data to the DataGridView
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTrainerID.Text))
            {
                MessageBox.Show("Please enter a Trainer ID to delete.");
                return;
            }

            try
            {
                string query = "DELETE FROM Trainers WHERE TrainerID = @TrainerID";

                using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@TrainerID", txtTrainerID.Text);

                        connection.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Record deleted successfully!");
                LoadData(); // Refresh data
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
                if (string.IsNullOrWhiteSpace(txtName.Text) || 
                    string.IsNullOrWhiteSpace(txtEmail.Text) || 
                    string.IsNullOrWhiteSpace(txtPhoneNumber.Text) ||
                    cboxEmployment.SelectedItem == null)
                {
                    MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string name = txtName.Text.Trim();
                string email = txtEmail.Text.Trim();
                string PhoneNumber = txtPhoneNumber.Text.Trim();
                string address = txtAddress.Text.Trim();
                string employmentStatus = cboxEmployment.SelectedItem.ToString();

                string query = "INSERT INTO Trainers (Name, Email, PhoneNumber, Address, EmploymentStatus) VALUES (@Name, @Email, @PhoneNumber, @Address, @EmploymentStatus)";

                using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", name);
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@PhoneNumber", PhoneNumber);
                        command.Parameters.AddWithValue("@Address", string.IsNullOrWhiteSpace(address) ? (object)DBNull.Value : address);
                        command.Parameters.AddWithValue("@EmploymentStatus", employmentStatus);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Record added successfully!");
                LoadData();
                btnClearFields_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }


        }
        
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridView.CurrentRow == null)
            {
                MessageBox.Show("Please select a record from the table to update.");
                return;
            }

            string trainerID = dataGridView.CurrentRow.Cells["TrainerID"].Value?.ToString(); // Assumes TrainerID column exists
            if (string.IsNullOrWhiteSpace(trainerID))
            {
                MessageBox.Show("No TrainerID found for the selected row.");
                return;
            }

            string query = "UPDATE Trainers SET " +
                           "Name = @Name, " +
                           "PhoneNumber = @PhoneNumber, " +
                           "Email = @Email, " +
                           "Address = @Address, " +
                           "EmploymentStatus = @EmploymentStatus " +
                           "WHERE TrainerID = @TrainerID";

            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@TrainerID", trainerID);
                        cmd.Parameters.AddWithValue("@Name", string.IsNullOrWhiteSpace(txtName.Text) ?
                            dataGridView.CurrentRow.Cells["Name"].Value : txtName.Text);
                        cmd.Parameters.AddWithValue("@PhoneNumber", string.IsNullOrWhiteSpace(txtPhoneNumber.Text) ?
                            dataGridView.CurrentRow.Cells["PhoneNumber"].Value : txtPhoneNumber.Text);
                        cmd.Parameters.AddWithValue("@Email", string.IsNullOrWhiteSpace(txtEmail.Text) ?
                            dataGridView.CurrentRow.Cells["Email"].Value : txtEmail.Text);
                        cmd.Parameters.AddWithValue("@Address", string.IsNullOrWhiteSpace(txtAddress.Text) ?
                            dataGridView.CurrentRow.Cells["Address"].Value : txtAddress.Text);
                        cmd.Parameters.AddWithValue("@EmploymentStatus", string.IsNullOrWhiteSpace(cboxEmployment.Text) ?
                            dataGridView.CurrentRow.Cells["EmploymentStatus"].Value : cboxEmployment.Text);

                        connection.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Record updated successfully!");
                        }
                        else
                        {
                            MessageBox.Show("No records were updated. Please check the selected row.");
                        }
                    }
                }

                LoadData(); // Refresh data after the update
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"SQL Error: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void btnsearch_Click(object sender, EventArgs e)
        {
            try
            {
                string searchName = txtSearch.Text;
                if (string.IsNullOrWhiteSpace(searchName))
                {
                    MessageBox.Show("Please enter a name to search.");
                    return;
                }

                string query = "SELECT * FROM Trainers WHERE Name LIKE @Name";

                using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(query, connection))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@Name", "%" + searchName + "%");
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

        private void btnClearFields_Click(object sender, EventArgs e)
        {
            txtTrainerID.Clear();
            txtName.Clear();
            txtPhoneNumber.Clear();
            txtEmail.Clear();
            txtAddress.Clear();
            cboxEmployment.SelectedIndex = -1;
            txtTrainerID.Focus();
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView.Rows[e.RowIndex];
                txtTrainerID.Text = row.Cells["TrainerID"].Value?.ToString();
                txtName.Text = row.Cells["Name"].Value?.ToString();
                txtPhoneNumber.Text = row.Cells["PhoneNumber"].Value?.ToString();
                txtEmail.Text = row.Cells["Email"].Value?.ToString();
                txtAddress.Text = row.Cells["Address"].Value?.ToString();
                cboxEmployment.Text = row.Cells["EmploymentStatus"].Value?.ToString();
            }

        }
    }

}

