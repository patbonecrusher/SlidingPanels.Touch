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
			View.Frame = frame;

			View.BackgroundColor = UIColor.Blue;
			AddChildViewController (PanelVC);
			View.AddSubview (PanelVC.View);

			frame = PanelVC.View.Frame;
			frame.Y = 0;
			PanelVC.View.Frame = frame;

			Hide ();
		}

		public void Toggle ()
		{
			Console.WriteLine ("Toggling");
			if (View.Hidden) {
				Show ();
			} else {
				Hide ();
			}
		}

		public void Show ()
		{
			Console.WriteLine ("Showing");
			Panel.RefreshContent ();
			View.Hidden = false;
		}

		public void Hide ()
		{
			Console.WriteLine ("Hiding");
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

