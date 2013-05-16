using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace SlidingPanels.Lib.PanelContainers
{
	public abstract class PanelContainer : UIViewController
	{
		public UIViewController PanelVC { get; private set; }
		public PanelType PanelType { get; private set; }
		public bool IsVisible { get { return !View.Hidden; } }

		protected IPanelView Panel { get { return (IPanelView) PanelVC; } }

		protected PanelContainer (UIViewController panel, PanelType panelType)
		{
			PanelVC = panel;
			PanelType = panelType;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			RectangleF frame = View.Frame;
			frame.Y = 0;
			frame.Height = UIScreen.MainScreen.Bounds.Height;
			View.Frame = frame;

			View.BackgroundColor = UIColor.Blue;

			Hide ();
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			AddChildViewController (PanelVC);
			View.AddSubview (PanelVC.View);
		}

		public void Toggle ()
		{
			if (View.Hidden) {
				Show ();
			} else {
				Hide ();
			}
		}

		public void Show ()
		{
			Panel.RefreshContent ();
			View.Hidden = false;
		}

		public void Hide ()
		{
			View.Hidden = true;
		}

		public abstract RectangleF GetTopViewPositionWhenSliderIsVisible (RectangleF topViewCurrentFrame);
		public abstract RectangleF GetTopViewPositionWhenSliderIsHidden (RectangleF topViewCurrentFrame);

		public abstract bool CanStartPanning (PointF touchPosition, RectangleF topViewCurrentFrame);
		public abstract void PanningStarted (PointF touchPosition, RectangleF topViewCurrentFrame);
		public abstract RectangleF Panning (PointF touchPosition, RectangleF topViewCurrentFrame);
		public abstract bool PanningEnded (PointF touchPosition, RectangleF topViewCurrentFrame);
	}
}

