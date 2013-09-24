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
using MonoTouch.CoreGraphics;

namespace SlidingPanels.Lib.Layouts.FollowMe
{
	public class FollowMeLayout : Layout
	{
		private UIPanGestureRecognizer _panRecognizer;
		private UIPinchGestureRecognizer _pinchRecognizer;

		private Containers.Container _masterContainer;
		private Containers.Container _leftContainer;
		private Containers.Container _rightContainer;

		public Containers.Container MasterContainer
		{
			get
			{
				return _masterContainer;
			}
			set
			{
				_masterContainer = value;
				AddPanelContainer (_masterContainer);
			}
		}

		public Containers.Container LeftContainer
		{
			get
			{
				return _leftContainer;
			}
			set
			{
				_leftContainer = value;
				AddPanelContainer (_leftContainer);
			}
		}

		public Containers.Container RightContainer
		{
			get
			{
				return _rightContainer;
			}
			set
			{
				_rightContainer = value;
				AddPanelContainer (_rightContainer);
			}
		}

		public FollowMeLayout ()
		{
			_panRecognizer = new UIPanGestureRecognizer (PanPanel);
			_panRecognizer.MaximumNumberOfTouches = 1;
			_panRecognizer.Delegate = new GestureDelegate (this);

			_pinchRecognizer = new UIPinchGestureRecognizer (PinchPanel);
		}

		public override void InsertPanelsIntoParentView (UIView parent)
		{
			base.InsertPanelsIntoParentView (parent);
		}

		public override void AddPanelContainer (Containers.Container panelContainer)
		{
			base.AddPanelContainer (panelContainer);
		}

		protected override void InsertPanelIntoParentView (SlidingPanels.Lib.Containers.Container container, UIView parent)
		{
			if (ParentView != null)
			{
				RectangleF newPosition = new RectangleF();

				if (container == _masterContainer) {
					MasterContainer.Content.View.AddGestureRecognizer (_panRecognizer);
					MasterContainer.Content.View.AddGestureRecognizer (_pinchRecognizer);
					newPosition.Location = new PointF (
						_leftContainer.Constraints.Size.Width,
						ParentView.Bounds.Location.Y);
					newPosition.Size = new SizeF (
						ParentView.Bounds.Size.Width - _leftContainer.Constraints.Size.Width,
						ParentView.Bounds.Size.Height);
				} else if (container == _leftContainer) {
					newPosition.Location = new PointF (
						0,
						ParentView.Bounds.Location.Y);
					newPosition.Size = new SizeF (
						_leftContainer.Constraints.Size.Width,
						ParentView.Bounds.Size.Height);
				} else if (container == _rightContainer) {
					newPosition.Location = new PointF (
						ParentView.Bounds.Size.Width,
						ParentView.Bounds.Location.Y);
					newPosition.Size = new SizeF (
						_rightContainer.Constraints.Size.Width,
						ParentView.Bounds.Size.Height);
				}

				container.Content.View.Frame = newPosition;
				ParentView.AddSubview (container.Content.View);
				container.Content.View.Hidden = false;
			}
		}

		public override void ShowPanel (int panelID)
		{
			RectangleF leftPanelPosition = LeftContainer.Content.View.Frame;
			RectangleF masterPanelPosition = MasterContainer.Content.View.Frame;
			RectangleF rightPanelPosition = RightContainer.Content.View.Frame;

			if (panelID == _leftContainer.PanelID) {
				leftPanelPosition.Location = new PointF (
					0,
					ParentView.Bounds.Location.Y);
				masterPanelPosition.Location = new PointF (
					_leftContainer.Constraints.Size.Width,
					ParentView.Bounds.Location.Y);
				rightPanelPosition.Location = new PointF (
					_leftContainer.Constraints.Size.Width + _masterContainer.Content.View.Frame.Width,
					ParentView.Bounds.Location.Y);
			} else if (panelID == _rightContainer.PanelID) {
				rightPanelPosition.Location = new PointF (
					ParentView.Bounds.Width - RightContainer.Constraints.Size.Width,
					ParentView.Bounds.Location.Y);
				masterPanelPosition.Location = new PointF (
					rightPanelPosition.Location.X - MasterContainer.Content.View.Frame.Width,
					ParentView.Bounds.Location.Y);
				leftPanelPosition.Location = new PointF (
					masterPanelPosition.Location.X - _leftContainer.Constraints.Size.Width,
					ParentView.Bounds.Location.Y);
			} else if (panelID == _masterContainer.PanelID && !ZoomedOut) {
				masterPanelPosition.Location = new PointF (
					0,
					ParentView.Bounds.Location.Y);
				leftPanelPosition.Location = new PointF (
					masterPanelPosition.Location.X - _leftContainer.Constraints.Size.Width,
					ParentView.Bounds.Location.Y);
				rightPanelPosition.Location = new PointF (
					_masterContainer.Content.View.Frame.Width,
					ParentView.Bounds.Location.Y);
			}

			UIView.Animate(0.25, 0, UIViewAnimationOptions.CurveEaseInOut,
			    delegate {
				LeftContainer.Content.View.Frame = leftPanelPosition;
				MasterContainer.Content.View.Frame = masterPanelPosition;
				RightContainer.Content.View.Frame = rightPanelPosition;
			},
			delegate {
			});
		}

