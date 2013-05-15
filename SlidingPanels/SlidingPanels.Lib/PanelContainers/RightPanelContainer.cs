using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace SlidingPanels.Lib.PanelContainers
{
	public class RightPanelContainer : PanelContainer
	{
		public RightPanelContainer (UIViewController panel) : base(panel, PanelType.RightPanel)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			RectangleF frame = View.Frame;
			frame.X = (UIScreen.MainScreen.Bounds.Width - Panel.Size.Width);
		}

		public override RectangleF GetTopViewPositionWhenSliderIsVisible(RectangleF topViewCurrentFrame)
		{
			topViewCurrentFrame.X = - Panel.Size.Width;
			return topViewCurrentFrame;
		}

		public override RectangleF GetTopViewPositionWhenSliderIsHidden(RectangleF topViewCurrentFrame)
		{
			topViewCurrentFrame.X = 0;
			return topViewCurrentFrame;
		}

		public override bool CanStartPanning(PointF touchPosition, RectangleF topViewCurrentFrame)
		{
			if (!IsVisible)
			{
				return (touchPosition.X >= UIScreen.MainScreen.Bounds.Width-40f && touchPosition.X <= UIScreen.MainScreen.Bounds.Width);
			}
			else
			{
				return topViewCurrentFrame.Contains (touchPosition);
			}
		}

		private float topViewStartXPosition = 0.0f;
		private float touchPositionStartXPosition = 0.0f;

		public override void PanningStarted (PointF touchPosition, RectangleF topViewCurrentFrame)
		{
			touchPositionStartXPosition = touchPosition.X;
			topViewStartXPosition = topViewCurrentFrame.X;
		}

		public override RectangleF Panning (PointF touchPosition, RectangleF topViewCurrentFrame)
		{
			float translation = touchPosition.X - touchPositionStartXPosition;
			RectangleF frame = topViewCurrentFrame;

			frame.X = topViewStartXPosition + translation;
			var y = frame.X + frame.Width;
			var minY = UIScreen.MainScreen.Bounds.Width - PanelVC.View.Frame.Width;

			if (y >= UIScreen.MainScreen.Bounds.Width) { frame.X = 0; }
			if (y < minY) { frame.X = 0 - PanelVC.View.Frame.Width; }

			return frame;
		}

		public override bool PanningEnded (PointF touchPosition, RectangleF topViewCurrentFrame)
		{
			RectangleF frame = topViewCurrentFrame;
			float y = frame.X + frame.Width;
			if (y < (UIScreen.MainScreen.Bounds.Width - (PanelVC.View.Frame.Width/2))) {
				return true;
			} else {
				return false;
			}
		}
	}
}

