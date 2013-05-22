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

		public SizeF Size { get; private set; }

		protected PanelContainer (UIViewController panel, PanelType panelType)
		{
			PanelVC = panel;
			PanelType = panelType;

			Size = panel.View.Frame.Size;
		
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			View.BackgroundColor = UIColor.Brown;
			View.Frame = UIScreen.MainScreen.Bounds;

			AddChildViewController (PanelVC);
			View.AddSubview (PanelVC.View);

			Hide ();
		}

		public override void WillRotate (UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
//			RectangleF newFrame = UIScreen.MainScreen.ApplicationFrame;
//			float tmp = newFrame.Width - newFrame.Y;
//			newFrame.Width = newFrame.Height + newFrame.Y;
//			newFrame.Height = tmp;
//			View.Frame = newFrame;

			//PanelVC.WillRotate (toInterfaceOrientation, duration);
			base.WillRotate (toInterfaceOrientation, duration);

			RectangleF frame = UIScreen.MainScreen.Bounds;
			if (toInterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || 
			    toInterfaceOrientation == UIInterfaceOrientation.LandscapeRight)
			{
				frame.X = UIScreen.MainScreen.Bounds.Y;
				frame.Y = UIScreen.MainScreen.Bounds.X;
				frame.Height = UIScreen.MainScreen.Bounds.Width;
				frame.Width = UIScreen.MainScreen.Bounds.Height;
			}
			View.Frame = frame;
		}

		public override void DidRotate (UIInterfaceOrientation fromInterfaceOrientation)
		{
			//PanelVC.DidRotate (toInterfaceOrientation);
			base.DidRotate (fromInterfaceOrientation);
			RectangleF frame = UIScreen.MainScreen.Bounds;
			if (fromInterfaceOrientation == UIInterfaceOrientation.Portrait)
			{
				frame.X = UIScreen.MainScreen.Bounds.Y;
				frame.Y = UIScreen.MainScreen.Bounds.X;
				frame.Height = UIScreen.MainScreen.Bounds.Width;
				frame.Width = UIScreen.MainScreen.Bounds.Height;
			}
			View.Frame = frame;
		}

		public override void ViewWillAppear (bool animated)
		{
			PanelVC.ViewWillAppear (animated);
			base.ViewWillAppear (animated);
		}

		public override void ViewDidAppear (bool animated)
		{
			PanelVC.ViewDidAppear (animated);
			base.ViewDidAppear (animated);
		}

		public override void ViewWillDisappear (bool animated)
		{
			PanelVC.ViewWillDisappear (animated);
			base.ViewWillDisappear (animated);
		}

		public override void ViewDidDisappear (bool animated)
		{
			PanelVC.ViewDidDisappear (animated);
			base.ViewDidDisappear (animated);
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
			View.Layer.ZPosition = -1;
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

