using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Infrastructure.WebFramework.Fakes
{
    public interface IPageScriptManager
    {
        /// <summary>
        /// Adds a script file reference to the View.
        /// </summary>
        /// <param name="scriptPath">The URL of the script file.</param>
        /// <returns>Returns the SimpleScriptManager</returns>
        PageScriptManager ScriptInclude(string scriptPath);

        /// <summary>
        /// Adds a script file reference to the View.
        /// </summary>
        /// <param name="key">A unique identifier for the script file.</param>
        /// <param name="scriptPath">The URL of the script file.</param>
        /// <returns>Returns the SimpleScriptManager</returns>
        PageScriptManager ScriptInclude(string key, string scriptPath);

        /// <summary>
        /// Adds a script file reference to the View for an Embedded Web Resource.
        /// </summary>
        /// <typeparam name="T">The Type whos Assembly contains the Web Resource.</typeparam>
        /// <param name="key">A unique identifier for the script file.</param>
        /// <param name="resourceName">The name of the Web Resource.</param>
        /// <returns>Returns the SimpleScriptManager</returns>
        PageScriptManager ScriptInclude<T>(string key, string resourceName);

        /// <summary>
        /// Adds a script file reference to the View for an Embedded Web Resource.
        /// </summary>
        /// <typeparam name="T">The Type whos Assembly contains the Web Resource.</typeparam>
        /// <param name="resourceName">The name of the Web Resource.</param>
        /// <returns>Returns the SimpleScriptManager</returns>
        PageScriptManager ScriptInclude<T>(string resourceName);

        /// <summary>
        /// Adds a script block to the View.
        /// </summary>
        /// <param name="javascript">The JavaScript code to include in the View.</param>
        /// <returns>Returns the SimpleScriptManager</returns>
        PageScriptManager Script(string javascript);

        /// <summary>
        /// Adds a script block to the View.
        /// </summary>
        /// <param name="key">A unique identifier for the script.</param>
        /// <param name="javascript">The JavaScript code to include in the View.</param>
        /// <returns>Returns the SimpleScriptManager</returns>
        PageScriptManager Script(string key, string javascript);

        /// <summary>
        /// Adds a script block to the View.
        /// </summary>
        /// <param name="javascript">The JavaScript code to include in the View.</param>
        /// <returns>Returns the SimpleScriptManager</returns>
        PageScriptManager Script(Action javascript);

        /// <summary>
        /// Adds a script block to the View.
        /// </summary>
        /// <param name="key">A unique identifier for the script.</param>
        /// <param name="javascript">The JavaScript code to include in the View.</param>
        /// <returns>Returns the SimpleScriptManager</returns>
        PageScriptManager Script(string key, Action javascript);

        /// <summary>
        /// Renders the ScriptManager  
        /// </summary>
        MvcHtmlString Render();
    }
}
