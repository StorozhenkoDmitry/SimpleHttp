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
			FormUrlEncodedContent body = new FormUrlEncodedContent(requestData.Body);

			var client = new HttpClient
			{
				Timeout = requestData.Timeout
			};

			HttpResponseMessage response = requestData.Type == RequestData.RequestType.Get 
					? await client.GetAsync(requestData.Url) 
					: await client.PostAsync(requestData.Url, body);			

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

