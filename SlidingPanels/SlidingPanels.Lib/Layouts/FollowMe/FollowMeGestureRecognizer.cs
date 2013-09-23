// /// Copyright (C) 2013 Pat Laplante & Franc Caico
// ///
// ///	Permission is hereby granted, free of charge, to  any person obtaining a copy 
// /// of this software and associated documentation files (the "Software"), to deal 
// /// in the Software without  restriction, including without limitation the rights 
// /// to use, copy,  modify,  merge, publish,  distribute,  sublicense, and/or sell 
// /// copies of the  Software,  and  to  permit  persons  to   whom the Software is 
// /// furnished to do so, subject to the following conditions:
// ///
// ///		The above  copyright notice  and this permission notice shall be included 
// ///     in all copies or substantial portions of the Software.
// ///
// ///		THE  SOFTWARE  IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// ///     OR   IMPLIED,   INCLUDING  BUT   NOT  LIMITED   TO   THE   WARRANTIES  OF 
// ///     MERCHANTABILITY,  FITNESS  FOR  A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// ///     IN NO EVENT SHALL  THE AUTHORS  OR COPYRIGHT  HOLDERS  BE  LIABLE FOR ANY 
// ///     CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT 
// ///     OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION  WITH THE SOFTWARE OR 
// ///     THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// /// -----------------------------------------------------------------------------
//
using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace SlidingPanels.Lib.Layouts.FollowMe
{
	public class FollowMePanningEventArgs : EventArgs
	{
		private int _deltaFromStart;
		public FollowMePanningEventArgs(int deltaFromStart)
		{
			_deltaFromStart = deltaFromStart;
		}

		public int DeltaFromStart
		{
			get
			{
				return _deltaFromStart;
			}
		}
	}

	public class FollowMeGestureRecognizer : UIPanGestureRecognizer
	{
		private PointF _startedLocation;

		public event EventHandler PanningStartedEvent;
		public event EventHandler PanningEvent;
		public event EventHandler PanningEndedEvent;

		public FollowMeGestureRecognizer ()
		{
		}
	
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
		public override void TouchesBegan (MonoTouch.Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesBegan (touches, evt);

			UITouch touch = touches.AnyObject as UITouch;
			if (touch != null) 
			{
				_startedLocation = touch.LocationInView (this.View);

			}
			else
			{
				State = UIGestureRecognizerState.Failed;
				return;
			}
		}

		/// <summary>
		/// Manages what happens while the user is mid-slide
		/// </summary>
		/// <param name="touches">Touches.</param>
		/// <param name="evt">Evt.</param>
		public override void TouchesMoved (MonoTouch.Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesMoved (touches, evt);

			PointF touchPt;
			UITouch touch = touches.AnyObject as UITouch;
			if (touch != null) 
			{
				touchPt = touch.LocationInView (this.View);

				// We only want a left or right swipe.
				if (Math.Abs(_startedLocation.X - touchPt.X) > Math.Abs(_startedLocation.Y-touchPt.Y))
				{
					if (!_gestureStarted)
					{
						if ()
						_gestureStarted = true;
					}
				}
				else
				{
					State = UIGestureRecognizerState.Failed;
				}
			}
			else
			{
				State = UIGestureRecognizerState.Failed;
			}

//			RectangleF newFrame = CurrentActivePanelContainer.Sliding (touchPt, SlidingController.View.Frame);
//			SlidingController.View.Frame = newFrame;
		}

		/// <summary>
		/// Manages what happens when the user completes a slide
		/// </summary>
		/// <param name="touches">Touches.</param>
		/// <param name="evt">Evt.</param>
		public override void TouchesEnded (MonoTouch.Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesEnded (touches, evt);

//			if (CurrentActivePanelContainer == null)
//			{
//				return;
//			}

			PointF touchPt;
			UITouch touch = touches.AnyObject as UITouch;
			if (touch != null) 
			{
				touchPt = touch.LocationInView (this.View);
			}
			else
			{
				return;
			}

//			if (CurrentActivePanelContainer.SlidingEnded (touchPt, SlidingController.View.Frame)) 
//			{
//				if (ShowPanel != null) 
//				{
//					ShowPanel (this, new SlidingGestureEventArgs (CurrentActivePanelContainer));
//				}
//			} 
//			else 
//			{
//				if (HidePanel != null) 
//				{
//					HidePanel (this, new SlidingGestureEventArgs (CurrentActivePanelContainer));
//				}
//			}
		}

		/// <summary>
		/// Manages what happens when a slide is interrupted
		/// </summary>
		/// <param name="touches">Touches.</param>
		/// <param name="evt">Evt.</param>
		public override void TouchesCancelled (MonoTouch.Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesCancelled (touches, evt);
		}

		#endregion

	
	}
}

