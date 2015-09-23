namespace SimpleHttp.Model
{
	public struct JsonValueInfo
	{
		public JsonValueInfo(object value, bool needToWrapWithQuotes = true)
		{
			_value = value;
			_needToWrapWithQuotes = needToWrapWithQuotes;
		}

		public object Value
		{
			get
			{
				return _value;
			}
		}

		public bool NeedToWrapWithQuotes
		{
			get
			{
				return _needToWrapWithQuotes;
			}
		}

		private object _value;
		private bool _needToWrapWithQuotes;
	}
}

