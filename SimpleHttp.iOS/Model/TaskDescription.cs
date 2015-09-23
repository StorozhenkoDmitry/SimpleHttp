using System;

namespace SimpleHttp.iOS.Model
{
	internal sealed class TaskDescription
	{
		public TaskDescription(RequestData request, Guid guid)
		{
			Request = request;
			TaskGuid = guid;
		}

		public RequestData Request { get; set; }
		public Guid TaskGuid { get; set; }
	}
}

