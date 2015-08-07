// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace SlidingPanels
{
	[Register ("ExampleContentA")]
	partial class ExampleContentA
	{
		[Outlet]
		UIKit.UITextView CenterText { get; set; }

		[Outlet]
		UIKit.UIImageView RightArrowImage { get; set; }

		[Outlet]
		UIKit.UIImageView LeftArrowImage { get; set; }

		[Outlet]
		UIKit.UIImageView UpArrowImage { get; set; }

		[Outlet]
		UIKit.UITextView NavigateText { get; set; }

		[Outlet]
		UIKit.UIButton NavigateButton { get; set; }

		[Action ("DoIt:")]
		partial void DoIt (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (CenterText != null) {
				CenterText.Dispose ();
				CenterText = null;
			}

			if (RightArrowImage != null) {
				RightArrowImage.Dispose ();
				RightArrowImage = null;
			}

			if (LeftArrowImage != null) {
				LeftArrowImage.Dispose ();
				LeftArrowImage = null;
			}

			if (UpArrowImage != null) {
				UpArrowImage.Dispose ();
				UpArrowImage = null;
			}

			if (NavigateText != null) {
				NavigateText.Dispose ();
				NavigateText = null;
			}

			if (NavigateButton != null) {
				NavigateButton.Dispose ();
				NavigateButton = null;
			}
		}
	}
}
