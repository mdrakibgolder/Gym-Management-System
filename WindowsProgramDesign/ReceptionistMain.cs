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
    public partial class ReceptionistMain : Form
    {
        public ReceptionistMain()
        {
            InitializeComponent();
        }

        private void btnTrainers_Click(object sender, EventArgs e)
        {
            Trainer trainer = new Trainer();
            trainer.FormClosed += (s, args) => this.Show();
            this.Hide();
            trainer.Show();
        }

        private void btnMembers_Click(object sender, EventArgs e)
        {
            Members gymMember = new Members();
            gymMember.FormClosed += (s, args) => this.Show();
            this.Hide();
            gymMember.Show();
        }

        private void btnProduct_Click(object sender, EventArgs e)
        {
            Product inventory = new Product();
            inventory.FormClosed += (s, args) => this.Show();
            this.Hide();
            inventory.Show();
        }

        private void btnTraningAppointment_Click(object sender, EventArgs e)
        {
            TraningAppointment appointment = new TraningAppointment();
            appointment.FormClosed += (s, args) => this.Show();
            this.Hide();
            appointment.Show();
        }

        private void btnReceptionistProfile_Click(object sender, EventArgs e)
        {
            ReceptionistProfile profile = new ReceptionistProfile();
            profile.FormClosed += (s, args) => this.Show();
            this.Hide();
            profile.Show();
        }

        private void btnlogout_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to logout?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                Home loginForm = new Home();
                loginForm.Show();
                this.Close();
            }
        }

        private void btnTrainingSession_Click(object sender, EventArgs e)
        {
            TrainingSession profile = new TrainingSession();
            profile.FormClosed += (s, args) => this.Show();
            this.Hide();
            profile.Show();
        }
    }
}
