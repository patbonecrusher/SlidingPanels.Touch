using System;
using MonoTouch.UIKit;
using SlidingPanels.Lib;
using System.Drawing;

namespace SlidingPanels
{
	public class LeftPanelViewController : UIViewController, IPanelView
	{
		public LeftPanelViewController ()
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			View.BackgroundColor = UIColor.Red;
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

