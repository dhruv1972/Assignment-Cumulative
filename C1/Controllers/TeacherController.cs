using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Web.Mvc;
using Cumulative1.Models;


namespace Cumulative1.Controllers
{
    public class TeacherController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List(string SearchKey = null)
        {
            TeacherDataController controller = new TeacherDataController();
            IEnumerable<Teacher> Teachers = controller.ListTeachers(SearchKey);
            return View(Teachers);
        }

        public ActionResult Show(int id)
        {
            Debug.WriteLine($"Show action called with ID: {id}");
            TeacherDataController controller = new TeacherDataController();
            Teacher NewTeacher = controller.FindTeacher(id);

            if (NewTeacher == null || NewTeacher.TeacherId == 0)
            {
                Debug.WriteLine($"Teacher not found for ID: {id}");
                TempData["ErrorMessage"] = "Teacher not found.";
                return RedirectToAction("List");
            }

            Debug.WriteLine($"Teacher found: ID = {NewTeacher.TeacherId}, Name = {NewTeacher.TeacherFname} {NewTeacher.TeacherLname}");
            return View(NewTeacher);
        }
        /// <summary>
        /// Displays a form to add a new teacher.
        /// </summary>
        /// <returns>The view displaying a form to add a new teacher.</returns>
        /// <example>
        /// GET : /Teacher/New
        /// </example>
        public ActionResult New()
        {
            return View();
        }

        /// <summary>
        /// Displays a form to add a new teacher.
        /// </summary>
        /// <returns>The view displaying a form to add a new teacher using AJAX request.</returns>
        /// <example>
        /// GET : /Teacher/Ajax_New
        /// </example>
        public ActionResult Ajax_New()
        {
            return View();
        }

        /// <summary>
        /// Creates a new teacher with the provided information.
        /// </summary>
        /// <param name="TeacherFname">The first name of the teacher.</param>
        /// <param name="TeacherLname">The last name of the teacher.</param>
        /// <param name="EmployeeNumber">The employee number of the teacher.</param>
        /// <param name="HireDate">The hire date of the teacher.</param>
        /// <param name="Salary">The salary of the teacher.</param>
        /// <returns>
        /// A response indicating the success or failure of the operation.
        /// Returns a 200 OK response if the teacher is added successfully.
        /// Returns a 400 Bad Request response if the provided information is missing or incorrect.
        /// </returns>
        /// <example>
        /// Example of POST request body
        /// POST /Teacher/Create
        /// {
        ///     "TeacherFname": "Dhruv",
        ///     "TeacherLname": "Chavda",
        ///     "EmployeeNumber": "T1234",
        ///     "HireDate": "2025-04-01",
        ///     "Salary": 65
        /// }
        /// </example>
        [HttpPost]
        public ActionResult Create(string TeacherFname, string TeacherLname, string EmployeeNumber, DateTime HireDate, decimal? Salary)
        {
            // Check for missing information
            if (string.IsNullOrEmpty(TeacherFname) || string.IsNullOrEmpty(TeacherLname) ||
                string.IsNullOrEmpty(EmployeeNumber) || HireDate == null || HireDate > DateTime.Now || Salary == null || Salary < 0)
            {
                // Return the view with an error message
                ViewBag.Message = "Missing or incorrect information when adding a teacher";
                return View("New");
            }
            Teacher NewTeacher = new Teacher();
            NewTeacher.TeacherFname = TeacherFname;
            NewTeacher.TeacherLname = TeacherLname;
            NewTeacher.EmployeeNumber = EmployeeNumber;
            NewTeacher.HireDate = HireDate;
            NewTeacher.Salary = Salary ?? 0;

            TeacherDataController controller = new TeacherDataController();
            controller.AddTeacher(NewTeacher);

            // Return the view to list page
            return RedirectToAction("List");
        }

        /// <summary>
        /// Displays a confirmation page to delete a specific teacher.
        /// </summary>
        /// <param name="id">The ID of the teacher to delete.</param>
        /// <returns>The view displaying a confirmation page to delete the teacher.</returns>
        /// <example>
        /// GET : /Teacher/DeleteConfirm/{id}
        /// </example>
        public ActionResult DeleteConfirm(int id)
        {
            TeacherDataController controller = new TeacherDataController();
            Teacher NewTeacher = controller.FindTeacher(id);
            return View(NewTeacher);
        }

