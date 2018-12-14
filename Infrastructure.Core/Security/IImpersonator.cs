using System;

namespace Infrastructure.Core.Security
{
    public interface IImpersonator
    {
        void RunImpersonatedOperation(string domain, string userName, string password, Action action);
        bool ImpersonateValidUser(string userName, string password, string domain);
        void UndoImpersonation();
    }
}