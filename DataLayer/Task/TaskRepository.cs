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

    }
}
