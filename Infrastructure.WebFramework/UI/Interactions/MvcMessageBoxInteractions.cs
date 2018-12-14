using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Infrastructure.WebFramework.UI.Interactions
{
    /// <summary>
    /// ayni view icin viewbag
    /// viewler arasi gecis icin tempdata kullanilir.
    /// bu bilgilere gore css veya js eklemesi yaparak mesaj kutusu vs cikarabilirisniz.
    /// </summary>
    public class MvcMessageBoxInteractions : IMessageBoxInteractions
    {
        private readonly Controller _currentController;

        public MvcMessageBoxInteractions(Controller controller)
        {
            _currentController = controller;
        }
        public void ShowMessageBox(Exception exception)
        {
            SetMessageBoxMessage(_currentController, exception.ToString(), SourceLevels.Error);
        }

        public void ShowMessageBox(string message, SourceLevels sourceLevel)
        {
            SetMessageBoxMessage(_currentController, message, sourceLevel);
        }

        private void SetMessageBoxMessage(Controller controller, string message, SourceLevels sourceLevel)
        {
            switch (sourceLevel)
            {
                case SourceLevels.Error:
                    SetErrorMessage(controller, message);
                    break;
                case SourceLevels.Warning:
                    SetWarningMessage(controller, message);
                    break;
                case SourceLevels.Information:
                    SetInfoMessage(controller, message);
                    break;
                default:
                    SetErrorMessage(controller, message);
                    break;
            }
        }

        private void SetInfoMessage(Controller controller, string message)
        {
            controller.ViewBag.Info = message;
            controller.TempData["Info"] = message;
        }

        private void SetWarningMessage(Controller controller, string message)
        {
            //todo: bir messagebox daha yazarsak information için bu kısım warning olarak değiştirilebilir.
            //controller.ViewBag.Warning = message;
            controller.ViewBag.Info = message;
            controller.TempData["Info"] = message;
        }

        private void SetErrorMessage(Controller controller, string message)
        {
            controller.ViewBag.Error = message;
            controller.TempData["Error"] = message;
        }

        public void ShowErrorMessageBox(string message)
        {
            _currentController.ViewBag.Error = message;
            _currentController.TempData["Error"] = message;
        }

        public void ShowInfoMessageBox(string message)
        {
            _currentController.ViewBag.Info = message;
            _currentController.TempData["Info"] = message;
            _currentController.TempData["Success"] = message;
        }

        public void ShowWarningMessageBox(string message)
        {
            //todo: bir messagebox daha yazarsak information için bu kısım warning olarak değiştirilebilir.
            //controller.ViewBag.Warning = message;
            _currentController.ViewBag.Info = message;
            _currentController.TempData["Info"] = message;
        }

        public void ShowPromptMessageBox(string message, string callbackFunction)
        {
            _currentController.ViewBag.Prompt = message;
            _currentController.TempData["Prompt"] = message;
        }

        public void ShowConfirmMessageBox(string message, string callbackFunctionYes, string callbackFunctionNo)
        {
            _currentController.ViewBag.Confirm = message;
            _currentController.TempData["Confirm"] = message;
        }
    }
}
