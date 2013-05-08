﻿namespace EmpreendaVc.Web.Mvc
{
    using System;
    using System.Reflection;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Castle.Windsor;

    // EmpreendaVc.Web.Mvc.CastleWindsor
    using CastleWindsor;

    using CommonServiceLocator.WindsorAdapter;

    using Controllers;

    using Infrastructure.NHibernateMaps;

    using log4net.Config;

    using Microsoft.Practices.ServiceLocation;

    using SharpArch.NHibernate;
    using SharpArch.NHibernate.Web.Mvc;
    using SharpArch.Web.Mvc.Castle;
    using SharpArch.Web.Mvc.ModelBinder;
    using NHibernate.Tool.hbm2ddl;
    using System.Web;
    using EmpreendaVc.Web.Mvc.Util;


    /// <summary>
    /// Represents the MVC Application
    /// </summary>
    /// <remarks>
    /// For instructions on enabling IIS6 or IIS7 classic mode, 
    /// visit http://go.microsoft.com/?LinkId=9394801
    /// </remarks>
    public class MvcApplication : System.Web.HttpApplication
    {
        private WebSessionStorage webSessionStorage;

        /// <summary>
        /// Due to issues on IIS7, the NHibernate initialization must occur in Init().
        /// But Init() may be invoked more than once; accordingly, we introduce a thread-safe
        /// mechanism to ensure it's only initialized once.
        /// See http://msdn.microsoft.com/en-us/magazine/cc188793.aspx for explanation details.
        /// </summary>
        public override void Init() {
            base.Init();
            this.webSessionStorage = new WebSessionStorage(this);
        }

        protected void Application_BeginRequest(object sender, EventArgs e) {
            NHibernateInitializer.Instance().InitializeNHibernateOnce(this.InitialiseNHibernateSessions);
        }

        protected void Application_Error(object sender, EventArgs e) {
            // Useful for debugging
            var logger = log4net.LogManager.GetLogger("LogInFile");
            Exception ex = this.Server.GetLastError();
            Application["TheException"] = ex.ToString();

            while (ex.InnerException != null) ex = ex.InnerException;

            if (ex is HttpException && ((HttpException)ex).ErrorCode == 404) {
                if (logger.IsWarnEnabled) {
                    logger.Warn("404 - Página não encontrada", ex);
                }
            }
            else {
                if (logger.IsFatalEnabled) {
                    logger.Fatal("Ocorreu erro não tratado: ", ex);
                }
            }

            var reflectionTypeLoadException = ex as ReflectionTypeLoadException;
        }

        protected void Application_Start() {
            XmlConfigurator.Configure();

            ViewEngines.Engines.Clear();

            ViewEngines.Engines.Add(new RazorViewEngine());

            ModelBinders.Binders.DefaultBinder = new SharpModelBinder();
            ModelBinders.Binders.Add(typeof(decimal), new DecimalModelBinder());
            ModelBinders.Binders.Add(typeof(decimal?), new DecimalNullableModelBinder());

            ModelValidatorProviders.Providers.Add(new ClientDataTypeModelValidatorProvider());

            this.InitializeServiceLocator();

            AreaRegistration.RegisterAllAreas();
            RouteRegistrar.RegisterRoutesTo(RouteTable.Routes);
        }

        /// <summary>
        /// Instantiate the container and add all Controllers that derive from
        /// WindsorController to the container.  Also associate the Controller
        /// with the WindsorContainer ControllerFactory.
        /// </summary>
        protected virtual void InitializeServiceLocator() {
            IWindsorContainer container = new WindsorContainer();

            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(container));

            container.RegisterControllers(typeof(HomeController).Assembly);
            ComponentRegistrar.AddComponentsTo(container);

            ServiceLocator.SetLocatorProvider(() => new WindsorServiceLocator(container));
        }

        private void InitialiseNHibernateSessions() {
            NHibernateSession.ConfigurationCache = new NHibernateConfigurationFileCache();

            var config = NHibernateSession.Init(
             this.webSessionStorage,
             new[] { Server.MapPath("~/bin/EmpreendaVc.Infrastructure.dll") },
             new AutoPersistenceModelGenerator().Generate(),
             Server.MapPath("~/NHibernate.config"));

            //atualiza mudanças de bando de dados
            SchemaUpdate updater = new SchemaUpdate(config);
            updater.Execute(false, true);
        }
    }
}