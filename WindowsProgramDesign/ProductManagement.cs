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
    
    public partial class Product : Form
    {
        public Product()
        {
            InitializeComponent();
            LoadData();
        }
        
        private void LoadData()
        {
            try
            {
                string query = "SELECT * FROM Products";

                using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
                using (SqlDataAdapter da = new SqlDataAdapter(query, connection))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView.DataSource = dt; // Bind data to DataGridView
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}");
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtProductName.Text) || 
                    string.IsNullOrWhiteSpace(txtProductCategory.Text) ||
                    string.IsNullOrWhiteSpace(txtUnitsOrdered.Text) ||
                    string.IsNullOrWhiteSpace(txtUnitsInStock.Text) ||
                    string.IsNullOrWhiteSpace(txtUnitsSold.Text) ||
                    string.IsNullOrWhiteSpace(txtSellingPrice.Text))
                {
                    MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string productName = txtProductName.Text.Trim();
                string productCategory = txtProductCategory.Text.Trim();
                
                if (!int.TryParse(txtUnitsOrdered.Text, out int unitsOrdered) || unitsOrdered < 0)
                {
                    MessageBox.Show("Units Ordered must be a valid non-negative number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                if (!int.TryParse(txtUnitsInStock.Text, out int unitsInStock) || unitsInStock < 0)
                {
                    MessageBox.Show("Units In Stock must be a valid non-negative number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                if (!int.TryParse(txtUnitsSold.Text, out int unitsSold) || unitsSold < 0)
                {
                    MessageBox.Show("Units Sold must be a valid non-negative number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                if (!decimal.TryParse(txtSellingPrice.Text, out decimal sellingPrice) || sellingPrice < 0)
                {
                    MessageBox.Show("Selling Price must be a valid non-negative number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string query = "INSERT INTO Products (ProductName, ProductCategory, UnitsOrdered, UnitsInStock, UnitsSold, SellingPrice) " +
                               "VALUES (@ProductName, @ProductCategory, @UnitsOrdered, @UnitsInStock, @UnitsSold, @SellingPrice)";

                using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ProductName", productName);
                        command.Parameters.AddWithValue("@ProductCategory", productCategory);
                        command.Parameters.AddWithValue("@UnitsOrdered", unitsOrdered);
                        command.Parameters.AddWithValue("@UnitsInStock", unitsInStock);
                        command.Parameters.AddWithValue("@UnitsSold", unitsSold);
                        command.Parameters.AddWithValue("@SellingPrice", sellingPrice);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Product added successfully!");
                LoadData();
                btnClearProduct_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                int productID = int.Parse(txtProductID.Text); // Ensure ProductID is filled
                string productName = txtProductName.Text;
                string productCategory = txtProductCategory.Text;
                int unitsOrdered = int.Parse(txtUnitsOrdered.Text);
                int unitsInStock = int.Parse(txtUnitsInStock.Text);
                int unitsSold = int.Parse(txtUnitsSold.Text);
                decimal sellingPrice = decimal.Parse(txtSellingPrice.Text);

                string query = "UPDATE Products SET ProductName = @ProductName, ProductCategory = @ProductCategory, " +
                               "UnitsOrdered = @UnitsOrdered, UnitsInStock = @UnitsInStock, UnitsSold = @UnitsSold, SellingPrice = @SellingPrice " +
                               "WHERE ProductID = @ProductID";

                using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ProductID", productID);
                        command.Parameters.AddWithValue("@ProductName", productName);
                        command.Parameters.AddWithValue("@ProductCategory", productCategory);
                        command.Parameters.AddWithValue("@UnitsOrdered", unitsOrdered);
                        command.Parameters.AddWithValue("@UnitsInStock", unitsInStock);
                        command.Parameters.AddWithValue("@UnitsSold", unitsSold);
                        command.Parameters.AddWithValue("@SellingPrice", sellingPrice);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Product updated successfully!");
                LoadData(); // Refresh data
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
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

                string query = "SELECT * FROM Products WHERE ProductID LIKE @SearchKeyword OR ProductCategory LIKE @SearchKeyword";

                using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
                using (SqlDataAdapter da = new SqlDataAdapter(query, connection))
                {
                    da.SelectCommand.Parameters.AddWithValue("@SearchKeyword", "%" + searchKeyword + "%");
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching for products: {ex.Message}");
            }
        }

        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow row = dataGridView.Rows[e.RowIndex];

                    // Populate the form fields with the selected row data
                    txtProductID.Text = row.Cells["ProductID"].Value.ToString();
                    txtProductName.Text = row.Cells["ProductName"].Value.ToString();
                    txtProductCategory.Text = row.Cells["ProductCategory"].Value.ToString();
                    txtUnitsOrdered.Text = row.Cells["UnitsOrdered"].Value.ToString();
                    txtUnitsInStock.Text = row.Cells["UnitsInStock"].Value.ToString();
                    txtUnitsSold.Text = row.Cells["UnitsSold"].Value.ToString();
                    txtSellingPrice.Text = row.Cells["SellingPrice"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting product: {ex.Message}");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                int productID = int.Parse(txtProductID.Text); // Get the ProductID from the form

                string query = "DELETE FROM Products WHERE ProductID = @ProductID";

                using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ProductID", productID);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Product deleted successfully!");
                LoadData();
                btnClearProduct_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting product: {ex.Message}");
            }
        }

        private void btnClearProduct_Click(object sender, EventArgs e)
        {
            // Clear all input fields
            txtProductID.Clear();
            txtProductName.Clear();
            txtProductCategory.Clear();
            txtUnitsOrdered.Clear();
            txtUnitsInStock.Clear();
            txtUnitsSold.Clear();
            txtSellingPrice.Clear();
            txtSearch.Clear();
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void Product_Load(object sender, EventArgs e)
        {

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

                txtProductID.Text = row.Cells["ProductID"].Value?.ToString();
                txtProductName.Text = row.Cells["ProductName"].Value?.ToString();
                txtProductCategory.Text = row.Cells["ProductCategory"].Value?.ToString();
                txtUnitsOrdered.Text = row.Cells["UnitsOrdered"].Value?.ToString();
                txtUnitsInStock.Text = row.Cells["UnitsInStock"].Value?.ToString();
                txtUnitsSold.Text = row.Cells["UnitsSold"].Value?.ToString();
                txtSellingPrice.Text = row.Cells["SellingPrice"].Value?.ToString();
            }
        }

        private void btnDelete_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtProductID.Text))
                {
                    MessageBox.Show("Please select a product to delete.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int productID = int.Parse(txtProductID.Text);

                string query = "DELETE FROM Products WHERE ProductID = @ProductID";

                using (SqlConnection connection = new SqlConnection(DatabaseConfig.ConnectionString))
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ProductID", productID);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Product deleted successfully!");
                LoadData();
                btnClearProduct_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting product: {ex.Message}");
            }
        }
    }
}

