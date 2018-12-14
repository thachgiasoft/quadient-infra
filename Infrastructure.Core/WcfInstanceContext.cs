using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Core
{
    ///<summary>
    /// This class incapsulates context information for a service instance
    ///</summary>
    public class WcfInstanceContext : IExtension<InstanceContext>
    {
        private readonly IDictionary _items;

        private WcfInstanceContext()
        {
            _items = new Hashtable();
        }

        ///<summary>
        /// <see cref="IDictionary"/> stored in current instance context.
        ///</summary>
        public IDictionary Items
        {
            get { return _items; }
        }

        ///<summary>
        /// Gets the current instance of <see cref="WcfInstanceContext"/>
        ///</summary>
        public static WcfInstanceContext Current
        {
            get
            {
                if (OperationContext.Current != null)
                {
                    var context = OperationContext.Current.InstanceContext.Extensions.Find<WcfInstanceContext>();
                    if (context == null)
                    {
                        context = new WcfInstanceContext();
                        OperationContext.Current.InstanceContext.Extensions.Add(context);
                    }
                    return context;
                }
                return null;
            }
        }

        /// <summary>
        /// <see cref="IExtension{T}"/> Attach() method
        /// </summary>
        public void Attach(InstanceContext owner) { }

        /// <summary>
        /// <see cref="IExtension{T}"/> Detach() method
        /// </summary>
        public void Detach(InstanceContext owner) { }
    }
}
