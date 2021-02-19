using System;

namespace TaVazando
{
	public interface IFile
	{
		void SavePictureFile (string fileName, Media.Plugin.Abstractions.MediaFile file);
		System.IO.Stream LoadPictureFile (string fileName);
	}
}
