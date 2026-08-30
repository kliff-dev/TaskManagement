using System.Data.Common;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics.Metrics;
using Microsoft.VisualBasic;
using System.Collections.ObjectModel;
using static TaskManagement.MainWindow;
using static TaskManagement.Window2;

namespace TaskManagement
{
    public partial class TaskDialog : Window
    {
   
        private int CurrentUserId;
    

        public ObservableCollection<MainWindow.Task> Tasks { get; }


        public TaskDialog(int currentUserId, ObservableCollection<MainWindow.Task> tasks)
        { 
            InitializeComponent();
            CurrentUserId = currentUserId;
            Tasks = tasks;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        
        {
            // Collect task details from input fields
            string title = TitleTextBox.Text;
            string description = DescriptionTextBox.Text;
            DateTime? dueDate = DueDatePicker.SelectedDate; 

            string priority = PriorityComboBox.Text;
            string category = CategoryComboBox.Text;



            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Title cannot be empty.");
                return;
            }
            if (string.IsNullOrWhiteSpace(description))
            {
                MessageBox.Show("Desciprtion cannot be empty.");
                return;
            }
            if (!dueDate.HasValue)
            {
                MessageBox.Show("Please select a due date.");
                return;
            }

            if (string.IsNullOrWhiteSpace(priority))
            {
                MessageBox.Show("Please select a priority.");
                return;
            }
            if (string.IsNullOrWhiteSpace(category))
            {
                MessageBox.Show("Please select a category.");
                return;
            }



            // Insert task into the database
            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Login;Integrated Security=True;";
            string query = "INSERT INTO Tasks (UserId, Title, Description, DueDate, Priority, Category, Completed) VALUES (@UserId, @title, @description, @dueDate, @priority, @category,   @Completed)";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))

                    {
                        command.Parameters.AddWithValue("@Completed", 0);
                        command.Parameters.AddWithValue("@UserId", CurrentUserId);
                        command.Parameters.AddWithValue("@title", title);
                        command.Parameters.AddWithValue("@description", description);
                        command.Parameters.AddWithValue("@dueDate", dueDate.HasValue ? dueDate.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@priority", priority);
                        command.Parameters.AddWithValue("@category", category);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Task added successfully!");

                            // Add the new task to the shared ObservableCollection
                            Tasks.Add(new MainWindow.Task
                            {
                                Title = title,
                                Description = description,
                                DueDate = dueDate.Value,
                                Priority = priority,
                                Category = category
                            });

                            this.Close(); // Close the dialog
                        }
                        else
                        {
                            MessageBox.Show("Failed to add the task.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    


    }
}
