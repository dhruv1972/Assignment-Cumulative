using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using Cumulative1.Models;
using System.Diagnostics;
using System.Web.Http;
using System.Web.Http.Cors;



namespace Cumulative1.Controllers
{
    public class TeacherDataController : ApiController
    {
        private SchoolDbContext School = new SchoolDbContext();

        /// <summary>
        /// Returns a list of teachers in the system filtered by an optional search key.
        /// </summary>
        /// <param name="SearchKey">Optional search key to filter teachers by first name, last name, full name, hire date, or salary.</param>
        /// <returns>
        /// A list of teacher objects.

        [HttpGet]
        [Route("api/TeacherData/ListTeachers/{SearchKey?}")]
        public IEnumerable<Teacher> ListTeachers(string SearchKey = null)
        {
            MySqlConnection Conn = School.AccessDatabase();
            Conn.Open();
            MySqlCommand cmd = Conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Teachers WHERE LOWER(teacherfname) LIKE LOWER(@Key) OR LOWER(teacherlname) LIKE LOWER(@Key) OR LOWER(CONCAT(teacherfname, ' ', teacherlname)) LIKE LOWER(@Key) or hiredate Like @Key or DATE_FORMAT(hiredate, '%d-%m-%Y') Like @Key or salary LIKE @Key ";
            cmd.Parameters.AddWithValue("@Key", "%" + SearchKey + "%");
            cmd.Prepare();
            MySqlDataReader ResultSet = cmd.ExecuteReader();
            List<Teacher> Teachers = new List<Teacher>();
            while (ResultSet.Read())
            {
                Teachers.Add(new Teacher()
                {
                    TeacherId = Convert.ToInt32(ResultSet["teacherId"]),
                    TeacherFname = ResultSet["teacherFname"].ToString(),
                    TeacherLname = ResultSet["teacherLname"].ToString(),
                    EmployeeNumber = ResultSet["employeenumber"].ToString(),
                    HireDate = Convert.ToDateTime(ResultSet["hiredate"]),
                    Salary = Convert.ToDecimal(ResultSet["salary"])
                });
            }
            Conn.Close();
            return Teachers;
        }

        /// <summary>
        /// Finds a teacher in the system given an ID.
        /// </summary>
        /// <param name="id">The teacher's primary key.</param>
        /// <returns>
        /// A teacher object, or null if the teacher is not found.
        /// </returns>
        [HttpGet]
        [Route("api/TeacherData/FindTeacher/{id}")]
        public Teacher FindTeacher(int id)
        {
            Teacher NewTeacher = null;
            MySqlConnection Conn = School.AccessDatabase();
            try
            {
                Conn.Open();
                MySqlCommand cmd = Conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM Teachers WHERE teacherid = @id";
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Prepare();
                MySqlDataReader ResultSet = cmd.ExecuteReader();

                if (ResultSet.Read())
                {
                    NewTeacher = new Teacher
                    {
                        TeacherId = Convert.ToInt32(ResultSet["teacherId"]),
                        TeacherFname = ResultSet["teacherFname"].ToString(),
                        TeacherLname = ResultSet["teacherLname"].ToString(),
                        EmployeeNumber = ResultSet["employeenumber"].ToString(),
                        HireDate = Convert.ToDateTime(ResultSet["hiredate"]),
                        Salary = Convert.ToDecimal(ResultSet["salary"])
                    };
                    Debug.WriteLine($"Teacher data read: ID = {NewTeacher.TeacherId}, Name = {NewTeacher.TeacherFname} {NewTeacher.TeacherLname}");
                }
                ResultSet.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in FindTeacher (Teacher Data): " + ex.Message);
                return null;
            }
            finally
            {
                Conn.Close();
            }
            return NewTeacher;

        }
        /// <summary>
        /// Adds a teacher to the MySQL Database.
        /// </summary>
        /// <param name="NewTeacher">An object with fields that map to the columns of the teacher's table.</param>
        /// <returns>
        /// A response indicating the success or failure of the operation.
        /// Returns a 400 Bad Request response if the provided information is missing or incorrect.
        /// Returns a 200 OK response if the teacher is added successfully.
        /// </returns>
        /// <example>
        /// POST api/TeacherData/AddTeacher 
        /// FORM DATA / POST DATA / REQUEST BODY 
        /// {
        ///	"TeacherFname":"Dhruv",
        ///	"TeacherLname":"Chavda",
        ///	"EmployeeNumber":"T1234",
        ///	"HireDate":"04-01-2025"
        ///	"Salary": 65
        /// }
        /// </example>
        [HttpPost]
        [EnableCors(origins: "*", methods: "*", headers: "*")]
        [Route("api/TeacherData/AddTeacher")]
        public IHttpActionResult AddTeacher([FromBody] Teacher NewTeacher)
        {

            if (string.IsNullOrEmpty(NewTeacher.TeacherFname) || string.IsNullOrEmpty(NewTeacher.TeacherLname) ||
                string.IsNullOrEmpty(NewTeacher.EmployeeNumber) || NewTeacher.HireDate == null || NewTeacher.HireDate > DateTime.Now || NewTeacher.Salary < 0)
            {
                // Return a 400 Bad Request response with an error message
                return BadRequest("Invalid data provided for adding the teacher.");
            }

            //Create an instance of a connection
            MySqlConnection Conn = School.AccessDatabase();

            //Open the connection between the web server and database
            Conn.Open();

            //Establish a new command (query) for our database
            MySqlCommand cmd = Conn.CreateCommand();

            //SQL QUERY
            cmd.CommandText = "insert into teachers (teacherfname, teacherlname, employeenumber, hiredate, salary) values (@TeacherFname,@TeacherLname,@Employeenumber, @HireDate, @Salary)";
            cmd.Parameters.AddWithValue("@TeacherFname", NewTeacher.TeacherFname);
            cmd.Parameters.AddWithValue("@TeacherLname", NewTeacher.TeacherLname);
            cmd.Parameters.AddWithValue("@EmployeeNumber", NewTeacher.EmployeeNumber);
            cmd.Parameters.AddWithValue("@HireDate", NewTeacher.HireDate);
            cmd.Parameters.AddWithValue("@Salary", NewTeacher.Salary);

            cmd.Prepare();

            cmd.ExecuteNonQuery();

            Conn.Close();
            return Ok("Teacher added successfully");
        }

