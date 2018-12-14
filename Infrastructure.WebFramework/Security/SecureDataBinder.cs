using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace Infrastructure.WebFramework.Security
{
    public static class SecureDataBinder
    {

        public static object Eval(object container, string expression)
        {
            var value = DataBinder.Eval(container, expression);
            return value != null && value.GetType() == typeof(string) ? System.Web.HttpUtility.HtmlEncode(value.ToString()) : value;
        }

        public static string Eval(object container, string expression, string format)
        {
            return System.Web.HttpUtility.HtmlEncode(DataBinder.Eval(container, expression, format));
        }

        public static object GetPropertyValue(object container, string propName)
        {
            var value = DataBinder.GetPropertyValue(container, propName);
            return value != null && value.GetType() == typeof(string) ? System.Web.HttpUtility.HtmlEncode(value.ToString()) : value;
        }

        public static string GetPropertyValue(object container, string propName, string format)
        {
            return System.Web.HttpUtility.HtmlEncode(DataBinder.GetPropertyValue(container, propName, format));
        }
        //--For Html
        public static object HtmlEval(object container, string expression)
        {
            return HtmlSanitizer.SanitizeHtml(DataBinder.Eval(container, expression).ToString());
        }

        public static string HtmlEval(object container, string expression, string format)
        {
            return HtmlSanitizer.SanitizeHtml(DataBinder.Eval(container, expression, format));
        }

    }
}