        /// <summary>
        /// Displays a confirmation page to delete a specific teacher using AJAX.
        /// </summary>
        /// <param name="id">The ID of the teacher to delete.</param>
        /// <returns>The view displaying a confirmation page to delete the teacher using AJAX request.</returns>
        /// <example>
        /// GET : /Teacher/Ajax_DeleteConfirm/{id}
        /// </example>
        public ActionResult Ajax_DeleteConfirm(int id)
        {
            TeacherDataController controller = new TeacherDataController();
            Teacher NewTeacher = controller.FindTeacher(id);
            return View(NewTeacher);
        }

        /// <summary>
        /// Deletes a specific teacher from the system.
        /// </summary>
        /// <param name="id">The ID of the teacher to delete.</param>
        /// <returns>A response indicating the success or failure of the operation.</returns>
       
        [HttpPost]
        public ActionResult Delete(int id)
        {
            TeacherDataController controller = new TeacherDataController();
            controller.DeleteTeacher(id);

            // Return a 200 OK response
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
        /// <summary>
        /// Routes to a dynamically generated "Teacher Update" Page. Gathers information from the database.
        /// </summary>
        /// <param name="id">Id of the Teacher</param>
        /// <returns>A dynamic "Update Teacher" webpage which provides the current information of the teacher and asks the user for new information as part of a form.</returns>
        /// <example>
        /// Example of GET request:
        /// GET /Teacher/Update/123
        /// </example>
        public ActionResult Update(int id)
        {
            TeacherDataController controller = new TeacherDataController();
            Teacher SelectedTeacher = controller.FindTeacher(id);

            return View(SelectedTeacher);
        }

        /// <summary>
        /// Routes to a dynamically generated "Teacher Update" Page. 
        /// </summary>
        /// <param name="id">Id of the Teacher</param>
        /// <returns>A dynamic "Update Teacher" webpage </returns>
        /// <example>
        /// Example of GET request:
        /// GET /Teacher/Ajax_Update/123
        /// </example>
        public ActionResult Ajax_Update(int id)
        {
            TeacherDataController controller = new TeacherDataController();
            Teacher SelectedTeacher = controller.FindTeacher(id);

            return View(SelectedTeacher);
        }

        /// <summary>
        /// Updates the information of a specific teacher in the system.
        /// </summary>
        /// <param name="id">The ID of the teacher to update.</param>
        /// <param name="TeacherFname">The updated first name of the teacher.</param>
        /// <param name="TeacherLname">The updated last name of the teacher.</param>
        /// <param name="EmployeeNumber">The updated employee number of the teacher.</param>
        /// <param name="HireDate">The updated hire date of the teacher.</param>
        /// <param name="Salary">The updated salary of the teacher.</param>
        /// <returns>
        /// Returns the "Update Teacher" view with an error message if the provided information is missing or incorrect.
        /// </returns>
        /// <example>
        /// Example of POST request body:
        /// POST /Teacher/Update/{id}
        /// {
        ///     "TeacherFname": "UpdatedFirstName",
        ///     "TeacherLname": "UpdatedLastName",
        ///     "EmployeeNumber": "UpdatedEmployeeNumber",
        ///     "HireDate": "2024-04-20",
        ///     "Salary": 70
        /// }
        /// </example>
        [HttpPost]
        public ActionResult Update(int id, string TeacherFname, string TeacherLname, string EmployeeNumber, DateTime HireDate, decimal? Salary)
        {
            TeacherDataController controller = new TeacherDataController();
            if (string.IsNullOrEmpty(TeacherFname) || string.IsNullOrEmpty(TeacherLname) ||
    string.IsNullOrEmpty(EmployeeNumber) || HireDate == null || HireDate > DateTime.Now || Salary == null || Salary < 0)
            {
                // Return the view with an error message
                ViewBag.Message = "Missing or incorrect information when updating a teacher";
                Teacher SelectedTeacher = controller.FindTeacher(id);
                return View("Update", SelectedTeacher);
            }
            Teacher TeacherInfo = new Teacher();
            TeacherInfo.TeacherFname = TeacherFname;
            TeacherInfo.TeacherLname = TeacherLname;
            TeacherInfo.EmployeeNumber = EmployeeNumber;
            TeacherInfo.HireDate = HireDate;
            TeacherInfo.Salary = Salary ?? 0;

            controller.UpdateTeacher(id, TeacherInfo);

            return RedirectToAction("Show/" + id);
        }
    }
}