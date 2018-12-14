using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Infrastructure.Core
{
    /// <summary>
    /// Uygulama Web ise HttpContext.Current, WCF ise OperationContext barındıran WcfInstanceContext.Current , Winform uygulamsı ise static bir
    /// Dictionary saglar. Objelerin yasam dongusunun dogru saglanması amacı ile yazilmistir.
    /// </summary>
    public class HybridContext : IDisposable
    {
        private const string CurrentContextKey = "CurrentContextKey";
        [ThreadStatic]
        private static volatile IDictionary _items = null;
        private HybridContext()
        {
        }
        public string ContextType
        {
            get
            {
                return HttpContext.Current != null
                           ? "HttpContext.Current"
                           : (WcfInstanceContext.Current != null ? "WcfInstanceContext.Current" : "Hashtable");
            }
        }
        public static HybridContext Current
        {
            get
            {

                object currentContext = null;
                if (HttpContext.Current != null)
                    currentContext = HttpContext.Current.Items[CurrentContextKey];
                else if (WcfInstanceContext.Current != null)
                    currentContext = WcfInstanceContext.Current.Items[CurrentContextKey];
                else
                {
                    if (_items == null)//her thread icin 1 table olusturulur
                        _items = new Hashtable();
                    currentContext = _items[CurrentContextKey];
                }
                if (currentContext != null)
                    return (HybridContext)currentContext;
                if (HttpContext.Current != null && currentContext == null)
                {
                    HttpContext.Current.Items[CurrentContextKey] = currentContext = new HybridContext();
                }
                else if (WcfInstanceContext.Current != null && currentContext == null)
                {
                    WcfInstanceContext.Current.Items[CurrentContextKey] = currentContext = new HybridContext();
                }
                else if (currentContext == null)
                {
                    _items[CurrentContextKey] = currentContext = new HybridContext();
                }
                return (HybridContext)currentContext;
            }
        }
        public object this[object key]
        {
            get
            {
                var obj = HttpContext.Current != null
                           ? HttpContext.Current.Items[key]
                           : (WcfInstanceContext.Current != null ? WcfInstanceContext.Current.Items[key] : _items[key]);
                return obj;
            }
            set
            {
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.Items[key] = value;
                }
                else if (WcfInstanceContext.Current != null)
                {
                    WcfInstanceContext.Current.Items[key] = value;
                }
                else
                {
                    _items[key] = value;
                }
            }
        }

        public void Dispose()
        {
            if (HttpContext.Current != null)
            {
                foreach (System.Collections.DictionaryEntry obj in HttpContext.Current.Items)
                {
                    if (obj.Value is IDisposable && !(obj.Value is HybridContext)) //kendi kendini cagirmasn
                        ((IDisposable)obj.Value).Dispose();
                }
                HttpContext.Current.Items.Clear();
            }
            else if (WcfInstanceContext.Current != null)
            {
                foreach (System.Collections.DictionaryEntry obj in WcfInstanceContext.Current.Items)
                {
                    if (obj.Value is IDisposable && !(obj.Value is HybridContext)) //kendi kendini cagirmasn
                        ((IDisposable)obj.Value).Dispose();
                }
                WcfInstanceContext.Current.Items.Clear();
            }
            else if (_items != null)
            {
                foreach (System.Collections.DictionaryEntry obj in _items)
                {
                    if (obj.Value is IDisposable && !(obj.Value is HybridContext)) //kendi kendini cagirmasn
                        ((IDisposable)obj.Value).Dispose();
                }
                _items.Clear();
            }
        }
    }
}
