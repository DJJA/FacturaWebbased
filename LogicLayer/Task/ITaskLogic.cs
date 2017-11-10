using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Models;

namespace LogicLayer
{
    public interface ITaskLogic
    {
        List<Task> GetAllTasks();
        void Insert(Task task);
        List<Task> GetTasksByDescription(string description);
        Task GetTaskById(int id);

    }
}
