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
    public partial class Members : Form
    {
        public Members()
        {
            InitializeComponent();
        }
        
        private void Members_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
                {
                    string query = "SELECT * FROM Members";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dataGridView.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }

        private void ClearFields()
        {
            txtSearch.Clear();
            txtMemberID.Clear();
            txtName.Clear();
            txtPhoneNumber.Clear();
            txtEmail.Clear();
            txtAddress.Clear();
            cmbMembershipType.SelectedIndex = -1;
            dtpRegistrationDate.Value = DateTime.Now;
            txtSearch.Focus();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMemberID.Text))
            {
                MessageBox.Show("Please enter a Member ID to delete.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtMemberID.Text, out int memberID))
            {
                MessageBox.Show("Member ID must be a valid integer.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = "DELETE FROM Members WHERE MemberID = @MemberID";

            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.Add("@MemberID", SqlDbType.Int).Value = memberID;

                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Member deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                        
                    }
                    else
                    {
                        MessageBox.Show("No member found with the provided Member ID.", "Delete Failed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"SQL Error: {sqlEx.Message}", "Delete Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Delete Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                int memberId = int.Parse(txtMemberID.Text);
                string name = txtName.Text;
                string PhoneNumber = txtPhoneNumber.Text;
                string email = txtEmail.Text;
                string address = txtAddress.Text;
                string membershipType = cmbMembershipType.Text;
                DateTime registrationDate = dtpRegistrationDate.Value;

                string query = "UPDATE Members SET Name = @Name, PhoneNumber = @PhoneNumber, Email = @Email, Address = @Address, " +
                               "MembershipType = @MembershipType, RegistrationDate = @RegistrationDate WHERE MemberID = @MemberID";

                using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MemberID", memberId);
                        command.Parameters.AddWithValue("@Name", name);
                        command.Parameters.AddWithValue("@PhoneNumber", PhoneNumber);
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Address", address);
                        command.Parameters.AddWithValue("@MembershipType", membershipType);
                        command.Parameters.AddWithValue("@RegistrationDate", registrationDate);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Member updated successfully!");
                LoadData();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                string name = txtName.Text;
                string PhoneNumber = txtPhoneNumber.Text;
                string email = txtEmail.Text;
                string address = txtAddress.Text;
                string membershipType = cmbMembershipType.Text;
                DateTime registrationDate = dtpRegistrationDate.Value;

                string query = "INSERT INTO Members (Name, PhoneNumber, Email, Address, MembershipType, RegistrationDate) " +
                               "VALUES (@Name, @PhoneNumber, @Email, @Address, @MembershipType, @RegistrationDate)";

                using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", name);
                        command.Parameters.AddWithValue("@PhoneNumber", PhoneNumber);
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Address", address);
                        command.Parameters.AddWithValue("@MembershipType", membershipType);
                        command.Parameters.AddWithValue("@RegistrationDate", registrationDate);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Member added successfully!");
                LoadData();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnsearch_Click(object sender, EventArgs e)
        {
            try
            {
                string searchValue = txtSearch.Text;
                using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
                {
                    string query = "SELECT * FROM Members WHERE Name LIKE @Search OR PhoneNumber LIKE @Search";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    adapter.SelectCommand.Parameters.AddWithValue("@Search", "%" + searchValue + "%");
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dataGridView.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnClearMember_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void btnViewAll_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView.Rows[e.RowIndex];

                txtMemberID.Text = row.Cells["MemberID"].Value.ToString();
                txtName.Text = row.Cells["Name"].Value.ToString();
                txtPhoneNumber.Text = row.Cells["PhoneNumber"].Value.ToString();
                txtEmail.Text = row.Cells["Email"].Value.ToString();
                txtAddress.Text = row.Cells["Address"].Value.ToString();
                cmbMembershipType.Text = row.Cells["MembershipType"].Value.ToString();
                dtpRegistrationDate.Value = Convert.ToDateTime(row.Cells["RegistrationDate"].Value);
            }
        }
    }
}
