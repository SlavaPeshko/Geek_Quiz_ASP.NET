using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using GeekQuiz.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using SimpleInjector;
using SimpleInjector.Advanced;
using SimpleInjector.Integration.Web;
using SimpleInjector.Integration.Web.Mvc;
using SimpleInjector.Integration.WebApi;

namespace GeekQuiz
{
	public class SimpleInjector
	{
		public static void Configure()
		{
			//create container
			var container = new Container();

			container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();

			InitializeContainer(container);

			//register for mvc controller
			container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
			//register for api contoller
			container.RegisterWebApiControllers(GlobalConfiguration.Configuration);

			container.Verify();

			//resolver for mvc
			DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
			//resolver for api
			GlobalConfiguration.Configuration.DependencyResolver =
				new SimpleInjectorWebApiDependencyResolver(container);
		}

		private static void InitializeContainer(Container container)
		{
			container.Register<TriviaContext>(Lifestyle.Scoped);

			container.Register<ApplicationSignInManager>(Lifestyle.Scoped);
			container.Register<ApplicationUserManager>(Lifestyle.Scoped);

			container.Register<IdentityFactoryOptions<ApplicationUserManager>>(Lifestyle.Scoped);
			container.Register<ApplicationDbContext>(Lifestyle.Scoped);

			container.Register<IUserStore<ApplicationUser>>(() =>
				new UserStore<ApplicationUser>(container.GetInstance<ApplicationDbContext>()), Lifestyle.Scoped);

			container.Register(() => container.IsVerifying()
				? new OwinContext(new Dictionary<string, object>()).Authentication
				: HttpContext.Current.GetOwinContext().Authentication, Lifestyle.Scoped);
		}
	}
}