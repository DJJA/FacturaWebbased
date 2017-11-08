using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using LogicLayer;
using Models;

namespace Factory
{
    public static class TaskFactory
    {
        public static ITaskLogic ManageTasks()
        {
            return new TaskLogic(new TaskRepository(new SqlTaskContext()));
        }
    }
}
