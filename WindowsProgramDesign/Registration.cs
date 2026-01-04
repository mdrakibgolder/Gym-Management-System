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
    public partial class Registration : Form
    {
        public Registration()
        {
            InitializeComponent();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            // Label click event - no action needed
        }

        private void label1_Click(object sender, EventArgs e)
        {
            // Label click event - no action needed
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Home loginForm = new Home();
            loginForm.Show();
            this.Close();
        }

        private void Registration_Load(object sender, EventArgs e)
        {
            // Hide Manager ID field - registration is only for Receptionists
            label2.Visible = false;
            textBox8.Visible = false;
            textBox8.Text = "0";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Register button - create new user account
            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(textBox1.Text) || 
                    string.IsNullOrWhiteSpace(textBox7.Text) ||
                    string.IsNullOrWhiteSpace(textBox6.Text) ||
                    string.IsNullOrWhiteSpace(textBox5.Text) ||
                    string.IsNullOrWhiteSpace(textBox4.Text) ||
                    string.IsNullOrWhiteSpace(textBox2.Text))
                {
                    MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string receptionistID = textBox1.Text.Trim();
                string password = textBox7.Text.Trim();
                string name = textBox6.Text.Trim();
                string phoneNo = textBox5.Text.Trim();
                string email = textBox4.Text.Trim();
                string address = textBox3.Text.Trim();
                string employment = textBox2.Text.Trim();

                // Validate email format
                if (!email.Contains("@") || !email.Contains("."))
                {
                    MessageBox.Show("Please enter a valid email address.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validate Receptionist ID
                if (string.IsNullOrWhiteSpace(receptionistID) || receptionistID == "0")
                {
                    MessageBox.Show("Please provide a valid Receptionist ID.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Registration is only for Receptionist role
                string role = "Receptionist";
                string username = "receptionist_" + receptionistID;

                // Check if username already exists
                using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
                {
                    connection.Open();
                    
                    string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, connection))
                    {
                        checkCmd.Parameters.AddWithValue("@Username", username);
                        int count = (int)checkCmd.ExecuteScalar();
                        
                        if (count > 0)
                        {
                            MessageBox.Show("Username already exists. Please use a different ID.", "Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    // Insert into Users table
                    string insertUserQuery = @"INSERT INTO Users (Username, Password, Role, Status) 
                                              VALUES (@Username, @Password, @Role, 'Active')";
                    
                    using (SqlCommand insertCmd = new SqlCommand(insertUserQuery, connection))
                    {
                        insertCmd.Parameters.AddWithValue("@Username", username);
                        insertCmd.Parameters.AddWithValue("@Password", password);
                        insertCmd.Parameters.AddWithValue("@Role", role);
                        insertCmd.ExecuteNonQuery();
                    }

                    // Insert into Receptionists table
                    string insertReceptionistQuery = @"INSERT INTO Receptionists (Name, PhoneNumber, Email, HomeAddress, EmploymentStatus) 
                                                      VALUES (@Name, @PhoneNumber, @Email, @Address, @Employment)";
                    
                    using (SqlCommand insertRecCmd = new SqlCommand(insertReceptionistQuery, connection))
                    {
                        insertRecCmd.Parameters.AddWithValue("@Name", name);
                        insertRecCmd.Parameters.AddWithValue("@PhoneNumber", phoneNo);
                        insertRecCmd.Parameters.AddWithValue("@Email", email);
                        insertRecCmd.Parameters.AddWithValue("@Address", address);
                        insertRecCmd.Parameters.AddWithValue("@Employment", employment);
                        insertRecCmd.ExecuteNonQuery();
                    }

                    // Insert into ReceptionistProfile
                    string insertProfileQuery = @"INSERT INTO ReceptionistProfile (ReceptionistID, Password, Name, PhoneNo, Email, Address, Employment) 
                                                 VALUES (@ReceptionistID, @Password, @Name, @PhoneNo, @Email, @Address, @Employment)";
                    
                    using (SqlCommand insertProfCmd = new SqlCommand(insertProfileQuery, connection))
                    {
                        insertProfCmd.Parameters.AddWithValue("@ReceptionistID", receptionistID);
                        insertProfCmd.Parameters.AddWithValue("@Password", password);
                        insertProfCmd.Parameters.AddWithValue("@Name", name);
                        insertProfCmd.Parameters.AddWithValue("@PhoneNo", phoneNo);
                        insertProfCmd.Parameters.AddWithValue("@Email", email);
                        insertProfCmd.Parameters.AddWithValue("@Address", address);
                        insertProfCmd.Parameters.AddWithValue("@Employment", employment);
                        insertProfCmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show($"Registration successful!\n\nUsername: {username}\nRole: {role}\n\nYou can now login with your credentials.", 
                               "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                Home loginForm = new Home();
                loginForm.Show();
                this.Close();
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Database Error: {sqlEx.Message}", "Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearFields()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            textBox7.Clear();
            textBox8.Clear();
            textBox1.Focus();
        }
    }
}
