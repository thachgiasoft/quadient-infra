using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.WebFramework.UI.Interactions
{
    public interface IMessageBoxInteractions
    {
        void ShowMessageBox(Exception exception);
        void ShowMessageBox(string message, SourceLevels sourceLevel);
        void ShowErrorMessageBox(string message);
        void ShowInfoMessageBox(string message);
        void ShowWarningMessageBox(string message);
        void ShowPromptMessageBox(string message, string callbackFunction);
        void ShowConfirmMessageBox(string message, string callbackFunctionYes, string callbackFunctionNo);
    }
}
