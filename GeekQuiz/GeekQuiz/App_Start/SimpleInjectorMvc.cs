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
	public static class SimpleInjectorMvc
	{
		public static void Configure()
		{
			var container = new Container();

			container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();

			InitializeContainer(container);

			container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
			container.RegisterMvcIntegratedFilterProvider();

			container.Verify();

			DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
		}

		private static void InitializeContainer(Container container)
		{
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