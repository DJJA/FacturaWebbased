using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Task = Models.Task;

namespace FacturaWeb.ViewModels
{
    public class TaskViewModel
    {
        public Task Task { get; set; }
        public string Description { get; set; }
        public List<Task>  Tasks { get; set; }

    }
}