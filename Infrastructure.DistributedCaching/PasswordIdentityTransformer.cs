using System.Collections;
using System.Security;
using System.Security.Principal;
using Tangosol.Net;
using Tangosol.Net.Security;

namespace Infrastructure.DistributedCaching
{
    class PasswordIdentityTransformer : IIdentityTransformer
    {
        public object TransformIdentity(IPrincipal principal, IService service)
        {
            // The IService is not needed so the service argument is being ignored.
            // It could be used, for example, if there were different token types
            // required per service.
            if (principal == null)
            {
                throw new SecurityException("Incomplete Principal");
            }

            

            ArrayList asRoleName = new ArrayList();

            asRoleName.Add("welcome1");
            asRoleName.Add("weblogic");

           
              //  asRoleName.Add("Administrator");
          
            // The token consists of the password plus the user name plus
            // role names as an array of pof-able types, in this case strings.
            //return asRoleName.ToArray();
            return null;
        }
    }
}
