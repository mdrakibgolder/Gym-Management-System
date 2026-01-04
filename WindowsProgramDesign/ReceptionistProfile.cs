using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace WindowsProgramDesign
{
    public partial class ReceptionistProfile : Form
    {
        
        public ReceptionistProfile()
        {
            InitializeComponent();
            LoadData();
        }
        private void LoadData()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(DatabaseConfig.ConnectionString))
                {
                    // Query to get all data from ReceptionistProfile table
                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM ReceptionistProfile", con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        dataGridView.DataSource = dt;
                    }
                    else
                    {
                        MessageBox.Show("No records found.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private byte[] ImageToByteArray(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                return ms.ToArray();
            }
        }
        private Image ByteArrayToImage(byte[] byteArray)
        {
            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                return Image.FromStream(ms);
            }
        }
        private void SetPlaceholderImage()
        {
            try
            {
                string placeholderImagePath = Path.Combine(Application.StartupPath, "Images", "placeholder_image.png");

                if (File.Exists(placeholderImagePath))
                {
                    pictureBoxProfile.Image = Image.FromFile(placeholderImagePath);
                }
                else
                {
                    MessageBox.Show("Placeholder image not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading placeholder image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReceptionistProfile_Load(object sender, EventArgs e)
        {
            SetPlaceholderImage();
        }

        private void ClearFields()
        {
            txtReceptionistID.Clear();
            txtPassword.Clear();
            txtName.Clear();
            txtPhoneNo.Clear();
            txtEmail.Clear();
            txtAddress.Clear();
            txtEmployment.Clear();
            pictureBoxProfile.Image = null;
            SetPlaceholderImage();
        }
        private void ToggleFields(bool enabled)
        {
            txtReceptionistID.Enabled = enabled;
            txtPassword.Enabled = enabled;
            txtName.Enabled = enabled;
            txtPhoneNo.Enabled = enabled;
            txtEmail.Enabled = enabled;
            txtAddress.Enabled = enabled;
            txtEmployment.Enabled = enabled;
            btn_browse.Enabled = enabled;
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtReceptionistID.Text))
            {
                MessageBox.Show("Please select a Receptionist ID to delete.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult dialog = MessageBox.Show("Are you sure you want to delete this profile?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialog == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(DatabaseConfig.ConnectionString))
                    {
                        con.Open();
                        SqlCommand cmd = new SqlCommand("DELETE FROM ReceptionistProfile WHERE ReceptionistID=@ReceptionistID", con);
                        cmd.Parameters.AddWithValue("@ReceptionistID", txtReceptionistID.Text);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Profile deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting profile: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btn_browse_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                pictureBoxProfile.Image = Image.FromFile(ofd.FileName); // Display selected image
            }
        }

        private void btnSaveProfile_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtReceptionistID.Text))
            {
                MessageBox.Show("Receptionist ID cannot be empty!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DatabaseConfig.ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("UPDATE ReceptionistProfile SET Password=@Password, Name=@Name, PhoneNo=@PhoneNo, Email=@Email, Address=@Address, Employment=@Employment, ProfilePicture=@ProfilePicture WHERE ReceptionistID=@ReceptionistID", con);

                    cmd.Parameters.AddWithValue("@ReceptionistID", txtReceptionistID.Text);
                    cmd.Parameters.AddWithValue("@Password", txtPassword.Text);
                    cmd.Parameters.AddWithValue("@Name", txtName.Text);
                    cmd.Parameters.AddWithValue("@PhoneNo", txtPhoneNo.Text);
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@Address", txtAddress.Text);
                    cmd.Parameters.AddWithValue("@Employment", txtEmployment.Text);

                    // Handle the profile picture
                    byte[] profilePicture = null;
                    if (pictureBoxProfile.Image != null)
                    {
                        profilePicture = ImageToByteArray(pictureBoxProfile.Image);
                    }

                    // Use DBNull.Value if profilePicture is null
                    cmd.Parameters.AddWithValue("@ProfilePicture", (object)profilePicture ?? DBNull.Value);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Profile updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData(); // Refresh DataGridView
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating profile: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEditProfile_Click(object sender, EventArgs e)
        {
            ToggleFields(true);

            btnEditProfile.Text = "Save";
            btnEditProfile.Click -= btnEditProfile_Click;
            btnEditProfile.Click += btnSaveProfile_Click;
        }

        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView.Rows[e.RowIndex];
                txtReceptionistID.Text = row.Cells["ReceptionistID"].Value?.ToString();
                txtPassword.Text = row.Cells["Password"].Value?.ToString();
                txtName.Text = row.Cells["Name"].Value?.ToString();
                txtPhoneNo.Text = row.Cells["PhoneNo"].Value?.ToString();
                txtEmail.Text = row.Cells["Email"].Value?.ToString();
                txtAddress.Text = row.Cells["Address"].Value?.ToString();
                txtEmployment.Text = row.Cells["Employment"].Value?.ToString();

                // Load the profile picture (if available)
                if (row.Cells["ProfilePicture"].Value != DBNull.Value)
                {
                    byte[] imageBytes = (byte[])row.Cells["ProfilePicture"].Value;
                    pictureBoxProfile.Image = ByteArrayToImage(imageBytes);
                }
                else
                {
                    SetPlaceholderImage(); // Set the placeholder image if no picture
                }
            }
        }

        private void btn_backtomain_Click(object sender, EventArgs e)
        {
            this.Close();
            ReceptionistMain mainMenuForm = new ReceptionistMain();
            mainMenuForm.Show();
        }

        private void btnClearFields_Click(object sender, EventArgs e)
        {
            txtReceptionistID.Clear();
            txtPassword.Clear();
            txtName.Clear();
            txtPhoneNo.Clear();
            txtEmail.Clear();
            txtAddress.Clear();
            txtEmployment.Clear();
            pictureBoxProfile.Image = null;
            // Set placeholder image if no profile image is selected
            SetPlaceholderImage();
        }

    }
}


