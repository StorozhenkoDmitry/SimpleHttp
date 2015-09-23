using System;

namespace SimpleHttp
{
	public sealed class ResponseMessage
	{
		public ResponseMessage(string errorMessage, RequestData requestData = null)
		{
			IsSuccessStatusCode = false;

			ErrorMessage = errorMessage;

			Request = requestData;
		}

		public ResponseMessage(byte[] message, RequestData requestData = null, bool isSuccessStatusCode = true)
		{
			Content = new ResponseContent(message);

			IsSuccessStatusCode = isSuccessStatusCode;

			Request = requestData;
		}

		public ResponseContent Content { get; private set; }

		public bool IsSuccessStatusCode { get; private set; }

		public string ErrorMessage { get; private set; }

		public RequestData Request { get; private set; }
	}
}

