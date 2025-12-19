using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LibraryManagementSystem.Utils;

namespace LibraryManagementSystem.MainformsUser
{
    public partial class Reports : UserControl
    {
        SqlConnection connect = Database.GetConnection();

        public Reports()
        {
            InitializeComponent();
            InitializeYearComboBox();
            SetupDataGridViews();
            ThemeManager.StyleDataGridView(monthlyStatsDataGrid);
            ThemeManager.StyleDataGridView(popularBooksDataGrid);
            ThemeManager.StyleDataGridView(userActivityDataGrid);
            LoadReports();
        }

        private void InitializeYearComboBox()
        {
            int currentYear = DateTime.Now.Year;
            for (int year = currentYear - 5; year <= currentYear + 1; year++)
            {
                yearComboBox.Items.Add(year);
            }
            yearComboBox.SelectedItem = currentYear;
        }

        private void SetupDataGridViews()
        {
            // Monthly Stats columns
            monthlyStatsDataGrid.Columns.Clear();
            monthlyStatsDataGrid.Columns.Add("Month", "Month");
            monthlyStatsDataGrid.Columns.Add("Issued", "Books Issued");
            monthlyStatsDataGrid.Columns.Add("Returned", "Books Returned");
            monthlyStatsDataGrid.Columns.Add("Overdue", "Overdue Books");
            monthlyStatsDataGrid.Columns.Add("Fines", "Total Fines");

            // Popular Books columns
            popularBooksDataGrid.Columns.Clear();
            popularBooksDataGrid.Columns.Add("Title", "Book Title");
            popularBooksDataGrid.Columns.Add("Author", "Author");
            popularBooksDataGrid.Columns.Add("Issues", "Times Issued");

            // User Activity columns
            userActivityDataGrid.Columns.Clear();
            userActivityDataGrid.Columns.Add("Name", "Student Name");
            userActivityDataGrid.Columns.Add("Issues", "Total Issues");
        }

        public void refreshData()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)refreshData);
                return;
            }
            LoadReports();
        }

        private void LoadReports()
        {
            try
            {
                int year = Convert.ToInt32(yearComboBox.SelectedItem);
                LoadMonthlyStats(year);
                LoadPopularBooks();
                LoadUserActivity();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading reports: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadMonthlyStats(int year)
        {
            var stats = ReportGenerator.GetMonthlyStatistics(year);
            
            monthlyStatsDataGrid.Rows.Clear();
            foreach (var stat in stats)
            {
                monthlyStatsDataGrid.Rows.Add(
                    new DateTime(stat.Year, stat.Month, 1).ToString("MMMM yyyy"),
                    stat.BooksIssued,
                    stat.BooksReturned,
                    stat.OverdueBooks,
                    stat.TotalFines.ToString("C2")
                );
            }
        }

        private void LoadPopularBooks()
        {
            var books = ReportGenerator.GetPopularBooks(10);
            
            popularBooksDataGrid.Rows.Clear();
            foreach (var book in books)
            {
                popularBooksDataGrid.Rows.Add(
                    book.Title,
                    book.Author,
                    book.IssueCount
                );
            }
        }

        private void LoadUserActivity()
        {
            var activity = ReportGenerator.GetUserActivityStats();
            
            userActivityDataGrid.Rows.Clear();
            foreach (var kvp in activity.OrderByDescending(x => x.Value).Take(10))
            {
                userActivityDataGrid.Rows.Add(
                    kvp.Key,
                    kvp.Value
                );
            }
        }

        private void yearComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadReports();
        }

        private void refreshBtn_Click(object sender, EventArgs e)
        {
            LoadReports();
        }
    }
}

