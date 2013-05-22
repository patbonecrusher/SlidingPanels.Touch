/// Copyright (C) 2013 Pat Laplante & Franc Caico
///
///	Permission is hereby granted, free of charge, to  any person obtaining a copy 
/// of this software and associated documentation files (the "Software"), to deal 
/// in the Software without  restriction, including without limitation the rights 
/// to use, copy,  modify,  merge, publish,  distribute,  sublicense, and/or sell 
/// copies of the  Software,  and  to  permit  persons  to   whom the Software is 
/// furnished to do so, subject to the following conditions:
///
///		The above  copyright notice  and this permission notice shall be included 
///     in all copies or substantial portions of the Software.
///
///		THE  SOFTWARE  IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
///     OR   IMPLIED,   INCLUDING  BUT   NOT  LIMITED   TO   THE   WARRANTIES  OF 
///     MERCHANTABILITY,  FITNESS  FOR  A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
///     IN NO EVENT SHALL  THE AUTHORS  OR COPYRIGHT  HOLDERS  BE  LIABLE FOR ANY 
///     CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT 
///     OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION  WITH THE SOFTWARE OR 
///     THE USE OR OTHER DEALINGS IN THE SOFTWARE.
/// -----------------------------------------------------------------------------

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
			View.BackgroundColor = UIColor.Red;

			RectangleF frame = PanelVC.View.Bounds;
			frame.Y = View.Bounds.Height - Size.Height;
			frame.Height = Size.Height;
			PanelVC.View.Frame = frame;
		}

		public override void ViewWillAppear (bool animated)
		{
			RectangleF frame = PanelVC.View.Bounds;
			frame.Y = View.Bounds.Height - Size.Height;
			frame.Height = Size.Height;
			PanelVC.View.Frame = frame;
			base.ViewWillAppear (animated);
		}

		public override RectangleF GetTopViewPositionWhenSliderIsVisible(RectangleF topViewCurrentFrame)
		{
			topViewCurrentFrame.Y = UIScreen.MainScreen.Bounds.Height - topViewCurrentFrame.Height - Size.Height;
			return topViewCurrentFrame;
		}

		public override RectangleF GetTopViewPositionWhenSliderIsHidden(RectangleF topViewCurrentFrame)
		{
			topViewCurrentFrame.Y = 0;
			return topViewCurrentFrame;
		}

		public override bool CanStartPanning(PointF touchPosition, RectangleF topViewCurrentFrame)
		{
			// touchPosition is in Screen coordinate.

			float offset = 0;
			touchPosition.Y += offset;

			if (!IsVisible)
			{
				return (touchPosition.Y >= (View.Bounds.Height - 40f) && touchPosition.Y <= View.Bounds.Height);
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
			float translation = touchPosition.Y - touchPositionStartYPosition;

			RectangleF frame = topViewCurrentFrame;
			frame.Y = topViewStartYPosition + translation;

			if (frame.Y >= 0) 
			{ 
				frame.Y = 0; 
			}
			else if (frame.Y <= (UIScreen.MainScreen.Bounds.Height - topViewCurrentFrame.Height - Size.Height))
			{
				frame.Y = UIScreen.MainScreen.Bounds.Height - topViewCurrentFrame.Height - Size.Height;
			}
			return frame;
		}

		public override bool PanningEnded (PointF touchPosition, RectangleF topViewCurrentFrame)
		{
			// touchPosition will be in View coordinate, and will be adjusted to account
			// for the nav bar if visible.

			float screenHeight = topViewCurrentFrame.Height;
			float panelHeight = Size.Height;

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

