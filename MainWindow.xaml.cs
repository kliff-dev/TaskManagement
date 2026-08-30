using Microsoft.VisualBasic;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using static TaskManagement.Window2;
using CustomTask = TaskManagement.MainWindow.Task;
namespace TaskManagement
{
    public partial class MainWindow : Window, INotifyPropertyChanged 
    {

        private System.Timers.Timer notificationTimer;
        public ObservableCollection<Task> taskList { get; set; } = new ObservableCollection<Task>();
        public ObservableCollection<Task> filteredTasks { get; set; }
        public ObservableCollection<Task> CompletedTaskList{ get; set; }

        private int CurrentUserId;


        public MainWindow(int userId)
        {
            InitializeComponent();

            CurrentUserId = userId;
            filteredTasks = new ObservableCollection<Task>(taskList);

            this.DataContext = this;
            CompletedTaskList = new ObservableCollection<Task>(taskList);
            PriorityFilterComboBox.SelectedIndex = 0; 
            CategoryFilterComboBox.SelectedIndex = 0;


            LoadTasks();
     CompleteLoadTasks();

      



        }
        private void StartNotificationTimer()
        {
            notificationTimer = new System.Timers.Timer(60000); // Check every minute
            notificationTimer.Elapsed += CheckDueDates;
            notificationTimer.Start();
        }

