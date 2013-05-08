using SharpArch.Web.Mvc.Castle;

namespace EmpreendaVc.Web.Mvc.CastleWindsor
{
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;

    using SharpArch.Domain.PersistenceSupport;
    using SharpArch.NHibernate;
    using SharpArch.NHibernate.Contracts.Repositories;
    using System.Web;
    using EmpreendaVc.Infrastructure.Queries.Authentication;
    using EmpreendaVc.Infrastructure.Queries.Usuarios;
    

    public class ComponentRegistrar
    {
        public static void AddComponentsTo(IWindsorContainer container) {
            AddGenericRepositoriesTo(container);
            AddCustomRepositoriesTo(container);
            AddQueryObjectsTo(container);
            AddTasksTo(container);
            AddCommandsTo(container);
        }

        private static void AddTasksTo(IWindsorContainer container) {
            container.Register(
                AllTypes
                    .FromAssemblyNamed("EmpreendaVc.Tasks")
                    .Pick()
                    .WithService.FirstNonGenericCoreInterface("EmpreendaVc.Domain"));
        }

        private static void AddCustomRepositoriesTo(IWindsorContainer container) {
            container.Register(
                AllTypes
                    .FromAssemblyNamed("EmpreendaVc.Infrastructure")
                    .Pick()
                    .WithService.FirstNonGenericCoreInterface("EmpreendaVc.Domain"));
        }

        private static void AddGenericRepositoriesTo(IWindsorContainer container) {

            container.Register(Component.For<HttpRequestBase>().LifeStyle.PerWebRequest
                  .UsingFactoryMethod(() => new HttpRequestWrapper(HttpContext.Current.Request)));
            container.Register(Component.For<HttpContextBase>().LifeStyle.PerWebRequest
              .UsingFactoryMethod(() => new HttpContextWrapper(HttpContext.Current)));
            container.Register(Component.For<HttpSessionStateBase>().LifeStyle.PerWebRequest
    .UsingFactoryMethod(() => new HttpSessionStateWrapper(HttpContext.Current.Session)));



            //     builder.Register(c => HttpContext.Current != null ?
            //(new HttpContextWrapper(HttpContext.Current) as HttpContextBase) :
            //(new FakeHttpContext("~/") as HttpContextBase))
            //.As<HttpContextBase>()
            //.InstancePerHttpRequest();
            //     builder.Register(c => c.Resolve<HttpContextBase>().Request)
            //         .As<HttpRequestBase>()
            //         .InstancePerHttpRequest();
            //     builder.Register(c => c.Resolve<HttpContextBase>().Response)
            //         .As<HttpResponseBase>()
            //         .InstancePerHttpRequest();
            //     builder.Register(c => c.Resolve<HttpContextBase>().Server)
            //         .As<HttpServerUtilityBase>()
            //         .InstancePerHttpRequest();
            //     builder.Register(c => c.Resolve<HttpContextBase>().Session)
            //         .As<HttpSessionStateBase>()
            //         .InstancePerHttpRequest();



            container.Register(
                Component.For(typeof(IQuery<>))
                    .ImplementedBy(typeof(NHibernateQuery<>))
                    .Named("NHibernateQuery"));

            container.Register(
                Component.For(typeof(IEntityDuplicateChecker))
                    .ImplementedBy(typeof(EntityDuplicateChecker))
                    .Named("entityDuplicateChecker"));

            container.Register(
                Component.For(typeof(INHibernateRepository<>))
                    .ImplementedBy(typeof(NHibernateRepository<>))
                    .Named("nhibernateRepositoryType")
                    .Forward(typeof(IRepository<>)));

            container.Register(
                Component.For(typeof(INHibernateRepositoryWithTypedId<,>))
                    .ImplementedBy(typeof(NHibernateRepositoryWithTypedId<,>))
                    .Named("nhibernateRepositoryWithTypedId")
                    .Forward(typeof(IRepositoryWithTypedId<,>)));

            container.Register(
                    Component.For(typeof(ISessionFactoryKeyProvider))
                        .ImplementedBy(typeof(DefaultSessionFactoryKeyProvider))
                        .Named("sessionFactoryKeyProvider"));

            container.Register(
                    Component.For(typeof(SharpArch.Domain.Commands.ICommandProcessor))
                        .ImplementedBy(typeof(SharpArch.Domain.Commands.CommandProcessor))
                        .Named("commandProcessor"));

            container.Register(
                   Component.For(typeof(EmpreendaVc.Infrastructure.Queries.Authentication.IAuthenticationService)).LifeStyle.PerWebRequest
                       .ImplementedBy(typeof(EmpreendaVc.Infrastructure.Queries.Authentication.FormsAuthenticationService))
                       .Named("formsAuthenticationService"));

            container.Register(
              Component.For(typeof(EmpreendaVc.Infrastructure.Queries.Usuarios.IUsuarioRepository))
                  .ImplementedBy(typeof(EmpreendaVc.Infrastructure.Queries.Usuarios.UsuarioRepository))
                  .Named("usuarioRepository"));

            //container.Register(
            //             Component.For(typeof(IProjectRepository))
            //                 .ImplementedBy(typeof(ProjectRepository))
            //                 .Named("projectRepository"));

        }

        private static void AddQueryObjectsTo(IWindsorContainer container) {
            container.Register(
                AllTypes.FromAssemblyNamed("EmpreendaVc.Web.Mvc")
                    .Pick()
                    .WithService.FirstInterface());
        }

        private static void AddCommandsTo(IWindsorContainer container) {
            container.Register(
                AllTypes.FromAssemblyNamed("EmpreendaVc.Tasks")
                    .Pick()
                    .WithService.FirstInterface());
        }
    }
}