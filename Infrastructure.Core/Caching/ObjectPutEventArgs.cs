﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Core.Caching
{
    [Serializable]
    public class ObjectPutEventArgs : EventArgs
    {
        public string Key { get; set; }
        public object Version { get; set; }
    }
}