        private void CheckDueDates(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var now = DateTime.Now;
                var notificationThreshold = now.AddDays(1); // Set the threshold to one day from now

                foreach (var task in taskList)
                {
                    // Reset notification status if the task is overdue
                    if (task.DueDate < now)
                    {
                        task.IsNotificationShown = false; // Reset for overdue tasks
                    }

                    // Check if the task is not complete and due within the next day
                    if (!task.IsComplete && task.DueDate <= notificationThreshold && task.DueDate > now)
                    {
                        // Check if the notification has already been shown
                        if (!task.IsNotificationShown)
                        {
                            // Display notification for tasks nearing due date
                            MessageBox.Show(
                                $"Task \"{task.Title}\" is due in less than 24 hours: {task.DueDate:dd/MM/yyyy}",
                                "Due Date Notification",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);

                            // Mark that the notification has been shown to avoid duplicate alerts
                            task.IsNotificationShown = true;
                        }
                    }
                }
            });
        }
        private void LoadTasks()
        {
            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Login;Integrated Security=True;";
            string query = "SELECT Title, Description, DueDate, Priority, Category FROM Tasks WHERE UserID = @userId AND Completed = @Completed";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", CurrentUserId);
                        command.Parameters.AddWithValue("@Completed", 0);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                         
                            while (reader.Read())
                            {
                                DateTime dueDate = reader["DueDate"] != DBNull.Value
         ? (DateTime)reader["DueDate"] // Convert DBNull to DateTime
         : DateTime.MinValue;          // Default value if `DueDate` is null
                            taskList.Add(new Task
                                {
                                    Title = reader["Title"].ToString(),
                                    Description = reader["Description"].ToString(),
                                    DueDate = dueDate,
                                    Priority = reader["Priority"].ToString(),
                                    Category = reader["Category"].ToString()
                                });
                            }

                            TaskListView.ItemsSource = taskList;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }
        private void CompleteLoadTasks()
        {
            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Login;Integrated Security=True;";
            string query = "SELECT Title, Description, DueDate, Priority, Category FROM Tasks WHERE UserID = @userId AND Completed = @Completed";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", CurrentUserId);
                        command.Parameters.AddWithValue("@Completed", 1);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                DateTime dueDate = reader["DueDate"] != DBNull.Value
           ? (DateTime)reader["DueDate"] // Convert DBNull to DateTime
           : DateTime.MinValue;          // Default value if `DueDate` is null
                                CompletedTaskList.Add(new Task
                                {
                                    Title = reader["Title"].ToString(),
                                    Description = reader["Description"].ToString(),
                                    DueDate = dueDate,
                                    Priority = reader["Priority"].ToString(),
                                    Category = reader["Category"].ToString()
                                });
                            }

                            CompletedTaskListView.ItemsSource = CompletedTaskList;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }
        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            TaskDialog taskDialog = new TaskDialog(CurrentUserId, taskList);
           taskDialog.ShowDialog();
          

        }

        private void EditTask_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (taskList == null)
            {
                MessageBox.Show("The tasks list is not initialized.");
                return;
            }

            // Get the selected task
            Task selectedTask = (Task)TaskListView.SelectedItem;

            // Check if a task is selected
            if (selectedTask == null)
            {
                MessageBox.Show("Please select a task to delete.");
                return;
            }

            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Login;Integrated Security=True;";
            string query = "DELETE FROM Tasks WHERE Title = @title AND UserId = @userId";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@title", selectedTask.Title);
                        command.Parameters.AddWithValue("@userId", CurrentUserId);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            // Remove the task from the local list
                            taskList.Remove(selectedTask);

                            // Refresh the ListView binding
                            TaskListView.ItemsSource = null;
                            TaskListView.ItemsSource = taskList;

                            MessageBox.Show($"Task '{selectedTask.Title}' deleted successfully!");
                        }
                        else
                        {
                            MessageBox.Show("Failed to delete the task from the database.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters(); 
        }

        private void PriorityFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters(); 
        }

        private void CategoryFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters(); 
        }

        private void ApplyFilters()
        {
            string searchText = SearchTextBox.Text.ToLower();
            string selectedPriority = (PriorityFilterComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string selectedCategory = (CategoryFilterComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            DateTime? startDate = StartDatePicker.SelectedDate;
            DateTime? endDate = EndDatePicker.SelectedDate;
            filteredTasks.Clear();

            foreach (var task in taskList)
            {
                bool matchesSearch = string.IsNullOrEmpty(searchText) ||
                                     task.Title.ToLower().Contains(searchText) ||
                                     task.Description.ToLower().Contains(searchText);
                bool matchesPriority = selectedPriority == "All" || task.Priority == selectedPriority;
                bool matchesCategory = selectedCategory == "All" || task.Category == selectedCategory;
                bool matchesDate = (!startDate.HasValue || task.DueDate >= startDate.Value) &&
                           (!endDate.HasValue || task.DueDate <= endDate.Value);
           
                if (matchesSearch && matchesPriority && matchesCategory && matchesDate)
                {

                    filteredTasks.Add(task);
                    TaskListView.ItemsSource = filteredTasks;

                }
           
            }

        }




        private void StartDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void EndDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public class Task : INotifyPropertyChanged
        {
            private string title;
            private string description;
            private DateTime dueDate;
            private string priority;
            private string category;
            private bool isComplete;
            private bool isNotificationShown;

            public string Title
            {
                get => title;
                set
                {
                    if (title != value)
                    {
                        title = value;
                        OnPropertyChanged(nameof(Title));
                    }
                }
            }

            public string Description
            {
                get => description;
                set
                {
                    if (description != value)
                    {
                        description = value;
                        OnPropertyChanged(nameof(Description));
                    }
                }
            }

            public DateTime DueDate
            {
                get => dueDate;
                set
                {
                    if (dueDate != value)
                    {
                        dueDate = value;
                        OnPropertyChanged(nameof(DueDate));
                    }
                }
            }

            public string Priority
            {
                get => priority;
                set
                {
                    if (priority != value)
                    {
                        priority = value;
                        OnPropertyChanged(nameof(Priority));
                    }
                }
            }

            public string Category
            {
                get => category;
                set
                {
                    if (category != value)
                    {
                        category = value;
                        OnPropertyChanged(nameof(Category));
                    }
                }
            }
            public bool IsComplete
            {
                get => isComplete;
                set
                {
                    isComplete = value;
                    OnPropertyChanged(nameof(IsComplete));
                }
            }

            public bool IsNotificationShown
            {
                get => isNotificationShown;
                set
                {
                    isNotificationShown = value;
                    OnPropertyChanged(nameof(IsNotificationShown));
                }
            }


            // INotifyPropertyChanged implementation
            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private void UpdateTaskAsCompletedInDatabase(int taskId)
        {
            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Login;Integrated Security=True;";
            string query = "UPDATE Tasks SET Completed = 1 WHERE TaskId = @TaskId";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@TaskId", taskId);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while updating the task: {ex.Message}");
            }
        }
        private void CompleteTask_Click(object sender, RoutedEventArgs e)
        {
            Button completeButton = sender as Button;
            if (completeButton != null)
            {
                Task taskToComplete = completeButton.Tag as Task;
                if (taskToComplete != null)
                {
                    CompletedTaskList.Add(taskToComplete);
                    taskList.Remove(taskToComplete);
                    taskToComplete.IsNotificationShown = false; // Reset notification status
                    ApplyFilters();

                    MessageBox.Show("Task Completed!", "Task Status", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }





}