		SizeF MasterOriginalSize;
		PointF MasterOriginalPosition;
		bool ZoomedOut = true;

		void PinchPanel(UIPinchGestureRecognizer gestureRecognizer)
		{
			Console.WriteLine ("Scale {0}", gestureRecognizer.Scale);
			if (ZoomedOut && gestureRecognizer.Scale > 1)
			{
				MasterOriginalSize = MasterContainer.Content.View.Frame.Size;
				MasterOriginalPosition = MasterContainer.Content.View.Frame.Location;

				RectangleF leftPanelRect = LeftContainer.Content.View.Frame;
				RectangleF rightPanelRect = RightContainer.Content.View.Frame;
				RectangleF masterPanelRect = MasterContainer.Content.View.Frame;

				masterPanelRect = ParentView.Bounds;
				PointF center = MasterContainer.Content.View.Center;

				bool stretchingLeft = MasterContainer.Content.View.Frame.Location.X > 0;
				if (stretchingLeft) {
					leftPanelRect.Location = new PointF (0 - LeftContainer.Constraints.Size.Width, leftPanelRect.Location.Y);
					MasterContainer.Content.View.ContentMode = UIViewContentMode.Redraw;
					center = new PointF (ParentView.Bounds.Width, ParentView.Bounds.Height / 2);
				} else {
					rightPanelRect.Location = new PointF (ParentView.Bounds.Width, ParentView.Frame.Y);
					MasterContainer.Content.View.ContentMode = UIViewContentMode.Redraw;
					center = new PointF (0, ParentView.Bounds.Height / 2);
				}

//				MasterContainer.Content.View.Superview.AutosizesSubviews = true;
//				Console.WriteLine ("delta x {0}, delta width {0}", MasterOriginalSize.Width - masterPanelRect.Width, MasterOriginalPosition.X - masterPanelRect.X);
//				UIView.Animate(2.55, 0, UIViewAnimationOptions.CurveEaseInOut,
//				               delegate {
					MasterContainer.Content.View.Frame = masterPanelRect;
					LeftContainer.Content.View.Frame = leftPanelRect;
					RightContainer.Content.View.Frame = rightPanelRect;
//				},
//				delegate {
//					center = MasterContainer.Content.View.Center;
//					masterPanelRect = MasterContainer.Content.View.Bounds;
//				});

				ZoomedOut = false;

				gestureRecognizer.State = UIGestureRecognizerState.Ended;
			} else if (!ZoomedOut && gestureRecognizer.Scale <= 1) {
				RectangleF leftPanelRect = LeftContainer.Content.View.Frame;
				RectangleF rightPanelRect = RightContainer.Content.View.Frame;
				RectangleF masterPanelRect = MasterContainer.Content.View.Frame;

				bool shrinkingRight = MasterContainer.Content.View.Frame.Location.X < 0;
				bool shrinkingLeft = MasterContainer.Content.View.Frame.Location.X > 0;
				if (shrinkingRight) {
					leftPanelRect.Location = new PointF (0 - LeftContainer.Constraints.Size.Width, leftPanelRect.Location.Y);
					masterPanelRect.Location = new PointF (0, masterPanelRect.Location.Y);
				} else if (shrinkingLeft) {
					rightPanelRect.Location = new PointF (ParentView.Bounds.Width - RightContainer.Constraints.Size.Width, ParentView.Frame.Y);
				} else {
					if (MasterOriginalPosition.X > 0) {
						leftPanelRect.Location = new PointF (0, leftPanelRect.Location.Y);
						masterPanelRect.Location = new PointF (LeftContainer.Constraints.Size.Width, masterPanelRect.Location.Y);
					} else {
						rightPanelRect.Location = new PointF (ParentView.Bounds.Width - RightContainer.Constraints.Size.Width, ParentView.Frame.Y);
					}
				}

				masterPanelRect.Size = MasterOriginalSize;
//				UIView.Animate(0.05, 0, UIViewAnimationOptions.CurveEaseInOut,
//				               delegate {
					MasterContainer.Content.View.Frame = masterPanelRect;
					LeftContainer.Content.View.Frame = leftPanelRect;
					RightContainer.Content.View.Frame = rightPanelRect;
//				},
//				delegate {
//				});

				ZoomedOut = true;

				gestureRecognizer.State = UIGestureRecognizerState.Ended;
			}
		}

