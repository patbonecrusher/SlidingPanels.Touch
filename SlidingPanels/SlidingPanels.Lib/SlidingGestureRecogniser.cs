// Copyright (C) 2013 Pat Laplante & Frank Caico
//
//	Permission is hereby granted, free of charge, to  any person obtaining a copy 
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without  restriction, including without limitation the rights 
// to use, copy,  modify,  merge, publish,  distribute,  sublicense, and/or sell 
// copies of the  Software,  and  to  permit  persons  to   whom the Software is 
// furnished to do so, subject to the following conditions:
//
//		The above  copyright notice  and this permission notice shall be included 
//     in all copies or substantial portions of the Software.
//
//		THE  SOFTWARE  IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
//     OR   IMPLIED,   INCLUDING  BUT   NOT  LIMITED   TO   THE   WARRANTIES  OF 
//     MERCHANTABILITY,  FITNESS  FOR  A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
//     IN NO EVENT SHALL  THE AUTHORS  OR COPYRIGHT  HOLDERS  BE  LIABLE FOR ANY 
//     CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT 
//     OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION  WITH THE SOFTWARE OR 
//     THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// -----------------------------------------------------------------------------

using System;
using UIKit;
using System.Collections.Generic;
using SlidingPanels.Lib.PanelContainers;
using System.Linq;
using CoreGraphics;

namespace SlidingPanels.Lib
{
	/// <summary>
	/// Sliding Panels gesture recogniser.
	/// </summary>
	public class SlidingGestureRecogniser : UIPanGestureRecognizer
	{
		#region Data Members

		/// <summary>
		/// The list of panels that need to be monitored for gestures
		/// </summary>
		private List<PanelContainer> _panelContainers;

		#endregion

		#region Properties

		/// <summary>
		/// The currently showing panel
		/// </summary>
		/// <value>The current active panel container.</value>
		protected PanelContainer CurrentActivePanelContainer 
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the sliding controller.
		/// </summary>
		/// <value>The sliding controller.</value>
		public UIViewController SlidingController 
		{
			get;
			private set;
		}

		/// <summary>
		/// Occurs when a sliding panel should be shown
		/// </summary>
		public event EventHandler ShowPanel;

		/// <summary>
		/// Occurs when a sliding panel should be hidden
		/// </summary>
		public event EventHandler HidePanel;

		#endregion

		#region Construction / Destruction

		/// <summary>
		/// Initializes a new instance of the <see cref="SlidingPanels.Lib.SlidingGestureRecogniser"/> class.
		/// </summary>
		/// <param name="panelContainers">List of Panel Containers to monitor for gestures</param>
		/// <param name="shouldReceiveTouch">Indicates that touch events should be monitored</param>
		/// <param name="slidingController">The Sliding Panels controller</param>
		public SlidingGestureRecogniser (List<PanelContainer> panelContainers, UITouchEventArgs shouldReceiveTouch, UIViewController slidingController, UIView contentView)
		{
			SlidingController = slidingController;
			_panelContainers = panelContainers;

			this.ShouldReceiveTouch += (sender, touch) => {
				if (SlidingController == null) 
				{ 
					return false; 
				}

				if (touch.View is UIButton) 
				{ 
					return false; 
				}

				bool validTouch = false;
				UIView touchView = touch.View;
				while (touchView != null)
				{
					if (touchView == contentView)
					{
						validTouch = true;
						break;
					}
					touchView = touchView.Superview;
				}
				if (!validTouch)
				{
					return false;
				}

				return shouldReceiveTouch(sender, touch);
			};
		}

		#endregion

		#region Touch Methods

		/// <summary>
		/// We want to prevent any other gesture to be recognized on the window!
		/// </summary>
		/// <param name="preventingGestureRecognizer">Preventing gesture recognizer.</param>
		public override bool CanBePreventedByGestureRecognizer (UIGestureRecognizer preventingGestureRecognizer)
		{
			return State != UIGestureRecognizerState.Began;
		}

		/// <summary>
		/// Manages what happens when the user begins a possible slide 
		/// </summary>
		/// <param name="touches">Touches.</param>
		/// <param name="evt">Evt.</param>
		public override void TouchesBegan (Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesBegan (touches, evt);

			CGPoint touchPt;
			UITouch touch = touches.AnyObject as UITouch;
			if (touch != null) 
			{
				touchPt = touch.LocationInView (this.View);
			}
			else
			{
				State = UIGestureRecognizerState.Failed;
				return;
			}

			CurrentActivePanelContainer = _panelContainers.FirstOrDefault (p => p.IsVisible);
			if (CurrentActivePanelContainer == null) 
			{
				CurrentActivePanelContainer = _panelContainers.FirstOrDefault (p => p.CanStartSliding (touchPt, SlidingController.View.Frame));
				if (CurrentActivePanelContainer != null) 
				{
					CurrentActivePanelContainer.Show ();
					CurrentActivePanelContainer.SlidingStarted (touchPt, SlidingController.View.Frame);
				}
				else
				{
					State = UIGestureRecognizerState.Failed;
				}
			} 
			else 
			{
				CurrentActivePanelContainer.SlidingStarted (touchPt, SlidingController.View.Frame);
			}
		}

		/// <summary>
		/// Manages what happens while the user is mid-slide
		/// </summary>
		/// <param name="touches">Touches.</param>
		/// <param name="evt">Evt.</param>
		public override void TouchesMoved (Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesMoved (touches, evt);

			if (CurrentActivePanelContainer == null)
			{
				return;
			}

			CGPoint touchPt;
			UITouch touch = touches.AnyObject as UITouch;
			if (touch != null) 
			{
				touchPt = touch.LocationInView (this.View);
			}
			else
			{
				return;
			}

			CGRect newFrame = CurrentActivePanelContainer.Sliding (touchPt, SlidingController.View.Frame);
			SlidingController.View.Frame = newFrame;
		}

		/// <summary>
		/// Manages what happens when the user completes a slide
		/// </summary>
		/// <param name="touches">Touches.</param>
		/// <param name="evt">Evt.</param>
		public override void TouchesEnded (Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesEnded (touches, evt);

			if (CurrentActivePanelContainer == null)
			{
				return;
			}

			CGPoint touchPt;
			UITouch touch = touches.AnyObject as UITouch;
			if (touch != null) 
			{
				touchPt = touch.LocationInView (this.View);
			}
			else
			{
				return;
			}

			if (CurrentActivePanelContainer.SlidingEnded (touchPt, SlidingController.View.Frame)) 
			{
				if (ShowPanel != null) 
				{
					ShowPanel (this, new SlidingGestureEventArgs (CurrentActivePanelContainer));
				}
			} 
			else 
			{
				if (HidePanel != null) 
				{
					HidePanel (this, new SlidingGestureEventArgs (CurrentActivePanelContainer));
				}
			}
		}

		/// <summary>
		/// Manages what happens when a slide is interrupted
		/// </summary>
		/// <param name="touches">Touches.</param>
		/// <param name="evt">Evt.</param>
		public override void TouchesCancelled (Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesCancelled (touches, evt);
		}

		#endregion
	}
}

