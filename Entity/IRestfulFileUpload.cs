using System;
using System.Collections.Generic;

namespace TaVazando
{
	public interface IRestfulFileUpload
	{
		string SendFileToRest(byte[] file, string fileName, string token, string url, string resource, Dictionary<string, string> data);
	}
}

