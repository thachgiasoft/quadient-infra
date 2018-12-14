using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Infrastructure.Core.TypeFinders;

namespace Infrastructure.WebFramework.ViewEngine.Razor
{
    public class SeperatedRazorViewEngine : RazorViewEngine
    {
        public SeperatedRazorViewEngine(IEnumerable<string> appNames)
        {

            var areaViewAndPartialViewLocationFormats = new List<string>();

            var seperateViewLocationFormats = new List<string>();

            var masterLocationFormats = new List<string>();

            foreach (var app in appNames)
            {
                masterLocationFormats.Add(
                    "~/Areas/" + app + "/Views/{1}/{0}.cshtml");
                masterLocationFormats.Add(
                    "~/Areas/" + app + "/Views/{1}/{0}.vbhtml");
                masterLocationFormats.Add(
                    "~/Areas/" + app + "/Views/Shared/{1}/{0}.cshtml");
                masterLocationFormats.Add(
                    "~/Areas/" + app + "/Views/Shared/{1}/{0}.vbhtml");

                seperateViewLocationFormats.Add(
                    "~/Views/" + app + "/Views/{1}/{0}.cshtml");
                seperateViewLocationFormats.Add(
                    "~/Views/" + app + "/Views/{1}/{0}.vbhtml");
                seperateViewLocationFormats.Add(
                    "~/Views/" + app + "/Views/Shared/{0}.cshtml");
                seperateViewLocationFormats.Add(
                    "~/Views/" + app + "/Views/Shared/{0}.vbhtml");

                areaViewAndPartialViewLocationFormats.Add(
                    "~/Areas/" + app + "/Views/{1}/{0}.cshtml");
                areaViewAndPartialViewLocationFormats.Add(
                    "~/Areas/" + app + "/Views/{1}/{0}.vbhtml");
                areaViewAndPartialViewLocationFormats.Add(
                    "~/Areas/" + app + "/Areas/{2}/Views/{1}/{0}.cshtml");
                areaViewAndPartialViewLocationFormats.Add(
                    "~/Areas/" + app + "/Areas/{2}/Views/{1}/{0}.vbhtml");
                areaViewAndPartialViewLocationFormats.Add(
                    "~/Areas/" + app + "/Areas/{2}/Views/Shared/{0}.cshtml");
                areaViewAndPartialViewLocationFormats.Add(
                    "~/Areas/" + app + "/Areas/{2}/Views/Shared/{0}.vbhtml");
            }

            seperateViewLocationFormats.AddRange(ViewLocationFormats);
            ViewLocationFormats = seperateViewLocationFormats.ToArray();

            masterLocationFormats.AddRange(MasterLocationFormats);
            MasterLocationFormats = masterLocationFormats.ToArray();

            seperateViewLocationFormats.AddRange(PartialViewLocationFormats);
            PartialViewLocationFormats = seperateViewLocationFormats.ToArray();

            areaViewAndPartialViewLocationFormats.AddRange(AreaPartialViewLocationFormats);
            AreaPartialViewLocationFormats = areaViewAndPartialViewLocationFormats.ToArray();

            areaViewAndPartialViewLocationFormats.AddRange(AreaViewLocationFormats);
            AreaViewLocationFormats = areaViewAndPartialViewLocationFormats.ToArray();
        }
    }
}
