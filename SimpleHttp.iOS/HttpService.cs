using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Foundation;
using ServiceStack.Text;
using SimpleHttp.Model;
using UIKit;

namespace SimpleHttp.iOS
{
	public sealed class HttpService : IHttpService
	{
		#region public constructors

		public HttpService()
		{
			_configurationsPool = new Dictionary<string, NSUrlSessionConfiguration>();
			_sessionsPool = new Dictionary<string, NSUrlSession>();
			_taskCompletionSourcePool = new Dictionary<Guid, TaskCompletionSource<ResponseMessage>>();

			_taskComletionPoolLock = new object();
		}

		#endregion

		#region public methods

		public async Task<ResponseMessage> GetResponse(RequestData requestData)
		{
			EventHandler<DownloadEventArgs> downloadedAction = (sender, e) => {

				if (e.TaskGuid.Equals(Guid.Empty))
				{
					lock (_taskComletionPoolLock)
					{
						if (_taskCompletionSourcePool.Count == 1)
						{
							var completionSource = _taskCompletionSourcePool.First();

							completionSource.Value.TrySetResult(new ResponseMessage("Empty task guid!", requestData));

							_taskCompletionSourcePool.Clear();
						}
					}

					return;
				}

				string filePath = e.FileLocation.Replace("file://", "").Replace("%20", " ");

				ResponseMessage message;

				try
				{
					if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
					{
						byte[] data = File.ReadAllBytes(filePath);

						message = new ResponseMessage(data, requestData);
					}
					else 
					{
						message = new ResponseMessage(e.ErrorDescription, requestData);
					}
				}
				catch (Exception ex)
				{
					message = new ResponseMessage(ex.Message, requestData);
				}

				if (_taskCompletionSourcePool.ContainsKey(e.TaskGuid))
				{
					_taskCompletionSourcePool[e.TaskGuid].TrySetResult(message);
				}

				lock (_taskCompletionSourcePool)
				{
					_taskCompletionSourcePool.Remove(e.TaskGuid);
				}
			};

			Guid taskGuid = Guid.NewGuid();

			lock (_taskComletionPoolLock)
			{
				_taskCompletionSourcePool.Add(taskGuid, new TaskCompletionSource<ResponseMessage>());
			}

			BegingDownloading(requestData, downloadedAction, taskGuid);

			return await _taskCompletionSourcePool[taskGuid].Task;
		}

		#endregion

		#region private methods

		private async Task BegingDownloading(RequestData requestData, EventHandler<DownloadEventArgs> downloadedAction, Guid taskGuid)
		{
			NSUrlSession session = GetSession(requestData, downloadedAction);

			NSUrlSessionDownloadTask task;

			if (requestData.Type == RequestData.RequestType.Get)
			{
				NSString urlSting = new NSString(requestData.Url);

				urlSting = urlSting.CreateStringByAddingPercentEscapes(NSStringEncoding.UTF8);

				task = session.CreateDownloadTask(NSUrl.FromString(urlSting));
			}
			else
			{
				NSMutableUrlRequest request = new NSMutableUrlRequest(NSUrl.FromString(requestData.Url))
				{
					Body = FormatBody(requestData.Body),
					HttpMethod = "POST"
				};

				task = session.CreateDownloadTask(request);
			}

			if (task != null)
			{
				task.TaskDescription = await JsonParser.SerializeToJsonString(new Dictionary<string, JsonValueInfo> { 
					{ "request", new JsonValueInfo(requestData.ToJson(), false) },
					{ "guid", new JsonValueInfo(taskGuid) }
				});

				task.Resume();
			}
			else
			{
				downloadedAction(this, new DownloadEventArgs("", taskGuid, requestData, "Can't create NSUrlSessionDownloadTask."));
			}
		}

		private NSUrlSession GetSession(RequestData requestData, EventHandler<DownloadEventArgs> downloadedAction)
		{
			NSUrlSession session;

			if (_sessionsPool.ContainsKey(requestData.Url))
			{
				session = _sessionsPool[requestData.Url];

				session.Configuration.TimeoutIntervalForRequest = requestData.Timeout.TotalSeconds;
				session.Configuration.TimeoutIntervalForResource = requestData.Timeout.TotalSeconds;
			}
			else
			{
				NSUrlSessionConfiguration configuration = CreateConfiguration(requestData.Url);

				configuration.TimeoutIntervalForRequest = requestData.Timeout.TotalSeconds;
				configuration.TimeoutIntervalForResource = requestData.Timeout.TotalSeconds;

				DownloadDelegate sessionDelegate = new DownloadDelegate();

				sessionDelegate.DownloadCompleted += downloadedAction;

				session = NSUrlSession.FromConfiguration(configuration, sessionDelegate, 
					new NSOperationQueue());

				_sessionsPool.Add(requestData.Url, session);
			}

			return session;
		}

		private NSUrlSessionConfiguration CreateConfiguration(string identeficator)
		{
			if (_configurationsPool.ContainsKey(identeficator))
			{
				return _configurationsPool[identeficator];
			}
			else
			{
				var configuration = UIDevice.CurrentDevice.CheckSystemVersion(8, 0) 
					? NSUrlSessionConfiguration.CreateBackgroundSessionConfiguration(identeficator)
					: NSUrlSessionConfiguration.BackgroundSessionConfiguration(identeficator);

				return configuration;
			}
		}

		private string FormatBody(Dictionary<string, string> body)
		{
			StringBuilder formattedBody = new StringBuilder();

			foreach (var value in body) 
			{
				formattedBody.Append(string.Format("{0}={1}&", value.Key, value.Value));
			}

			return formattedBody.ToString().TrimEnd('&');
		}

		#endregion

		#region private fields

		private object _taskComletionPoolLock;

		private Dictionary<Guid, TaskCompletionSource<ResponseMessage>> _taskCompletionSourcePool;
		private Dictionary<string, NSUrlSessionConfiguration> _configurationsPool;
		private Dictionary<string, NSUrlSession> _sessionsPool;

		#endregion
	}
}

