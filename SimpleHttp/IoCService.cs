using System;
using Autofac;
using Autofac.Core;

namespace SimpleHttp
{
	public static class IoCService
	{
		public static IContainer Container { get; private set; }

		public static void RegisterModule(IModule module)
		{
			ContainerBuilder builder = new ContainerBuilder();

			builder.RegisterModule(module);

			Container = builder.Build();
		}
	}
}

