using Infrastructure.WebFramework.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Infrastructure.WebFramework.Fakes
{
    public class PageScriptManager : IPageScriptManager
    {
        private readonly IPage _page;
        private readonly IWebHelper _webHelper;
        private readonly IList<InfraScript> _scriptIncludes = new List<InfraScript>();
        private readonly IList<InfraScript> _scripts = new List<InfraScript>();
        private readonly Dictionary<string, Action> _scriptsActions = new Dictionary<string, Action>();

        public PageScriptManager(IPage page, IWebHelper webHelper)
        {
            _page = page;
            _webHelper = webHelper;
        }

        /// <summary>
        /// Adds a script file reference to the View.
        /// </summary>
        /// <param name="scriptPath">The URL of the script file.</param>
        /// <returns>Returns the SimpleScriptManager</returns>
        public PageScriptManager ScriptInclude(string scriptPath)
        {
            return ScriptInclude(Guid.NewGuid().ToString(), scriptPath);
        }

        /// <summary>
        /// Adds a script file reference to the View.
        /// </summary>
        /// <param name="key">A unique identifier for the script file.</param>
        /// <param name="scriptPath">The URL of the script file.</param>
        /// <returns>Returns the SimpleScriptManager</returns>
        public PageScriptManager ScriptInclude(string key, string scriptPath)
        {
            if (_scriptIncludes.FirstOrDefault(s => s.Key == key) == null)
            {
                // Check if the scriptPath is a Virtual Path
                if (scriptPath.StartsWith("~/"))
                {
                    // Convert the Virtual Path to an Application Absolute Path
                    scriptPath = VirtualPathUtility.ToAbsolute(scriptPath);
                }
                _scriptIncludes.Add(new InfraScript() { Key = key, PathOrResourceNameOrFullScript = scriptPath, IsRendered = false });
            }
            return this;
        }

        /// <summary>
        /// Adds a script file reference to the View for an Embedded Web Resource.
        /// </summary>
        /// <typeparam name="T">The Type whos Assembly contains the Web Resource.</typeparam>
        /// <param name="key">A unique identifier for the script file.</param>
        /// <param name="resourceName">The name of the Web Resource.</param>
        /// <returns>Returns the SimpleScriptManager</returns>
        public PageScriptManager ScriptInclude<T>(string key, string resourceName)
        {
            return ScriptInclude(key, _webHelper.GetResourceUrl<T>(resourceName));
        }

        /// <summary>
        /// Adds a script file reference to the View for an Embedded Web Resource.
        /// </summary>
        /// <typeparam name="T">The Type whos Assembly contains the Web Resource.</typeparam>
        /// <param name="resourceName">The name of the Web Resource.</param>
        /// <returns>Returns the SimpleScriptManager</returns>
        public PageScriptManager ScriptInclude<T>(string resourceName)
        {
            return ScriptInclude(_webHelper.GetResourceUrl<T>(resourceName));
        }

        /// <summary>
        /// Adds a script block to the View.
        /// </summary>
        /// <param name="javascript">The JavaScript code to include in the View.</param>
        /// <returns>Returns the SimpleScriptManager</returns>
        public PageScriptManager Script(string javascript)
        {
            return Script(Guid.NewGuid().ToString(), javascript);
        }

        /// <summary>
        /// Adds a script block to the View.
        /// </summary>
        /// <param name="key">A unique identifier for the script.</param>
        /// <param name="javascript">The JavaScript code to include in the View.</param>
        /// <returns>Returns the SimpleScriptManager</returns>
        public PageScriptManager Script(string key, string javascript)
        {
            if (_scripts.FirstOrDefault(s => s.Key == key) == null && !_scriptsActions.ContainsKey(key))
            {
                _scripts.Add(new InfraScript() { Key = key, PathOrResourceNameOrFullScript = javascript, IsRendered = false });
            }
            return this;
        }

        /// <summary>
        /// Adds a script block to the View.
        /// </summary>
        /// <param name="javascript">The JavaScript code to include in the View.</param>
        /// <returns>Returns the SimpleScriptManager</returns>
        public PageScriptManager Script(Action javascript)
        {
            return Script(Guid.NewGuid().ToString(), javascript);
        }

        /// <summary>
        /// Adds a script block to the View.
        /// </summary>
        /// <param name="key">A unique identifier for the script.</param>
        /// <param name="javascript">The JavaScript code to include in the View.</param>
        /// <returns>Returns the SimpleScriptManager</returns>
        public PageScriptManager Script(string key, Action javascript)
        {
            if (_scripts.FirstOrDefault(s => s.Key == key) == null && !_scriptsActions.ContainsKey(key))
            {
                _scriptsActions.Add(key, javascript);
            }
            return this;
        }

        /// <summary>
        /// Renders the InfraScriptManager  
        /// </summary>
        public MvcHtmlString Render()
        {

            var writer = new StringBuilder();

            // Render All Script Includes to the View
            foreach (var scriptInclude in _scriptIncludes.Where(scriptInclude => scriptInclude.IsRendered == false))
            {
                writer.AppendLine(String.Format("<script type='text/javascript' src='{0}'></script>",
                                                scriptInclude.PathOrResourceNameOrFullScript));
                scriptInclude.IsRendered = true;
            }

            // Render All other scripts to the View
            if (_scripts.Count > 0 || _scriptsActions.Count > 0)
            {
                writer.AppendLine("<script type='text/javascript'>");

                if (_scripts.Count > 0)
                {
                    foreach (var script in _scripts.Where(script => script.IsRendered == false))
                    {
                        writer.AppendLine(script.PathOrResourceNameOrFullScript);

                        script.IsRendered = true;
                    }
                }
                if (_scriptsActions.Count > 0)
                {
                    foreach (var script in _scriptsActions)
                    {
                        script.Value();
                    }
                }

                writer.AppendLine("</script>");
            }
            return MvcHtmlString.Create(writer.ToString());
        }
    }
}
