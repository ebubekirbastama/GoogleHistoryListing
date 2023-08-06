using System;
using System.Data;
using System.IO;
using System.Windows.Forms;


namespace googlegirilensitelerizleme
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PopulateDataGridView();
        }
        private void PopulateDataGridView()
        {
            string chromeHistoryPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                @"Google\Chrome\User Data\Default\History"
            );

            if (File.Exists(chromeHistoryPath))
            {
                string tempPath = Path.GetTempFileName();
                File.Copy(chromeHistoryPath, tempPath, true);

                using (var connection = new System.Data.SQLite.SQLiteConnection($"Data Source={tempPath};Version=3;"))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT url, title, last_visit_time FROM urls ORDER BY last_visit_time DESC LIMIT 10;";
                        using (var reader = command.ExecuteReader())
                        {
                            DataTable dt = new DataTable();
                            dt.Columns.Add("URL");
                            dt.Columns.Add("Title");
                            dt.Columns.Add("Last Visit Time");

                            while (reader.Read())
                            {
                                string url = reader.GetString(0);
                                string title = reader.GetString(1);
                                long lastVisitTime = reader.GetInt64(2);

                                DateTime lastVisitDateTime = new DateTime(1601, 1, 1).AddSeconds(lastVisitTime / 1000000);

                                dt.Rows.Add(url, title, lastVisitDateTime);
                            }

                            dataGridView1.DataSource = dt;
                        }
                    }
                }

                File.Delete(tempPath);
            }
            else
            {
                MessageBox.Show("Google Chrome history file not found.");
            }
        }

    }
}
