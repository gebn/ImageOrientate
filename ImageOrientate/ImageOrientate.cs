using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageOrientate
{
	/// <summary>
	/// Physically rotates an image based on its orientation EXIF data.
	/// </summary>
	/// <remarks>
	/// Some browsers pick up this flag (e.g. Chrome), while others don't (e.g. IE).
	/// This class ensures the same image is ultimately displayed, regardless of browser, by rotating the image properly and removing the orientation flag.
	/// </remarks>
	static class ImageOrientate
	{
		/// <summary>
		/// The EXIF property under which orientation data is stored.
		/// </summary>
		const int ORIENTATION_PROPERTY_ID = 0x112;

		/// <summary>
		/// Rotates an image based on its orientation metadata.
		/// </summary>
		/// <remarks>
		/// If the image has no orientation flag, it will be untouched.
		/// </remarks>
		/// <param name="image">The image to rotate.</param>
		/// <returns>Whether the image was altered.</returns>
		public static bool Process(Image image)
		{
			try
			{
				// apply the correct transformation
				image.RotateFlip(GetTransformation(GetOrientation(image)));

				// otherwise supporting browsers will re-apply any rotation we've just done
				image.RemovePropertyItem(ORIENTATION_PROPERTY_ID);

				return true;
			}
			catch (ArgumentException)
			{
				// RemovePropertyItem() didn't like the image; it may still have been altered
				return true;
			}
			catch (FormatException)
			{
				// no rotation necessary
				return false;
			}
			catch (InvalidOperationException)
			{
				// no, or invalid orientation property - do nothing
				return false;
			}
		}

		/// <summary>
		/// Get the transformation that needs to be applied to an image to orientate it properly, given its orientation flag.
		/// </summary>
		/// <param name="orientation">The orientation flag to convert.</param>
		/// <exception cref="FormatException">If no transformation is necessary - i.e. the orientation is TopLeft.</exception>
		/// <returns>The corresponding transformation flag.</returns>
		static RotateFlipType GetTransformation(Orientation orientation)
		{
			if (orientation == Orientation.TopRight) return RotateFlipType.RotateNoneFlipX;
			if (orientation == Orientation.BottomRight) return RotateFlipType.Rotate180FlipNone;
			if (orientation == Orientation.BottomLeft) return RotateFlipType.Rotate180FlipX;
			if (orientation == Orientation.LeftTop) return RotateFlipType.Rotate90FlipX;
			if (orientation == Orientation.RightTop) return RotateFlipType.Rotate90FlipNone;
			if (orientation == Orientation.RightBottom) return RotateFlipType.Rotate270FlipX;
			if (orientation == Orientation.LeftBottom) return RotateFlipType.Rotate270FlipNone;
			throw new FormatException("No transformation necessary");
		}

		/// <summary>
		/// Retrieves the byte representing the position of the camera relative to the scene captured.
		/// </summary>
		/// <param name="image">The image whose orientation to find.</param>
		/// <exception cref="InvalidOperationException">No valid orientation information can be gleaned from the image.</exception>
		/// <returns>The orientation of the image.</returns>
		static Orientation GetOrientation(Image image)
		{
			PropertyItem prop = null;
			try
			{
				prop = image.GetPropertyItem(ORIENTATION_PROPERTY_ID);
			}
			catch (ArgumentException)
			{
				// suggests the image is already the correct orientation
				throw new InvalidOperationException("No orientation property set");
			}

			// the first element represents the orientation
			Orientation orientation = (Orientation)prop.Value[0];

			if (!Enum.IsDefined(typeof(Orientation), orientation))
			{
				// we could throw FormatException to emphasise the point, but we're not the Orientation property police - treat it as if no flag set
				throw new InvalidOperationException("Invalid orientation property set");
			}

			return orientation;
		}

		/// <summary>
		/// Represents all possible orientation property values for an image.
		/// </summary>
		/// <remarks>
		/// See http://www.vb-helper.com/howto_net_read_exif_orientation.html for an explanation of what they mean.
		/// </remarks>
		enum Orientation : byte
		{
			TopLeft = 1,
			TopRight = 2,
			BottomRight = 3,
			BottomLeft = 4,
			LeftTop = 5,
			RightTop = 6,
			RightBottom = 7,
			LeftBottom = 8
		}
	}
}
