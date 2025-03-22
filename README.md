# School Management Application

This project is a simple web application for managing teacher information, built using ASP.NET MVC and a MySQL database.

## Features

* **List Teachers:** Displays a list of all teachers with their ID and full name. Includes an optional search functionality to filter teachers by first name, last name, full name, hire date, or salary.
* **Show Teacher Details:** Displays detailed information for a specific teacher, including their ID, employee number, full name, hire date, and salary.


## Technologies Used

* **ASP.NET MVC:** The web framework used for building the application.
* **C#:** The programming language used for the backend logic.
* **MySQL:** The relational database used to store teacher and class information.
* **MySql.Data:** The MySQL connector for .NET used to interact with the database.

## Setup Instructions

1.  **Prerequisites:**
    * **Visual Studio:** Recommended for building and running the ASP.NET MVC project.
    * **.NET Framework (or .NET Core/5+ SDK):** Ensure you have the appropriate SDK installed based on your project configuration.
    * **MySQL Server:** You need a running MySQL server instance.
    * **MySQL Connector/NET:** This is usually installed via NuGet in your Visual Studio project.

2.  **Clone the Repository:**
    ```bash
    git clone <repository_url>
    cd <repository_directory>
    ```
    (Replace `<repository_url>` and `<repository_directory>` with your project's information)

3.  **Database Setup:**
    * **Create a Database:** Create a new database named `school` (or your preferred name, but update the connection string accordingly) in your MySQL server.
    * **Create the `teachers` Table:** Execute the following SQL script to create the `teachers` table:
        ```sql
        CREATE TABLE teachers (
            teacherId INT PRIMARY KEY AUTO_INCREMENT,
            teacherFname VARCHAR(255) NOT NULL,
            teacherLname VARCHAR(255) NOT NULL,
            employeenumber VARCHAR(20),
            hiredate DATE,
            salary DECIMAL(10, 2)
        );
        ```
   
    * **Populate with Data (Optional):** Insert some sample teacher data into the `teachers` table for testing.

4.  **Configure the Connection String:**
    * Locate the `Web.config` file (or `appsettings.json` for .NET Core/5+) in your project.
    * Find the connection string section (usually under `<connectionStrings>` in `Web.config`).
    * Update the connection string to match your MySQL server details (server name, port, database name, username, password).

5.  **Build and Run the Application:**
    * Open the project in Visual Studio.
    * Build the solution (`Build > Build Solution`).
    * Run the application (`Debug > Start Debugging` or `Ctrl + F5`).

6.  **Access the Application:**
    * The application will typically open in your default web browser. Navigate to the teacher listing page (usually `/Teacher/List`).

## API Endpoints

The application exposes the following API endpoints (primarily used internally by the MVC controllers):

* **`GET /api/TeacherData/ListTeachers`**: Returns a JSON list of all teachers.
* **`GET /api/TeacherData/ListTeachers/{SearchKey?}`**: Returns a JSON list of teachers filtered by the optional `SearchKey`.
* **`GET /api/TeacherData/FindTeacher/{id}`**: Returns a JSON object containing the details of the teacher with the specified `id`.

You can test these API endpoints using tools like `curl` or Postman. Example `curl` commands:

```bash
# List all teachers
curl http://localhost:<your_port>/api/TeacherData/ListTeachers

# Find teacher with ID 1
curl http://localhost:<your_port>/api/TeacherData/FindTeacher/1