        /// <summary>
        /// Deletes a teacher from the connected MySQL Database if the ID of that teacher exists.
        /// </summary>
        /// <param name="id">The ID of the teacher.</param>
        /// <returns>
        /// A response indicating the success of the operation..
        /// Returns a 200 OK response if the teacher is updated successfully.
        /// </returns>
        /// <example>POST /api/TeacherData/DeleteTeacher/3</example>
        [HttpDelete]
        [EnableCors(origins: "*", methods: "*", headers: "*")]
        [Route("api/TeacherData/DeleteTeacher/{id}")]
        public IHttpActionResult DeleteTeacher(int id)
        {
            //Create an instance of a connection
            MySqlConnection Conn = School.AccessDatabase();

            //Open the connection between the web server and database
            Conn.Open();

            //Establish a new command (query) for our database
            MySqlCommand cmd = Conn.CreateCommand();

            // SQL QUERY
            // Delete from teachers table where teacherid = @id
            cmd.CommandText = "DELETE FROM teachers WHERE teacherid = @id";
            cmd.Parameters.AddWithValue("@id", id);

            try
            {
                int rowsAffected = cmd.ExecuteNonQuery();
                Conn.Close();

                if (rowsAffected > 0)
                {
                    return Ok("Teacher Deleted successfully");
                }
                else
                {
                    return NotFound(); // Or another appropriate response if the teacher wasn't found
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("Error deleting teacher: " + ex.Message);
                return InternalServerError(); // Or handle the error in a more specific way
            }
            finally
            {
                if (Conn.State == System.Data.ConnectionState.Open)
                {
                    Conn.Close();
                }
            }
        }
        /// <summary>
        /// Updates the information of a specific teacher in the MySQL Database.
        /// </summary>
        /// <param name="id">The ID of the teacher to update.</param>
        /// <param name="TeacherInfo">An object containing the updated information of the teacher.</param>
        /// <returns>
        /// Returns a 400 Bad Request response if the provided information is missing or incorrect.
        /// Returns a 200 OK response if the teacher is updated successfully.
        /// </returns>
        /// <example>
        /// Example of POST request body:
        /// POST /api/TeacherData/UpdateTeacher/{id}
        /// {
        ///     "TeacherFname": "UpdatedFirstName",
        ///     "TeacherLname": "UpdatedLastName",
        ///     "EmployeeNumber": "UpdatedEmployeeNumber",
        ///     "HireDate": "2024-04-20",
        ///     "Salary": 70
        /// }
        /// </example>
        [HttpPost]
        [EnableCors(origins: "*", methods: "*", headers: "*")]
        [Route("api/TeacherData/UpdateTeacher/{id}")]
        public IHttpActionResult UpdateTeacher(int id, [FromBody] Teacher TeacherInfo)
        {
            if (string.IsNullOrEmpty(TeacherInfo.TeacherFname) || string.IsNullOrEmpty(TeacherInfo.TeacherLname) ||
                string.IsNullOrEmpty(TeacherInfo.EmployeeNumber) || TeacherInfo.HireDate == null || TeacherInfo.HireDate > DateTime.Now || TeacherInfo.Salary < 0)
            {
                // Return a 400 Bad Request response with an error message
                return BadRequest("Invalid data provided for updating the teacher.");
            }

            //Create an instance of a connection
            MySqlConnection Conn = School.AccessDatabase();

            //Debug.WriteLine("id: " + id);
            //Debug.WriteLine("TeacherFname: " + TeacherInfo.TeacherFname);
            //Debug.WriteLine("TeacherLname: " + TeacherInfo.TeacherLname);
            //Debug.WriteLine("EmployeeNumber: " + TeacherInfo.EmployeeNumber);
            //Debug.WriteLine("HireDate: " + TeacherInfo.HireDate);
            //Debug.WriteLine("Salary: " + TeacherInfo.Salary);


            //Open the connection between the web server and database
            Conn.Open();

            //Establish a new command (query) for our database
            MySqlCommand cmd = Conn.CreateCommand();

            //SQL QUERY
            cmd.CommandText = "UPDATE teachers SET teacherfname=@TeacherFname, teacherlname=@TeacherLname, employeenumber=@EmployeeNumber, hiredate=@HireDate, salary=@Salary  where teacherid=@TeacherId";
            cmd.Parameters.AddWithValue("@TeacherFname", TeacherInfo.TeacherFname);
            cmd.Parameters.AddWithValue("@TeacherLname", TeacherInfo.TeacherLname);
            cmd.Parameters.AddWithValue("@EmployeeNumber", TeacherInfo.EmployeeNumber);
            cmd.Parameters.AddWithValue("@HireDate", TeacherInfo.HireDate);
            cmd.Parameters.AddWithValue("@Salary", TeacherInfo.Salary);
            cmd.Parameters.AddWithValue("@TeacherId", id);
            cmd.Prepare();

            cmd.ExecuteNonQuery();

            Conn.Close();

            return Ok("Teacher updated successfully");
        }

    }
}
    
    