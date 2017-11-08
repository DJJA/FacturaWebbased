using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Factory;
using FacturaWeb.ViewModels;
using LogicLayer;
using Models;

namespace FacturaWeb.Controllers
{
    public class TaskController : Controller
    {
        private ITaskLogic taskLogic = TaskFactory.ManageTasks();

        public ActionResult Index(string search = "")
        {
            TaskViewModel viewModel = new TaskViewModel()
            {
                Tasks = taskLogic.GetTasksByDescription(search)
            };

            

            return View(viewModel);
        }

        public ActionResult SaveTaskToDb(FormCollection collection)
        {
            string description = collection["tbDescription"];

            var task = new Task(id: 0, description: description);

            taskLogic.Insert(task);

            return RedirectToAction("Index");
        }

        public ActionResult CreateNewTask()
        {
            return View();
        }
    }
}