using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Npgsql;

namespace wpfMeds
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private string constring = "Host=localhost;Username=postgres;Password=VEST777berto;Database=postgres";

        public class Data
        {
            public string Name { get; set; }
            public int count { get; set; }
            public int price { get; set; }
            public string category { get; set; }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string query = "SELECT med.name, med.count, med.price, category.desk FROM ticket4.med JOIN ticket4.category ON med.category = category.id;";
            var data = new List<Data>();
            using (var conn = new NpgsqlConnection(constring))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        data.Add(new Data
                        {
                            Name = reader.GetString(0),
                            count = reader.GetInt32(1),
                            price = reader.GetInt32(2),
                            category = reader.GetString(3)
                        });
                    }

                    gd_meds.ItemsSource = data;
                }
            }
        }

        private void bt_filter_Click(object sender, RoutedEventArgs e)
        {
            string category = null;
            if (cb_category.SelectedItem is ComboBoxItem selectedCategory)
            {
                category = selectedCategory.Content.ToString(); 
            }

            string name = tb_name.Text;
            string query = "SELECT med.name, med.count, med.price, category.desk FROM ticket4.med JOIN ticket4.category ON med.category = category.id WHERE 1=1";

            if (!string.IsNullOrEmpty(category))
            {
                query += " AND category.desk = @Category";
            }

            if (!string.IsNullOrEmpty(name))
            {
                query += " AND med.name ILIKE @Name";
            }

            var data = new List<Data>();
            using (var conn = new NpgsqlConnection(constring))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    if (!string.IsNullOrEmpty(category))
                    {
                        cmd.Parameters.AddWithValue("@Category", category);
                    }
                    if (!string.IsNullOrEmpty(name))
                    {
                        cmd.Parameters.AddWithValue("@Name", $"%{name}%");
                    }

                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        data.Add(new Data
                        {
                            Name = reader.GetString(0),
                            count = reader.GetInt32(1),
                            price = reader.GetInt32(2),
                            category = reader.GetString(3)
                        });
                    }
                }
            }

            gd_meds.ItemsSource = data;
        }

    }
}
