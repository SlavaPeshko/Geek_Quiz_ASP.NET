using System.Web;
using System.Web.Http;
using GeekQuiz.Models;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using SimpleInjector.Lifestyles;

namespace GeekQuiz
{
	public static class SimpleInjectorWeApi
	{
		public static void Configure()
		{
			var container = new Container();
			container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

			InitializeContainer(container);

			// This is an extension method from the integration package.
			container.RegisterWebApiControllers(GlobalConfiguration.Configuration);

			container.Verify();

			GlobalConfiguration.Configuration.DependencyResolver =
				new SimpleInjectorWebApiDependencyResolver(container);
		}

		private static void InitializeContainer(Container container)
		{
			container.Register<TriviaContext>(Lifestyle.Scoped);
		}
	}
}