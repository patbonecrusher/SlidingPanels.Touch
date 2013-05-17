using System;
using SlidingPanels.Lib;
using Cirrious.MvvmCross.Touch.Views;

namespace MvxSlidingPanels.Touch.Views
{
	public class BottomPanelView : MvxViewController, IBottomPanelView
    {
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

        public BottomPanelView ()
        {
        }

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad();
		}
    }
}

