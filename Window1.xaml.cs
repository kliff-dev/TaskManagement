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
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Collections;
using System.Configuration;
using System.Diagnostics.Metrics;
namespace TaskManagement
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
   
        }
        private int CurrentUserId;
        private void Login(object sender, RoutedEventArgs e)
        {
           
            string username = User.Text;
            string password = Pass.Password;

            // Connection string
            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Login;Integrated Security=True;";

            // SQL query
            string query = "SELECT UserID FROM Users WHERE Username = @username AND Password = @password";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@password", password);

                        object result = command.ExecuteScalar();
                        if (result != null)
                        {
                            CurrentUserId = Convert.ToInt32(result);
                            MessageBox.Show("Login successful!");

                            if (CurrentUserId == 1)
                            {
                                Window2 window2 = new Window2(CurrentUserId);
                                window2.Show();
                                this.Close();
                            }
                            else {

                                Window3 window3 = new Window3(CurrentUserId);
                                window3.Show();
                                this.Close();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Invalid username or password.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void Create(object sender, RoutedEventArgs e)
        {
            string user = User.Text;
            string pass = Pass.Password;

            // Connection string
            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Login;Integrated Security=True;";

            // SQL query to check if the username exists
            string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @username";

            // SQL query to insert the new user
            string insertQuery = "INSERT INTO Users (Username, Password) VALUES (@username, @password)";
            if (string.IsNullOrWhiteSpace(user))
            {
                MessageBox.Show("Username cannot be empty.");
                return;
            }
            if (string.IsNullOrWhiteSpace(pass))
            {
                MessageBox.Show("Password cannot be empty.");
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Check if the username already exists
                    using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@username", user);

                        // Execute the query to count matching usernames
                        int userCount = (int)checkCommand.ExecuteScalar();

                        if (userCount > 0)
                        {
                            // If the username already exists
                            MessageBox.Show("Username already exists. Please choose a different username.");
                            return; // Exit the method to prevent inserting the user
                        }
                    }

                    // If the username doesn't exist, insert the new user
                    using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@username", user);
                        insertCommand.Parameters.AddWithValue("@password", pass); // Ideally, password should be hashed

                        // Execute the insert query
                        int rowsAffected = insertCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("User added successfully!");
                        }
                        else
                        {
                            MessageBox.Show("Failed to add user.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

    }
    }

