using System;
using Foundation;
using SimpleHttp.iOS.Model;

namespace SimpleHttp.iOS
{
	internal sealed class DownloadDelegate : NSUrlSessionDownloadDelegate
	{
		public event EventHandler<DownloadEventArgs> DownloadCompleted;

		public override void DidFinishDownloading(NSUrlSession session, NSUrlSessionDownloadTask downloadTask, NSUrl location)
		{
			OnDownloadCompleted(location.ToString(), downloadTask.TaskDescription ?? "");
		}

		public override void DidCompleteWithError(NSUrlSession session, NSUrlSessionTask task, NSError error)
		{
			if (error != null)
			{
				TaskDescription description = JsonParser.ParseTaskDescription(task.TaskDescription).Result;

				OnDownloadCompleted("", task.TaskDescription ?? "", error.LocalizedDescription);
			}
		}

		private void OnDownloadCompleted(string location, string taskDescription, string errorDescription = null)
		{
			if (DownloadCompleted != null)
			{
				TaskDescription description = JsonParser.ParseTaskDescription(taskDescription).Result;

				DownloadCompleted(this, new DownloadEventArgs(location, description.TaskGuid, description.Request, errorDescription));
			}
		}
	}
}

