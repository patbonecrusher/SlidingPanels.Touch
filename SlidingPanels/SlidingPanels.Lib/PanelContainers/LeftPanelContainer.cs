using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace SlidingPanels.Lib.PanelContainers
{
	public class LeftPanelContainer : PanelContainer
	{
		public LeftPanelContainer (UIViewController panel) : base(panel, PanelType.LeftPanel)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			RectangleF frame = PanelVC.View.Frame;
			frame.Y = 0;
			frame.Height = View.Frame.Height;
			frame.Width = Panel.Size.Width;
			PanelVC.View.Frame = frame;
		}

		public override RectangleF GetTopViewPositionWhenSliderIsVisible(RectangleF topViewCurrentFrame)
		{
			topViewCurrentFrame.X = Panel.Size.Width;
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
				return (touchPosition.X >= 0.0f && touchPosition.X <= 40.0f);
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
			float panelWidth = Panel.Size.Width;

			float translation = touchPosition.X - touchPositionStartXPosition;
			RectangleF frame = topViewCurrentFrame;

			frame.X = topViewStartXPosition + translation;
			if (frame.X <= 0) { frame.X = 0; }
			if (frame.X >= panelWidth) { frame.X = panelWidth; }

			return frame;
		}

		public override bool PanningEnded (PointF touchPosition, RectangleF topViewCurrentFrame)
		{
			float panelWidth = Panel.Size.Width;

			RectangleF frame = topViewCurrentFrame;
			if (frame.X > (panelWidth/2)) {
				return true;
			} else {
				return false;
			}
		}
	}
}

