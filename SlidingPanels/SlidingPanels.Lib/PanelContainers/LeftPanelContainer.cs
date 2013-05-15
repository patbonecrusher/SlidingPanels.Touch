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
				return (touchPosition.X >= topViewCurrentFrame.X && touchPosition.X <= UIScreen.MainScreen.Bounds.Width);
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
			if (topViewCurrentFrame.X < 0) { topViewCurrentFrame.X = 0; }
			if (topViewCurrentFrame.X > PanelVC.View.Frame.Width) { frame.X = PanelVC.View.Frame.Width; }

			return frame;
		}

		public override bool PanningEnded (PointF touchPosition, RectangleF topViewCurrentFrame)
		{
			RectangleF frame = topViewCurrentFrame;
			if (frame.X > (PanelVC.View.Frame.Width/2)) {
				return true;
			} else {
				return false;
			}
		}
	}
}