		void PanPanel (UIPanGestureRecognizer gestureRecognizer)
		{
			var image = gestureRecognizer.View;
			Console.WriteLine ("Panning");
			if (gestureRecognizer.State == UIGestureRecognizerState.Began || gestureRecognizer.State == UIGestureRecognizerState.Changed) {
				var translation = gestureRecognizer.TranslationInView (MasterContainer.Content.View);
				Console.WriteLine ("Translation {0}", translation);

				RectangleF masterContainerFrame = MasterContainer.Content.View.Frame;
				RectangleF leftContainerFrame = LeftContainer.Content.View.Frame;
				RectangleF rightContainerFrame = RightContainer.Content.View.Frame;

				float updatedMasterX = (masterContainerFrame.Location.X + translation.X);
				if ((updatedMasterX + MasterContainer.Content.View.Frame.Width) < (ParentView.Bounds.Width-RightContainer.Constraints.Size.Width)) {
					translation.X = 0;
				} else if (updatedMasterX > LeftContainer.Constraints.Size.Width) {
					translation.X -= (updatedMasterX - LeftContainer.Constraints.Size.Width);
				}

				masterContainerFrame.Location = new PointF(masterContainerFrame.Location.X + translation.X, masterContainerFrame.Location.Y);
				//masterContainerFrame.Size = new SizeF(ParentView.Bounds.Width-LeftContainer.)
				leftContainerFrame.Location = new PointF(leftContainerFrame.Location.X + translation.X, leftContainerFrame.Location.Y);
				rightContainerFrame.Location = new PointF(rightContainerFrame.Location.X + translation.X, rightContainerFrame.Location.Y);
				MasterContainer.Content.View.Frame = masterContainerFrame;
				LeftContainer.Content.View.Frame = leftContainerFrame;
				RightContainer.Content.View.Frame = rightContainerFrame;

				//image.Center = new PointF (image.Center.X + translation.X, image.Center.Y + translation.Y);
				// Reset the gesture recognizer's translation to {0, 0} - the next callback will get a delta from the current position.
				gestureRecognizer.SetTranslation (PointF.Empty, image);
			} else if (gestureRecognizer.State == UIGestureRecognizerState.Ended) {
				RectangleF masterContainerFrame = MasterContainer.Content.View.Frame;
				if (ZoomedOut) {
					if (masterContainerFrame.Location.X < LeftContainer.Constraints.Size.Width/2) {
						this.ShowPanel (RightContainer.PanelID);
					} else {
						this.ShowPanel (LeftContainer.PanelID);
					}
				} else {
					if (masterContainerFrame.Location.X < (0 - (_leftContainer.Constraints.Size.Width/2))) {
						this.ShowPanel (RightContainer.PanelID);
					} else if (masterContainerFrame.Location.X > (_rightContainer.Constraints.Size.Width/2)) {
						this.ShowPanel (LeftContainer.PanelID);
					} else {
						this.ShowPanel (MasterContainer.PanelID);
					}
				}
			}
		}
		class GestureDelegate : UIGestureRecognizerDelegate
		{
			FollowMeLayout _layout;

			public GestureDelegate (FollowMeLayout layout)
			{
				this._layout = layout;
			}

			public override bool ShouldReceiveTouch(UIGestureRecognizer aRecogniser, UITouch aTouch)
			{
				return true;
			}

			public override bool ShouldBegin (UIGestureRecognizer recognizer)
			{
				// NOTE: Don't call the base implementation on a Model class
				// see http://docs.xamarin.com/ios/tutorials/Events%2c_Protocols_and_Delegates 
//				throw new NotImplementedException ();
				return true;
			}

		}
	}
}

