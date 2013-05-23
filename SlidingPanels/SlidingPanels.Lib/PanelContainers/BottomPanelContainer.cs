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
	public class BottomPanelContainer : PanelContainer
	{
		private float _topViewStartYPosition = 0.0f;
		private float _touchPositionStartYPosition = 0.0f;


		public RectangleF PanelPosition
		{
			get
			{
				return new RectangleF 
				{
					X = 0,
					Y = View.Frame.Height - View.Frame.Y - Size.Height,
					Height = Size.Height,
					Width = View.Bounds.Width
				};
			}
		}

		public BottomPanelContainer (UIViewController panel) : base(panel, PanelType.BottomPanel)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			PanelVC.View.Frame = PanelPosition;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			PanelVC.View.Frame = PanelPosition;
		}

		public override RectangleF GetTopViewPositionWhenSliderIsVisible(RectangleF topViewCurrentFrame)
		{
			topViewCurrentFrame.Y = View.Frame.Height - topViewCurrentFrame.Height - Size.Height;
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
				return (touchPosition.Y >= (View.Bounds.Height - EdgeTolerance) && touchPosition.Y <= View.Bounds.Height);
			}
			else
			{
				return topViewCurrentFrame.Contains (touchPosition);
			}
		}

		public override void PanningStarted (PointF touchPosition, RectangleF topViewCurrentFrame)
		{
			_touchPositionStartYPosition = touchPosition.Y;
			_topViewStartYPosition = topViewCurrentFrame.Y;
		}

		public override RectangleF Panning (PointF touchPosition, RectangleF topViewCurrentFrame)
		{
			float translation = touchPosition.Y - _touchPositionStartYPosition;

			RectangleF frame = topViewCurrentFrame;
			frame.Y = _topViewStartYPosition + translation;

			if (frame.Y >= 0) 
			{ 
				frame.Y = 0; 
			}
			else if (frame.Y <= (View.Bounds.Height - topViewCurrentFrame.Height - Size.Height))
			{
				frame.Y = View.Bounds.Height - topViewCurrentFrame.Height - Size.Height;
			}
			return frame;
		}

		public override bool PanningEnded (PointF touchPosition, RectangleF topViewCurrentFrame)
		{
			// touchPosition will be in View coordinate, and will be adjusted to account
			// for the nav bar if visible.

			float screenHeight = topViewCurrentFrame.Height;
			float panelHeight = Size.Height;

			float y = topViewCurrentFrame.Y + topViewCurrentFrame.Height;
			return (y < (screenHeight - (panelHeight / 2)));
		}
	}
}

