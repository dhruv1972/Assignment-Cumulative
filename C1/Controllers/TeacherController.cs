using System.Collections.Generic;
using System.Diagnostics;
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

        public ActionResult New()
        {
            return View();
        }

        public ActionResult Ajax_New()
        {
            return View();
        }

        
        
    }
}