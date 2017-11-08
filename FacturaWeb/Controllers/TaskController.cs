using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Factory;
using FacturaWeb.ViewModels;
using LogicLayer;

namespace FacturaWeb.Controllers
{
    public class TaskController : Controller
    {
        private ITaskLogic taskLogic = TaskFactory.ManageTasks();
        public ActionResult Index()
        {
            TaskViewModel viewModel = new TaskViewModel()
            {
                Tasks = taskLogic.GetAllTasks()
            };

            return View(viewModel);
        }

        public ActionResult CreateNewTask()
        {
            return View();
        }
    }
}