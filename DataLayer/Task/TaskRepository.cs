using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using Models;

namespace DataLayer
{
    public class TaskRepository : Repository<Task>, ITaskRepository
    {
        private ITaskContext taskContext;
        public TaskRepository(ITaskContext context)
            : base(context)
        {
            taskContext = context;
        }

        public IEnumerable<Task> GetTasksByDescription(string description)
        {
            return taskContext.GetTaskByDescription(description);
        }

        public Task GetTaskById(int id)
        {
            return taskContext.GetTaskById(id);
        }
    }
}
