using System.Net.Http;
using System.Threading.Tasks;

namespace SimpleHttp.Droid
{
	public sealed class HttpService : IHttpService
	{
		public static void Init()
		{
			IoCService.RegisterModule(new AutofacModule());
		}

		public async Task<ResponseMessage> GetResponse(RequestData requestData)
		{
			if (requestData == null)
			{
				return new ResponseMessage("Request data is null.", requestData);
			}

			if ((requestData.Body == null || requestData.Body.Count == 0) && requestData.Type == RequestData.RequestType.Post)
			{
				return new ResponseMessage("Request body is empty. Set body or use GetAsync for this request.", requestData);
			}

			var client = new HttpClient
			{
				Timeout = requestData.Timeout
			};

			HttpResponseMessage response = requestData.Type == RequestData.RequestType.Get 
				? await client.GetAsync(requestData.Url) 
				: await client.PostAsync(requestData.Url, new FormUrlEncodedContent(requestData.Body));			

			ResponseMessage message;

			if (response != null)
			{
				message = response.Content != null 
					? new ResponseMessage(await response.Content.ReadAsByteArrayAsync(), requestData, response.IsSuccessStatusCode) 
					: new ResponseMessage(response.ReasonPhrase, requestData);
			}
			else
			{
				message = new ResponseMessage("Response is null.", requestData);
			}

			return message;
		}
	}
}

