using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SlidingPanels.Lib;

namespace SlidingPanels
{
	public partial class ExampleContentB : UIViewController, IContentView
	{
		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public ExampleContentB ()
			: base (UserInterfaceIdiomIsPhone ? "ExampleContentB_iPhone" : "ExampleContentB_iPad", null)
		{
			NavigationItem.LeftBarButtonItem = new UIBarButtonItem (UIBarButtonSystemItem.Action, delegate {
				if (ToggleFlyout != null) {
					ToggleFlyout(PanelType.LeftPanel);
				}
			});
			NavigationItem.RightBarButtonItem = new UIBarButtonItem (UIBarButtonSystemItem.Action, delegate {
				if (ToggleFlyout != null) {
					ToggleFlyout(PanelType.RightPanel);
				}
			});
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Perform any additional setup after loading the view, typically from a nib.
		}

		public event Action<PanelType> ToggleFlyout;

	}
}

