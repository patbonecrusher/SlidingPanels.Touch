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
	public abstract class PanelContainer : UIViewController
	{
		public UIViewController PanelVC { get; private set; }
		public PanelType PanelType { get; private set; }
		public bool IsVisible { get { return !View.Hidden; } }

		public IPanelView Panel { get { return (IPanelView) PanelVC; } }

		protected PanelContainer (UIViewController panel, PanelType panelType)
		{
			PanelVC = panel;
			PanelType = panelType;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			View.Frame = View.Bounds;
			View.BackgroundColor = UIColor.Blue;

			Hide ();
		}

		public virtual void Position() 
		{
			RectangleF frame = View.Frame;
			if (!UIApplication.SharedApplication.StatusBarHidden) {
				frame.Y = UIApplication.SharedApplication.StatusBarFrame.Height;
				frame.Height = UIScreen.MainScreen.Bounds.Height - UIApplication.SharedApplication.StatusBarFrame.Height;
			} else {
				frame.Y = 0;
				frame.Height = UIScreen.MainScreen.Bounds.Height;
			}
			View.Frame = frame;

		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			AddChildViewController (PanelVC);
			View.AddSubview (PanelVC.View);
		}

		public void Toggle ()
		{
			if (View.Hidden) {
				Show ();
			} else {
				Hide ();
			}
		}

		public void Show ()
		{
			Panel.RefreshContent ();
			View.Hidden = false;
		}

		public void Hide ()
		{
			View.Hidden = true;
		}

		public abstract RectangleF GetTopViewPositionWhenSliderIsVisible (RectangleF topViewCurrentFrame);
		public abstract RectangleF GetTopViewPositionWhenSliderIsHidden (RectangleF topViewCurrentFrame);

		public abstract bool CanStartPanning (PointF touchPosition, RectangleF topViewCurrentFrame);
		public abstract void PanningStarted (PointF touchPosition, RectangleF topViewCurrentFrame);
		public abstract RectangleF Panning (PointF touchPosition, RectangleF topViewCurrentFrame);
		public abstract bool PanningEnded (PointF touchPosition, RectangleF topViewCurrentFrame);
	}
}

