using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security;
using System.Windows;
using static TaskManagement.Window2;

namespace TaskManagement
{
    public partial class Window2 : Window
    {
        private int CurrentUserID;
        public Window2(int userId)
        {
            CurrentUserID = userId;
            InitializeComponent();
            LoadUserData();
        }
        public class User
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public int UserID { get; set; }
        }
      
        // Method to load user data into the DataGrid
        List<User> users = new List<User>();
        public void LoadUserData()
        {
            int count = 0;
            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Login;Integrated Security=True;";
            string query = "SELECT UserID, Username, Password FROM Users";


            try
            {
       

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        SqlDataReader reader = command.ExecuteReader();

                        // Check if any rows are returned
                        if (!reader.HasRows)
                        {
                            MessageBox.Show("No users found in the database.");
                            return;
                        }

                        // Read the data and add it to the list
                        while (reader.Read())
                        {
                            count++;
                            if (reader["Username"].ToString() == "admin") {
                                return;
                            }
                            users.Add(new User
                            {
                               
                                Username = reader["Username"].ToString(),
                                Password = reader["Password"].ToString(), // Ideally, do not show passwords
                                UserID = Convert.ToInt32(reader["UserID"])


                            });
                        }
                    }
                }

                // Bind the list of users to the DataGrid
                Z.Text = CurrentUserID.ToString();  
               
                UsersListView.ItemsSource = users;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }
      

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (users == null)
            {
                MessageBox.Show("The users list is not initialized.");
                return;
            }

            // Get the selected user
            User selectedUser = (User)UsersListView.SelectedItem;

            // Check if a user is selected
            if (selectedUser == null)
            {
                MessageBox.Show("Please select a user to delete.");
                return;
            }

            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Login;Integrated Security=True;";
            string query = "DELETE FROM Users WHERE Username = @username";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", selectedUser.Username);

                        int rowsAffected = command.ExecuteNonQuery();
                        
                        if (rowsAffected > 0)
                        {
                            // Remove the user from the local list
                            users.Remove(selectedUser);

                            // Refresh the ListView binding
                            UsersListView.ItemsSource = null;
                            UsersListView.ItemsSource = users;

                            MessageBox.Show($"User '{selectedUser.Username}' deleted successfully!");
                        }
                        else
                        {
                            MessageBox.Show("Failed to delete the user from the database.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
