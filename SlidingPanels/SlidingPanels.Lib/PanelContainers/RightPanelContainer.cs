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
using UIKit;
using System.Drawing;
using CoreGraphics;

namespace SlidingPanels.Lib.PanelContainers
{
	/// <summary>
	/// Container class for Sliding Panels located on the right edge of the device screen
	/// </summary>
	public class RightPanelContainer : PanelContainer
	{
		#region Data Members

		/// <summary>
		/// starting X Coordinate of the top view
		/// </summary>
		private nfloat _topViewStartXPosition = 0.0f;

		/// <summary>
		/// X coordinate where the user touched when starting a slide operation
		/// </summary>
		private nfloat _touchPositionStartXPosition = 0.0f;

		/// <summary>
		/// Gets the panel position.
		/// </summary>
		/// <value>The panel position.</value>
		public CGRect PanelPosition
		{
			get
			{
				return new CGRect 
				{
					X = View.Bounds.Width - Size.Width,
					Y = -View.Frame.Y,
					Width = Size.Width,
					Height = View.Bounds.Height
				};
			}
		}

		#endregion

		#region Construction / Destruction

		/// <summary>
		/// Initializes a new instance of the <see cref="SlidingPanels.Lib.PanelContainers.RightPanelContainer"/> class.
		/// </summary>
		/// <param name="panel">Panel.</param>
		public RightPanelContainer (UIViewController panel) : base(panel, PanelType.RightPanel)
		{
		}

		#endregion

		#region View Lifecycle

		/// <summary>
		/// Called after the Panel is loaded for the first time
		/// </summary>
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			PanelVC.View.Frame = PanelPosition;
		}

		/// <summary>
		/// Called whenever the Panel is about to become visible
		/// </summary>
		/// <param name="animated">If set to <c>true</c> animated.</param>
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			PanelVC.View.Frame = PanelPosition;
		}

		#endregion

		#region Position Methods

		/// <summary>
		/// Returns a rectangle representing the location and size of the top view 
		/// when this Panel is showing
		/// </summary>
		/// <returns>The top view position when slider is visible.</returns>
		/// <param name="topViewCurrentFrame">Top view current frame.</param>
		public override CGRect GetTopViewPositionWhenSliderIsVisible(CGRect topViewCurrentFrame)
		{
			topViewCurrentFrame.X = - Size.Width;
			return topViewCurrentFrame;
		}

		/// <summary>
		/// Returns a rectangle representing the location and size of the top view 
		/// when this Panel is hidden
		/// </summary>
		/// <returns>The top view position when slider is visible.</returns>
		/// <param name="topViewCurrentFrame">Top view current frame.</param>
		public override CGRect GetTopViewPositionWhenSliderIsHidden(CGRect topViewCurrentFrame)
		{
			topViewCurrentFrame.X = 0;
			return topViewCurrentFrame;
		}

		#endregion

		#region Sliding Methods

		/// <summary>
		/// Determines whether this instance can start sliding given the touch position and the 
		/// current location/size of the top view. 
		/// Note that touchPosition is in Screen coordinate.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="touchPosition">Touch position.</param>
		/// <param name="topViewCurrentFrame">Top view's current frame.</param>
		public override bool CanStartSliding(CGPoint touchPosition, CGRect topViewCurrentFrame)
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

		/// <summary>
		/// Called when sliding has started on this Panel
		/// </summary>
		/// <param name="touchPosition">Touch position.</param>
		/// <param name="topViewCurrentFrame">Top view current frame.</param>
		public override void SlidingStarted (CGPoint touchPosition, CGRect topViewCurrentFrame)
		{
			_touchPositionStartXPosition = touchPosition.X;
			_topViewStartXPosition = topViewCurrentFrame.X;
		}

		/// <summary>
		/// Called while the user is sliding this Panel
		/// </summary>
		/// <param name="touchPosition">Touch position.</param>
		/// <param name="topViewCurrentFrame">Top view current frame.</param>
		public override CGRect Sliding (CGPoint touchPosition, CGRect topViewCurrentFrame)
		{
			var screenWidth = View.Bounds.Size.Width;
			var panelWidth = Size.Width;
			var leftEdge = screenWidth - panelWidth;
			var translation = touchPosition.X - _touchPositionStartXPosition;

			CGRect frame = topViewCurrentFrame;

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

		/// <summary>
		/// Determines if a slide is complete
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="touchPosition">Touch position.</param>
		/// <param name="topViewCurrentFrame">Top view current frame.</param>
		public override bool SlidingEnded (CGPoint touchPosition, CGRect topViewCurrentFrame)
		{
			var screenWidth = View.Bounds.Size.Width;
			var panelWidth = Size.Width;

			var y = topViewCurrentFrame.X + topViewCurrentFrame.Width;
			return (y < (screenWidth - (panelWidth / 2)));
		}

		#endregion
	}
}

