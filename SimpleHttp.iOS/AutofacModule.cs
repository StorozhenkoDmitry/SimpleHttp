using System;
using Autofac;

namespace SimpleHttp.iOS
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

