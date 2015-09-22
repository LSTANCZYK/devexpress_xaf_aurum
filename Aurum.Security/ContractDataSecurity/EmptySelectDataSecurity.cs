using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Utils;

namespace Aurum.Security
{
    /// <summary>
    /// Реализует безопасную выборку данных без правил фильтрации
    /// </summary>
    class EmptySelectDataSecurity : ISelectDataSecurity
    {
        /// <contentfrom cref="ISelectDataSecurity" />
        public IList<string> GetObjectCriteria(Type type)
        {
            return new string[0];
        }

        /// <contentfrom cref="ISelectDataSecurity" />
        public IList<string> GetMemberCriteria(Type type, string memberName)
        {
            return new string[0];
        }

        /// <contentfrom cref="ISelectDataSecurity" />
        public bool IsGranted(IPermissionRequest permissionRequest)
        {
            return true;
        }

        /// <contentfrom cref="ISelectDataSecurity" />
        public IList<bool> IsGranted(IList<IPermissionRequest> permissionRequests)
        {
            Guard.ArgumentNotNull(permissionRequests, "permissionRequests");
            List<bool> result = new List<bool>(permissionRequests.Count);
            for (int i = 0; i < permissionRequests.Count; i++)
            {
                result.Add(IsGranted(permissionRequests[i]));
            }
            return result;
        }
    }
}
