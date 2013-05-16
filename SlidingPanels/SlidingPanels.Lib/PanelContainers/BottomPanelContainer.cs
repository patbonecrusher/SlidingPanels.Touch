using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace SlidingPanels.Lib.PanelContainers
{
	public class BottomPanelContainer : PanelContainer
	{
		public BottomPanelContainer (UIViewController panel) : base(panel, PanelType.BottomPanel)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// First adjust for the stupid bar at the top.
			RectangleF frame = PanelVC.View.Frame;
			frame.X = 0;
			frame.Height = UIScreen.MainScreen.Bounds.Height;
			frame.Width = View.Frame.Width;
			PanelVC.View.Frame = frame;

			frame.Y = (View.Bounds.Size.Height - Panel.Size.Height);
			frame.Height = Panel.Size.Height;
			PanelVC.View.Frame = frame;
		}

		public override RectangleF GetTopViewPositionWhenSliderIsVisible(RectangleF topViewCurrentFrame)
		{
			topViewCurrentFrame.Y = - (Panel.Size.Height - 40);
			return topViewCurrentFrame;
		}

		public override RectangleF GetTopViewPositionWhenSliderIsHidden(RectangleF topViewCurrentFrame)
		{
			topViewCurrentFrame.Y = 0;
			return topViewCurrentFrame;
		}

		public override bool CanStartPanning(PointF touchPosition, RectangleF topViewCurrentFrame)
		{
			if (!IsVisible)
			{
				return (touchPosition.Y >= View.Bounds.Size.Height-40f && touchPosition.Y <= View.Bounds.Size.Height);
			}
			else
			{
				return topViewCurrentFrame.Contains (touchPosition);
			}
		}

		private float topViewStartYPosition = 0.0f;
		private float touchPositionStartYPosition = 0.0f;

		public override void PanningStarted (PointF touchPosition, RectangleF topViewCurrentFrame)
		{
			touchPositionStartYPosition = touchPosition.Y;
			topViewStartYPosition = topViewCurrentFrame.Y;
		}

		public override RectangleF Panning (PointF touchPosition, RectangleF topViewCurrentFrame)
		{
			float screenHeight = View.Bounds.Size.Height;
			float panelHeight = Panel.Size.Height;
			float topEdge = screenHeight - panelHeight + 40;

			float translation = touchPosition.Y - touchPositionStartYPosition;

			RectangleF frame = topViewCurrentFrame;
			frame.Y = topViewStartYPosition + translation;
			var y = frame.Y + frame.Height;

			if (y >= screenHeight) { frame.Y = 0; }
			if (y <= topEdge) { frame.Y = topEdge - frame.Height; }

			return frame;
		}

		public override bool PanningEnded (PointF touchPosition, RectangleF topViewCurrentFrame)
		{
			float screenHeight = View.Bounds.Size.Height;
			float panelHeight = Panel.Size.Height;

			RectangleF frame = topViewCurrentFrame;
			float y = frame.Y + frame.Height;
			if (y < (screenHeight - (panelHeight/2))) {
				return true;
			} else {
				return false;
			}
		}
	}
}

