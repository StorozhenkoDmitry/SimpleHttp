using System;

namespace SimpleHttp.iOS
{
	internal sealed class DownloadEventArgs: EventArgs
	{
		public DownloadEventArgs(string fileLocation, Guid taskGuid, RequestData request, string errorDescription = null)
		{
			FileLocation = fileLocation;
			TaskGuid = taskGuid;
			ErrorDescription = errorDescription;
			Request = request;
		}

		public string FileLocation { get; set; }

		public string ErrorDescription { get; set; }

		public Guid TaskGuid { get; set; }

		public RequestData Request { get; set; }
	}
}

