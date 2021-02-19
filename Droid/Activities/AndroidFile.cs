using System;
using System.Runtime.CompilerServices;
using System.IO;
using TaVazando.Droid;
using Xamarin.Forms;
using Java.IO;

[assembly: Xamarin.Forms.Dependency (typeof (AndroidFile))]
namespace TaVazando.Droid
{
	public class AndroidFile : IFile 
	{
		public void SavePictureFile (string filename_, Media.Plugin.Abstractions.MediaFile  file) 
		{
			Stream stream = file.GetStream();

			var path = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			var filename = System.IO.Path.Combine (path, filename_);

			using (var fileStream = System.IO.File.Create (filename)) {
				stream.Seek (0, SeekOrigin.Begin);
				stream.CopyTo (fileStream);
				fileStream.Close ();

				stream.Dispose ();
				fileStream.Dispose ();
				GC.Collect ();
			}
		}

		public System.IO.Stream LoadPictureFile (string filename) 
		{
			var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			var filePath = Path.Combine (documentsPath, filename);

			//Stream stream=null;

			using (Stream fileStream = System.IO.File.OpenRead(filePath)) 
			{
				try
				{
					fileStream.Seek (0, SeekOrigin.Begin);					
					//fileStream.CopyTo(destination:stream);
				}
				catch(Exception ex)
				{
					var x = ex.Message;
				}
				finally
				{
				//	stream.Close();
					fileStream.Close ();
					fileStream.Dispose ();
				//	stream.Dispose();
					GC.Collect ();				
				}

				return fileStream;
			}
		}
	}
}
	