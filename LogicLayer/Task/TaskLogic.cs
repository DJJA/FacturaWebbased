using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using DataLayer;
using Task = Models.Task;

namespace LogicLayer
{
    public class TaskLogic : ITaskLogic
    {
        private ITaskRepository taskRepository;

        public TaskLogic(ITaskRepository taskRepository)
        {
            this.taskRepository = taskRepository;
        }

        public List<Task> GetAllTasks()
        {
            return taskRepository.GetAll().ToList();
        }

        public void Insert(Task task)
        {
            taskRepository.Add(task);
        }

        public List<Task> GetTasksByDescription(string description)
        {
            return taskRepository.GetTasksByDescription(description).ToList();
        }

        public Task GetTaskById(int id)
        {
            return taskRepository.GetById(id);
        }
    }
}
