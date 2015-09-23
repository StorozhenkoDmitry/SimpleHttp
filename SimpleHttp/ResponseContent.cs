using System.Text;

namespace SimpleHttp
{
	public sealed class ResponseContent
	{
		public ResponseContent(byte[] message)
		{
			_message = message;
		}

		public string AsString()
		{
			if (_message == null || _message.Length == 0)
			{
				return "";
			}

			return Encoding.UTF8.GetString(_message, 0, _message.Length);
		}

		public byte[] AsBytesArray()
		{
			return _message;
		}

		private byte[] _message;
	}
}

