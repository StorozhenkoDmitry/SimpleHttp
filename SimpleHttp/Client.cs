using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Autofac;

namespace SimpleHttp
{
	public sealed class Client
	{
		public TimeSpan Timeout { get; set; }

		public async Task<ResponseMessage> PostAsync(string url, Dictionary<string, string> body, string tag = null)
		{
			RequestData requestData = new RequestData
			{
				Body = body,
				Tag = tag,
				Url = url,
				Type = RequestData.RequestType.Post,
				Timeout = Timeout 
			};

			using (var scope = IoCService.Container.BeginLifetimeScope())
			{
				return await scope.Resolve<IHttpService>().GetResponse(requestData);
			}

		}

		public async Task<ResponseMessage> GetAsync(string url, string tag = null)
		{
			RequestData requestData = new RequestData
			{
				Tag = tag,
				Url = url,
				Type = RequestData.RequestType.Get,
				Timeout = Timeout
			};

			using (var scope = IoCService.Container.BeginLifetimeScope())
			{
				return await scope.Resolve<IHttpService>().GetResponse(requestData);
			}
		}
	}
}

