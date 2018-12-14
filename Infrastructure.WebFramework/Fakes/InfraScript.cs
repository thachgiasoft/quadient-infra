using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.WebFramework.Fakes
{
    public class InfraScript
    {
        public string Key { get; set; }
        public string PathOrResourceNameOrFullScript { get; set; }
        public bool IsRendered { get; set; }
    }
}
