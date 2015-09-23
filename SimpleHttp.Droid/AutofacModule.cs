using System;
using Autofac;

namespace SimpleHttp.Droid
{
	internal sealed class AutofacModule : Module
	{
		static AutofacModule()
		{
			IoCService.RegisterModule(new AutofacModule());
		}

		private AutofacModule()
		{
		}

		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			builder.RegisterType<HttpService>().As<IHttpService>();
		}
	}
}

