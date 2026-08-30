# Task Management

Task Management is a Windows desktop task manager built with C# and WPF. It uses SQL Server LocalDB to store users and tasks, with separate screens for login, signup, task management, admin user management, and profile updates.

## Features

- User login and account creation.
- Admin user list with user deletion.
- Personal task list per signed-in user.
- Add tasks with title, description, due date, priority, and category.
- Search tasks by title or description.
- Filter tasks by due date range, priority, and category.
- Mark tasks as completed.
- View active and completed tasks separately.
- Update username and password from the profile screen.
- Due-date reminder logic for tasks due within 24 hours.

## Tech Stack

- C#
- WPF
- .NET 8
- SQL Server LocalDB
- `System.Data.SqlClient`

## Project Structure

```text
App.xaml / App.xaml.cs               Application startup
Window1.xaml / Window1.xaml.cs       Login and signup screen
Window2.xaml / Window2.xaml.cs       Admin user management screen
Window3.xaml / Window3.xaml.cs       User menu screen
Window4.xaml / Window4.xaml.cs       Profile management screen
MainWindow.xaml / MainWindow.xaml.cs Task list, filters, and completed tasks
TaskDialog.xaml / TaskDialog.xaml.cs Add-task dialog
Users.sql                            Users table script
Tasks.sql                            Tasks table script
Task.sql                             Alternate/older task table script
```

## Requirements

- Windows
- Visual Studio 2022 or later
- .NET 8 SDK
- SQL Server Express LocalDB

## Database Setup

The app connects to this LocalDB database:

```text
Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Login;Integrated Security=True;
```

Create a LocalDB database named `Login`, then run the table scripts:

1. Run `Users.sql`.
2. Run `Tasks.sql`.
3. Make sure the `Tasks` table has a `Completed` column because the app filters active and completed tasks.

If your `Tasks` table does not have that column yet, add it:

```sql
ALTER TABLE Tasks
ADD Completed BIT NOT NULL DEFAULT 0;
```

Create an admin user first if you want access to the admin screen. The current code treats `UserID = 1` as the admin account.

Example:

```sql
INSERT INTO Users (Username, Password)
VALUES ('admin', 'admin');
```

Change the default admin password after testing.

## Run The App

Open the solution in Visual Studio:

```text
TaskManagement.sln
```

Then restore packages, build, and run the project.

You can also use the command line from the project folder:

```powershell
dotnet restore
dotnet build
dotnet run --project TaskManagement.csproj
```

## Usage

1. Open the app.
2. Log in or create a new account.
3. Admin users can view and delete registered users.
4. Regular users can open the task manager or profile management screen.
5. In the task manager, add tasks and filter them by search text, date range, priority, or category.
6. Use the `Complete` button to move a task to the completed list.

## Security Notes

This project is currently suitable for learning or local demonstration use. Before using it for real accounts, update the authentication flow so passwords are hashed and never displayed in the admin user list.

The repository currently contains build output and local database files. For a cleaner public GitHub repository, ignore or remove generated/local files such as:

- `.vs/`
- `bin/`
- `obj/`
- `*.user`
- `*.mdf`
- `*.ldf`
- `bin.zip`

## Status

The project has the core task management flow in place. Good next improvements would be editing existing tasks, hashing passwords, improving database setup scripts, and adding automated tests.
