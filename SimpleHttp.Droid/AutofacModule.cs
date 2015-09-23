using System;
using Autofac;

namespace SimpleHttp.Droid
{
	internal sealed class AutofacModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			builder.RegisterType<HttpService>().As<IHttpService>().SingleInstance();
		}
	}
}

