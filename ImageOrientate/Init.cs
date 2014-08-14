using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ImageOrientate
{
	/// <summary>
	/// Main class for running ImageOrientate from the CLI.
	/// </summary>
	static class Init
	{
		public static void Main(string[] args)
		{
			foreach (string path in args)
			{
				try
				{
					using (Image img = Image.FromFile(path))
					{
						// rotate the image, if necessary
						if (ImageOrientate.Process(img))
						{
							// save the modified image back to its original path
							using (FileStream file = new FileStream(path, FileMode.Create))
							{
								img.Save(file, ImageFormat.Jpeg);
							}
						}
					}
				}
				catch (FileNotFoundException)
				{
					Console.Error.WriteLine(String.Format("Couldn't read the image at {0}.", path));
				}
				catch (Exception e)
				{
					Console.Error.WriteLine(String.Format("Unable to process '{0}': {1}.", path, e.Message));
				}
			}
		}
	}
}
