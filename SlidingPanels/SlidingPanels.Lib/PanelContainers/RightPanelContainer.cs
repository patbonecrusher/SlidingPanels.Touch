/// Copyright (C) 2013 Pat Laplante & Frank Caico
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
	public class RightPanelContainer : PanelContainer
	{
		private float _topViewStartXPosition = 0.0f;
		private float _touchPositionStartXPosition = 0.0f;

		public RightPanelContainer (UIViewController panel) : base(panel, PanelType.RightPanel)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			PanelVC.View.Frame = new RectangleF 
			{
				X = View.Frame.Width - Size.Width,
				Y = -View.Frame.Y,
				Width = Size.Width,
				Height = View.Frame.Height
			};
		}

		public override RectangleF GetTopViewPositionWhenSliderIsVisible(RectangleF topViewCurrentFrame)
		{
			topViewCurrentFrame.X = - Size.Width;
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
				return (touchPosition.X >= View.Bounds.Size.Width - EdgeTolerance && touchPosition.X <= View.Bounds.Size.Width);
			}
			else
			{
				return topViewCurrentFrame.Contains (touchPosition);
			}
		}

		public override void PanningStarted (PointF touchPosition, RectangleF topViewCurrentFrame)
		{
			_touchPositionStartXPosition = touchPosition.X;
			_topViewStartXPosition = topViewCurrentFrame.X;
		}

		public override RectangleF Panning (PointF touchPosition, RectangleF topViewCurrentFrame)
		{
			float screenWidth = View.Bounds.Size.Width;
			float panelWidth = Size.Width;
			float leftEdge = screenWidth - panelWidth;
			float translation = touchPosition.X - _touchPositionStartXPosition;

			RectangleF frame = topViewCurrentFrame;
			frame.X = _topViewStartXPosition + translation;
			var y = frame.X + frame.Width;

			if (y >= screenWidth) 
			{ 
				frame.X = 0; 
			}

			if (y <= leftEdge) 
			{ 
				frame.X = leftEdge - frame.Width; 
			}

			return frame;
		}

		public override bool PanningEnded (PointF touchPosition, RectangleF topViewCurrentFrame)
		{
			float screenWidth = View.Bounds.Size.Width;
			float panelWidth = Size.Width;

			RectangleF frame = topViewCurrentFrame;
			float y = frame.X + frame.Width;
			if (y < (screenWidth - (panelWidth / 2))) 
			{
				return true;
			} 
			else 
			{
				return false;
			}
		}
	}
}

