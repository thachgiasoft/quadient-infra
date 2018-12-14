using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace Infrastructure.WebFramework.UI.Interactions
{
    public class WebFormsMessageBoxInteractions : IMessageBoxInteractions
    {
        private readonly Page _currentPage;
        private string ScriptKey { get { return Guid.NewGuid().ToString(); } }
        private const string InfoMessageString = "<script type=\"text/javascript\"> $(function () { $(document).ready(function () { $.msgbox('{0}', { type: \"info\", buttons: [{ type: \"submit\", value: \"Tamam\" }] }); }); }); </script>";
        private const string ErrorMessageString = "<script type=\"text/javascript\"> $(function () { $(document).ready(function () { $.msgbox('{0}', { type: \"error\", buttons: [{ type: \"submit\", value: \"Tamam\" }] }); }); }); </script>";
        private const string PromptMessageString = "<script type=\"text/javascript\"> $(function () { $(document).ready(function () { $.msgbox('{0}', { type: \"prompt\" }, function (result) { if (result) { {1}(result); } }); }); }); </script>";
        private const string ConfirmMessageString = "<script type=\"text/javascript\"> $(function () { $(document).ready(function () { $.msgbox('{0}', { type: \"confirm\", buttons: [ { type: \"submit\", value: \"Evet\" }, { type: \"submit\", value: \"Hayır\" }, { type: \"cancel\", value: \"İptal\" } ] }, function (result) { if (result === \"Evet\") {1}; else {2}; }); }); }); </script>";
        public WebFormsMessageBoxInteractions(Page currentPage)
        {
            _currentPage = currentPage;
        }

        public void ShowMessageBox(Exception exception)
        {
            SetMessageBoxMessage(exception.ToString(), SourceLevels.Error);
        }

        public void ShowMessageBox(string message, SourceLevels sourceLevel)
        {
            SetMessageBoxMessage(message, sourceLevel);
        }

        public void ShowErrorMessageBox(string message)
        {
            _currentPage.ClientScript.RegisterClientScriptBlock(_currentPage.GetType(), ScriptKey, ErrorMessageString.Replace("{0}", message));
        }

        public void ShowInfoMessageBox(string message)
        {
            _currentPage.ClientScript.RegisterClientScriptBlock(_currentPage.GetType(), ScriptKey, InfoMessageString.Replace("{0}", message));

        }

        public void ShowWarningMessageBox(string message)
        {
            _currentPage.ClientScript.RegisterClientScriptBlock(_currentPage.GetType(), ScriptKey, InfoMessageString.Replace("{0}", message));

        }

        public void ShowPromptMessageBox(string message, string callbackFunction)
        {
            _currentPage.ClientScript.RegisterClientScriptBlock(_currentPage.GetType(), ScriptKey, PromptMessageString.Replace("{0}", message).Replace("{1}", callbackFunction));

        }

        public void ShowConfirmMessageBox(string message, string callbackFunctionYes, string callbackFunctionNo)
        {
            _currentPage.ClientScript.RegisterClientScriptBlock(_currentPage.GetType(), ScriptKey, ConfirmMessageString.Replace("{0}", message).Replace("{1}", callbackFunctionYes).Replace("{2}", callbackFunctionNo));

        }

        public void ShowMessageBox(MessageBoxType type, string message)
        {
            throw new NotImplementedException();
        }
        private void SetMessageBoxMessage(string message, SourceLevels sourceLevel)
        {
            switch (sourceLevel)
            {
                case SourceLevels.Error:
                    SetErrorMessage(message);
                    break;
                case SourceLevels.Warning:
                    SetWarningMessage(message);
                    break;
                case SourceLevels.Information:
                    SetInfoMessage(message);
                    break;
                default:
                    SetErrorMessage(message);
                    break;
            }
        }

        private void SetInfoMessage(string message)
        {
            _currentPage.ClientScript.RegisterClientScriptBlock(_currentPage.GetType(), ScriptKey, InfoMessageString.Replace("{0}", message));

        }

        private void SetWarningMessage(string message)
        {
            //todo: bir messagebox daha yazarsak information için bu kısım warning olarak değiştirilebilir.
            //controller.ViewBag.Warning = message;
            _currentPage.ClientScript.RegisterClientScriptBlock(_currentPage.GetType(), ScriptKey, InfoMessageString.Replace("{0}", message));

        }

        private void SetErrorMessage(string message)
        {
            _currentPage.ClientScript.RegisterClientScriptBlock(_currentPage.GetType(), ScriptKey, ErrorMessageString.Replace("{0}", message));

        }
    }
}
