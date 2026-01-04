using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsProgramDesign
{
    public partial class ManagerMain : Form
    {
        public ManagerMain()
        {
            InitializeComponent();
        }

        private void btnTrainer_Click(object sender, EventArgs e)
        {
            Trainer trainerMain = new Trainer();
            trainerMain.FormClosed += (s, args) => this.Show();
            this.Hide();
            trainerMain.Show();
        }

        private void btnTraningAppointment_Click(object sender, EventArgs e)
        {
            TraningAppointment trainerApp = new TraningAppointment();
            trainerApp.FormClosed += (s, args) => this.Show();
            this.Hide();
            trainerApp.Show();
        }

        private void btngymMember_Click(object sender, EventArgs e)
        {
            Members gymMemberMain = new Members();
            gymMemberMain.FormClosed += (s, args) => this.Show();
            this.Hide();
            gymMemberMain.Show();
        }

        private void btnProduct_Click(object sender, EventArgs e)
        {
            Product inventoryMain = new Product();
            inventoryMain.FormClosed += (s, args) => this.Show();
            this.Hide();
            inventoryMain.Show();
        }

        private void btnReceptionistList_Click(object sender, EventArgs e)
        {
            ReceptionistList receptionistList = new ReceptionistList();
            receptionistList.FormClosed += (s, args) => this.Show();
            this.Hide();
            receptionistList.Show();
        }

        private void btnGenerateReport_Click(object sender, EventArgs e)
        {
            GenerateReport report = new GenerateReport();
            report.FormClosed += (s, args) => this.Show();
            this.Hide();
            report.Show();
        }

        private void btnTrainingSessions_Click(object sender, EventArgs e)
        {
            TrainingSession TrainingSessionBooking = new TrainingSession();
            TrainingSessionBooking.FormClosed += (s, args) => this.Show();
            this.Hide();
            TrainingSessionBooking.Show();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to logout?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                Home loginForm = new Home();
                loginForm.Show();
                this.Close();
            }
        }
    }
}
