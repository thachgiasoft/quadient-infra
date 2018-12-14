using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.JobScheduling
{
    public class JobDto
    {
        public string JobName { get; set; }
        public string GroupName { get; set; }
        public string ServiceEndPointKey { get; set; }
        //public Func<T> FuncData { get; set; }
    }
}
