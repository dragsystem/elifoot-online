using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmpreendaVc.Domain;

namespace EmpreendaVc.Infrastructure.Queries.Authentication
{
    /// <summary>
    /// Authentication service interface
    /// </summary>
    public partial interface IAuthenticationService
    {
        void SignIn(Usuario userAccount, bool createPersistentCookie);
        void SignOut();
        Usuario GetUserAuthenticated();
    }
}
