using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Infrastructure.WebFramework.Security
{
    public static class ResponseExtensions
    {
        /// <summary>
        /// Sadece domain name altinda bulunan url lere yönlendirmeye izin verir.
        /// Open redirect zaafiyetini gidermek icindir.
        /// </summary>
        /// <param name="response">current response</param>
        /// <param name="url">url to redirect</param>
        public static void SecureRedirect(this HttpResponse response, string url)
        {
            CheckAndRedirect(response, url);
        }
        /// <summary>
        /// Sadece domain name altinda bulunan url lere yonlendirmeye izin verir.
        /// Open redirect zaafiyetini gidermek icindir.
        /// </summary>
        /// <param name="response">current response</param>
        /// <param name="url">url to redirect</param>
        /// <param name="endResponse">endresponse. default is true</param>
        public static void SecureRedirect(this HttpResponse response, string url, bool endResponse)
        {
            CheckAndRedirect(response, url, endResponse);
        }
        /// <summary>
        /// open redirect zaafiyetine karsi Url control et
        /// </summary>
        /// <param name="response"></param>
        /// <param name="url"></param>
        /// <param name="endresponse"></param>
        private static void CheckAndRedirect(HttpResponse response, string url, bool endresponse = true)
        {
            if (string.IsNullOrEmpty(url))
            {
                return;
            }

            Uri absoluteUri;
            if (Uri.TryCreate(url, UriKind.Absolute, out absoluteUri))
            {
                if (String.Equals(HttpContext.Current.Request.Url.Host, absoluteUri.Host,
                    StringComparison.OrdinalIgnoreCase)) // if local
                    response.Redirect(url, endresponse);
            }
            else
            {
                url = url.Replace("|", "pipe");
                bool isLocal = !url.StartsWith("http:", StringComparison.OrdinalIgnoreCase)
                    && !url.StartsWith("https:", StringComparison.OrdinalIgnoreCase)
                    && Uri.IsWellFormedUriString(url, UriKind.Relative); //if local
                url = url.Replace("pipe", "|");
                if (isLocal)
                    response.Redirect(System.Web.HttpUtility.UrlPathEncode(url), endresponse);
            }
        }
    }
}
