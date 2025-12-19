using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using LibraryManagementSystem.Utils;

namespace LibraryManagementSystem.MainformsUser
{
    public partial class Settings : UserControl
    {
        SqlConnection connect = Database.GetConnection();

        public Settings()
        {
            InitializeComponent();
            LoadSettings();
        }

        public void refreshData()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)refreshData);
                return;
            }
            LoadSettings();
        }

        private void LoadSettings()
        {
            borrowingPeriodNumeric.Value = AppSettings.GetBorrowingPeriodDays();
            maxBooksNumeric.Value = AppSettings.GetMaxBooksPerUser();
            overdueThresholdNumeric.Value = AppSettings.GetOverdueThresholdDays();
            fineRateNumeric.Value = AppSettings.GetFineRatePerDay();
            maxRenewalsNumeric.Value = AppSettings.GetMaxRenewals();
            renewalDaysNumeric.Value = AppSettings.GetRenewalDays();
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            try
            {
                AppSettings.SetBorrowingPeriodDays((int)borrowingPeriodNumeric.Value);
                AppSettings.SetMaxBooksPerUser((int)maxBooksNumeric.Value);
                AppSettings.SetOverdueThresholdDays((int)overdueThresholdNumeric.Value);
                AppSettings.SetFineRatePerDay(fineRateNumeric.Value);
                AppSettings.SetMaxRenewals((int)maxRenewalsNumeric.Value);
                AppSettings.SetRenewalDays((int)renewalDaysNumeric.Value);

                MessageBox.Show("Settings saved successfully!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                AuditLogger.LogAction("Update Settings", "Settings", null, "System settings updated");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving settings: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void resetBtn_Click(object sender, EventArgs e)
        {
            DialogResult check = MessageBox.Show("Reset all settings to defaults?", "Confirmation", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (check == DialogResult.Yes)
            {
                borrowingPeriodNumeric.Value = 14;
                maxBooksNumeric.Value = 5;
                overdueThresholdNumeric.Value = 3;
                fineRateNumeric.Value = 5.00m;
                maxRenewalsNumeric.Value = 2;
                renewalDaysNumeric.Value = 14;
            }
        }
    }
}

