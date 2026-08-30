using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using static TaskManagement.Window2;

namespace TaskManagement
{
    /// <summary>
    /// Interaction logic for Window4.xaml
    /// </summary>
    public partial class Window4 : Window
    {
        private int CurrentUserID;

        public Window4(int userId)
        {
            CurrentUserID = userId;
            InitializeComponent();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string newUsername = UsernameTextBox.Text;


            if (string.IsNullOrWhiteSpace(newUsername))
            {
                MessageBox.Show("Username cannot be empty.");
                return;
            }
       
      

            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Login;Integrated Security=True;";
            string query = "UPDATE Users SET Username = @Username WHERE UserID = @UserId";
            string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @username";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@username", newUsername);

                        // Execute the query to count matching usernames
                        int userCount = (int)checkCommand.ExecuteScalar();

                        if (userCount > 0)
                        {
                            // If the username already exists
                            MessageBox.Show("Username already exists. Please choose a different username.");
                            return; // Exit the method to prevent inserting the user
                        }
                    }
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", newUsername);
               
                        command.Parameters.AddWithValue("@UserId", CurrentUserID); // Replace with the ID of the user you want to update.

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Username and password updated successfully.");
                        }
                        else
                        {
                            MessageBox.Show("Update failed. Please check the user ID.");
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
  
            string newPassword = PasswordBox.Password; // Use PasswordBox for secure password input.

            if ( string.IsNullOrWhiteSpace(newPassword))
            {
                MessageBox.Show("Username and password cannot be empty.");
                return;
            }

            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Login;Integrated Security=True;";
            string query = "UPDATE Users SET Password = @Password WHERE UserID = @UserId";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {

                        command.Parameters.AddWithValue("@Password", newPassword);
                        command.Parameters.AddWithValue("@UserId", CurrentUserID);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Username and password updated successfully.");
                        }
                        else
                        {
                            MessageBox.Show("Update failed. Please check the user ID.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void UsernameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
