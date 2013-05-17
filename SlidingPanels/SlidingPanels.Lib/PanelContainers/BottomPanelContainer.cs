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


		}

		public virtual void Position()
		{
			RectangleF frame = PanelVC.View.Frame;
			frame.Y = (PanelVC.View.Frame.Height - Panel.Size.Height);
			frame.Height = Panel.Size.Height;
			PanelVC.View.Frame = frame;
		}

		public override RectangleF GetTopViewPositionWhenSliderIsVisible(RectangleF topViewCurrentFrame)
		{
			topViewCurrentFrame.Y = - Panel.Size.Height;
			if (!UIApplication.SharedApplication.StatusBarHidden) {
				topViewCurrentFrame.Y += UIApplication.SharedApplication.StatusBarFrame.Height;
			}
			return topViewCurrentFrame;
		}

		public override RectangleF GetTopViewPositionWhenSliderIsHidden(RectangleF topViewCurrentFrame)
		{
			topViewCurrentFrame.Y = 0;
			if (!UIApplication.SharedApplication.StatusBarHidden) {
				topViewCurrentFrame.Y += UIApplication.SharedApplication.StatusBarFrame.Height;
			}
			return topViewCurrentFrame;
		}

		public override bool CanStartPanning(PointF touchPosition, RectangleF topViewCurrentFrame)
		{
			if (!UIApplication.SharedApplication.StatusBarHidden) {
				touchPosition.Y -= UIApplication.SharedApplication.StatusBarFrame.Height;
			}

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
			float topEdge = screenHeight - panelHeight;

			if (!UIApplication.SharedApplication.StatusBarHidden) {
				topEdge += UIApplication.SharedApplication.StatusBarFrame.Height;
			}

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

