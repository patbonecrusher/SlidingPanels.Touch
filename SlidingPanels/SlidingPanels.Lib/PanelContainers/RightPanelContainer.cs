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

			RectangleF frame = PanelVC.View.Frame;
			frame.Y = 0;
			frame.X = (UIScreen.MainScreen.Bounds.Width - Panel.Size.Width);
			frame.Height = View.Frame.Height;
			frame.Width = Panel.Size.Width;
			PanelVC.View.Frame = frame;
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
				return (touchPosition.X >= View.Bounds.Size.Width-40f && touchPosition.X <= View.Bounds.Size.Width);
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
			float screenWidth = View.Bounds.Size.Width;
			float panelWidth = Panel.Size.Width;
			float leftEdge = screenWidth - panelWidth;

			float translation = touchPosition.X - touchPositionStartXPosition;

			RectangleF frame = topViewCurrentFrame;
			frame.X = topViewStartXPosition + translation;
			var y = frame.X + frame.Width;

			if (y >= screenWidth) { frame.X = 0; }
			if (y <= leftEdge) { frame.X = leftEdge - frame.Width; }

			return frame;
		}

		public override bool PanningEnded (PointF touchPosition, RectangleF topViewCurrentFrame)
		{
			float screenWidth = View.Bounds.Size.Width;
			float panelWidth = Panel.Size.Width;

			RectangleF frame = topViewCurrentFrame;
			float y = frame.X + frame.Width;
			if (y < (screenWidth - (panelWidth/2))) {
				return true;
			} else {
				return false;
			}
		}
	}
}

