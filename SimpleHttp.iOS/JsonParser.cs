using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using ServiceStack.Text;
using SimpleHttp.iOS.Model;
using SimpleHttp.Model;

namespace SimpleHttp.iOS
{
	internal static class JsonParser
	{
		public static Task<string> SerializeToJsonString(Dictionary<string, JsonValueInfo> jsonValues)
		{
			return Task.Factory.StartNew(() => {

				if (jsonValues == null || jsonValues.Count == 0)
				{
					return "";
				} 

				StringBuilder result = new StringBuilder();

				foreach (var keyValue in jsonValues) 
				{
					string jsonValue = keyValue.Value.NeedToWrapWithQuotes 
						? string.Format("\"{0}\":\"{1}\",", keyValue.Key, keyValue.Value.Value)
						: string.Format("\"{0}\":{1},", keyValue.Key, keyValue.Value.Value);

					result.Append(jsonValue);
				}

				return string.Format("{{{0}}}", result.ToString().TrimEnd(','));
			});
		}
			
		public static Task<TaskDescription> ParseTaskDescription(string jsonString)
		{
			return Task.Factory.StartNew(() => {

				if (string.IsNullOrEmpty(jsonString))
				{
					return new TaskDescription(new RequestData { Url = "error" }, Guid.Empty);
				}

				JsonObject json = JsonSerializer.DeserializeFromString<JsonObject>(jsonString);

				Guid guid;

				string jsonGuid = json.Get("guid");

				bool isParsed = Guid.TryParse(jsonGuid, out guid);

				RequestData request = JsonSerializer.DeserializeFromString<JsonObject>(json.GetUnescaped("request")).ConvertTo(CreateRequestDataFromJson);

				return new TaskDescription(request, isParsed ? guid : Guid.Empty);
			});
		}

		private static RequestData CreateRequestDataFromJson(JsonObject json)
		{
			return new RequestData
			{
				Body = json.Get<Dictionary<string, string>>("Body"),
				Tag = json.Get("Tag"),
				Timeout = json.Get<TimeSpan>("Timeout"),
				Type = json.Get<RequestData.RequestType>("Type"),
				Url = json.Get("Url")
			};
		}
	}
}

