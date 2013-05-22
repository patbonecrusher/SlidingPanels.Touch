using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Cirrious.MvvmCross.Touch.Views;
using SlidingPanels.Lib;
using Cirrious.MvvmCross.Binding.BindingContext;
using System.Collections.Generic;

namespace MvxSlidingPanels.Touch.Views
{
	public partial class BottomPanelView : MvxViewController
    {
        static bool UserInterfaceIdiomIsPhone
        {
            get
            {
                return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone;
            }
        }

        public BottomPanelView ()
            : base (UserInterfaceIdiomIsPhone ? "BottomPanelView_iPhone" : "BottomPanelView_iPad", null)
        {
        }

        public override void DidReceiveMemoryWarning ()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();
            
            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad();
            
            // Perform any additional setup after loading the view, typically from a nib.
			this.AddBindings(
				new Dictionary<object, string>()
				{
					{DisplayText, "Text DisplayName"}
				});
        }

		#region IPanelView implementation

		public event EventHandler TopViewSwapped;

		public void RefreshContent ()
		{
		}

		public System.Drawing.SizeF Size
		{
			get
			{
				// This panel will appear on the left side.  The associated container doesn't
				// care about the height so we set it to an arbitrary value of -1.
				return new System.Drawing.SizeF (-1, 150);
			}
		}

		#endregion

    }
}

