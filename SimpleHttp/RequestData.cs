using System;
using System.Collections.Generic;

namespace SimpleHttp
{
	public sealed class RequestData
	{
		[Flags]
		public enum RequestType
		{
			Get,
			Post
		}

		public RequestType Type { get; set; }

		public Dictionary<string, string> Body { get; set; }

		public string Url { get; set; }

		public string Tag { get; set; }

		public TimeSpan Timeout { get; set; }
	}
}

