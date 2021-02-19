using System;
using TaVazando.Droid;
using System.Net;
using System.IO;
using System.Text;
using System.Collections.Generic;
using RestSharp;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[assembly: Xamarin.Forms.Dependency(typeof(RestFullFileUpload))]

namespace TaVazando.Droid
{
	public class RestFullFileUpload: IRestfulFileUpload
	{
		public RestFullFileUpload ()
		{
		}

		public string SendFileToRest(byte[] file, string fileName, string token, string url, string resource, Dictionary<string, string> data)
		{
			try 
			{
				string boundary = "---------------------------" + DateTime.Now.Ticks.ToString ("x");
				
				var client = new RestClient (url);
				
				var request = new RestRequest (resource, Method.POST);
				request.AddHeader ("authorization", string.Format ("Bearer {0}", token));
				request.AddFile ("arquivo", file, fileName, "image/jpeg");

				string header = string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n", "arquivo", fileName, "image/jpeg");
				request.AddParameter("multipart/form-data;",  "boundary=" + boundary,header,ParameterType.RequestBody);

				var result = client.Execute(request);

				if(result.StatusDescription=="OK"){
					return result.Content;
				}
				else{
					return "";
				}
			}
			catch (Exception ex) 
			{
				throw new Exception (string.Format("{0} : {1}", "SendFileToRest", ex.Message));

			}
		}

	}
}

