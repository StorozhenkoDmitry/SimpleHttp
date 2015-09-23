using System.Threading.Tasks;

namespace SimpleHttp
{
	public interface IHttpService
	{
		Task<ResponseMessage> GetResponse(RequestData requestData);
	}
}

