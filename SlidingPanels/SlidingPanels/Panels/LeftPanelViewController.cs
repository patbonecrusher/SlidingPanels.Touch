using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SlidingPanels.Lib;

namespace SlidingPanels.Panels
{
	public partial class LeftPanelViewController : UIViewController, IPanelView
	{
		public LeftPanelViewController () : base ("LeftPanelViewController", null)
		{
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

		partial void ShowScreenA (MonoTouch.Foundation.NSObject sender)
		{
			TopViewSwapped(this, new TopViewSwappedEventArgs(new UINavigationController(new ExampleContentA())));
		}

		partial void ShowScreenB (MonoTouch.Foundation.NSObject sender)
		{
			TopViewSwapped(this, new TopViewSwappedEventArgs(new UINavigationController(new ExampleContentB())));
		}

		#region IPanel implementation

		public event EventHandler TopViewSwapped;

		public void RefreshContent ()
		{
		}

		public SizeF Size
		{
			get
			{
				// This panel will appear on the left side.  The associated container doesn't
				// care about the height so we set it to an arbitrary value of -1.
				return new System.Drawing.SizeF (250, -1);
			}
		}		

		#endregion

	}
}

