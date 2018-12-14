using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Infrastructure.WebFramework.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class DefineAttribute : FilterAttribute
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        private string _description;

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        private string[] metaData;

        public string[] MetaData
        {
            get { return metaData; }
            set { metaData = value; }
        }

        public DefineAttribute()
        {

        }
        public DefineAttribute(string name, string description)
        {
            _name = name;
            _description = description;
        }
    }
}
