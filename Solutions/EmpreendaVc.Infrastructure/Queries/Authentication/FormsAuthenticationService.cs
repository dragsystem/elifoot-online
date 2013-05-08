using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using EmpreendaVc.Domain;
using EmpreendaVc.Infrastructure.Queries.Usuarios;

namespace EmpreendaVc.Infrastructure.Queries.Authentication
{
    public partial class FormsAuthenticationService : IAuthenticationService
    {
        private readonly HttpContextBase _httpContext;
        private readonly HttpSessionStateBase _httpContextSession;
        private readonly TimeSpan _expirationTimeSpan;
        private readonly IUsuarioRepository _UsuarioRepository;
        private Usuario _cachedUserAccount;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="_UsuarioRepository">Usuario service</param>
        public FormsAuthenticationService(HttpContextBase httpContext, IUsuarioRepository UsuarioRepository, HttpSessionStateBase httpContextSession)
        {
            this._httpContext = httpContext;
            this._expirationTimeSpan = FormsAuthentication.Timeout;
            this._UsuarioRepository = UsuarioRepository;
            this._httpContextSession = httpContextSession;
        }


        public virtual void SignIn(Usuario userAccount, bool createPersistentCookie)
        {
            var now = DateTime.UtcNow.ToLocalTime();

            var ticket = new FormsAuthenticationTicket(
                1 /*version*/,
                userAccount.Email,
                now,
                now.Add(_expirationTimeSpan),
                createPersistentCookie,
                userAccount.Email,
                FormsAuthentication.FormsCookiePath);

#if DEBUG
            FormsAuthentication.SetAuthCookie(userAccount.Email, createPersistentCookie);
#else
            var encryptedTicket = FormsAuthentication.Encrypt(ticket);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            cookie.HttpOnly = true;
            cookie.Expires = now.Add(_expirationTimeSpan);
            cookie.Secure = FormsAuthentication.RequireSSL;
            cookie.Path = FormsAuthentication.FormsCookiePath;
            if (FormsAuthentication.CookieDomain != null) {
                cookie.Domain = FormsAuthentication.CookieDomain;
            }
            _httpContext.Response.Cookies.Add(cookie);
#endif

            _cachedUserAccount = userAccount;
        }

        public virtual void SignOut()
        {
            _cachedUserAccount = null;
            FormsAuthentication.SignOut();
        }

        public virtual Usuario GetUserAuthenticated()
        {
            if (_cachedUserAccount != null)
                return _cachedUserAccount;

            if (_httpContext == null ||
                _httpContext.Request == null ||
                !_httpContext.Request.IsAuthenticated ||
                !(_httpContext.User.Identity is FormsIdentity))
            {
                return null;
            }

            var formsIdentity = (FormsIdentity)_httpContext.User.Identity;
            var Usuario = GetUserAuthenticatedFromTicket(formsIdentity.Ticket);
            if (Usuario != null)
                _cachedUserAccount = Usuario;
            return _cachedUserAccount;
        }

        public virtual Usuario GetUserAuthenticatedFromTicket(FormsAuthenticationTicket ticket)
        {
            if (ticket == null)
                throw new ArgumentNullException("ticket");

            var userName = ticket.Name;

            if (String.IsNullOrWhiteSpace(userName))
                return null;
            var Usuario = _UsuarioRepository.GetEmail(userName);
            return Usuario;
        }
    }
}
