using System.Collections.Generic;

namespace HttpClient
{
	internal static class Program
	{
		private static void Main()
		{
			// Connect to server
			var headers = new Dictionary<string, string>
			{
				{ "Accepting-Encoding", "gzip" }
			};
			var response = Client.Get("www.google.com", headers);
		}
	}
}
