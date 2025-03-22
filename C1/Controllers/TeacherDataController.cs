using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using Cumulative1.Models;
using System.Diagnostics;
using System.Web.Http;


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
    }
}