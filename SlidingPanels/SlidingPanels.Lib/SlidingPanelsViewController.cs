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
using SlidingPanels.Lib.PanelContainers;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace SlidingPanels.Lib
{
	public enum PanelType {
		LeftPanel, RightPanel, BottomPanel
	}

	public class SlidingPanelsViewController : UIViewController
	{
		const float AnimationSpeed = 0.25f;

		private UITapGestureRecognizer _tapToClose;
		private SlidingGestureRecogniser _slidingGesture;

		private UIViewController _visibleContentViewController;

		private List<PanelContainer> _panelContainers;
		protected PanelContainer CurrentActivePanelContainer
		{
			get
			{
				return _panelContainers.FirstOrDefault (p => p.IsVisible);
			}
		}

		public SlidingPanelsViewController ()
		{
			_panelContainers = new List<PanelContainer> ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			_tapToClose = new UITapGestureRecognizer();
			_tapToClose.AddTarget(() => { HidePanel (CurrentActivePanelContainer); });

			_slidingGesture = new SlidingGestureRecogniser (_panelContainers, ShouldReceiveTouch);
			_slidingGesture.ShowPanel += (object sender, EventArgs e) => {
				this.ShowPanel(((SlidingGestureEventArgs)e).PanelContainer);
			};
			_slidingGesture.HidePanel += (object sender, EventArgs e) => {
				this.HidePanel(((SlidingGestureEventArgs)e).PanelContainer);
			};
			View.AddGestureRecognizer (_slidingGesture);
		}

		bool ShouldReceiveTouch(UIGestureRecognizer sender, UITouch touch)
		{
			return true;
		}

		#region ContentViewInteraction

		private IContentView FindContentViewInstance(UIViewController controller)
		{
			if (controller is IContentView) {
				return controller as IContentView;
			} else {
				IContentView topView = null;
				foreach (UIViewController child in controller.ChildViewControllers)
				{
					topView = FindContentViewInstance (child);
					if (topView != null) {
						break;
					}
				}
				return topView;
			}
		}

		public void SetVisibleContentViewController (UIViewController visibleContentViewController)
		{
			// Panel must be realizing interface IPanel.
			IContentView contentView = FindContentViewInstance(visibleContentViewController);
			if (contentView == null)
			{
				throw new ArgumentException("view doesn't realize IContentView", "visibleContentViewController");
			}

			UIViewController prevVC = _visibleContentViewController;
			StopListeningForContentEvents ();
			_visibleContentViewController = visibleContentViewController;
			StartListeningForContentEvents ();

			_visibleContentViewController.View.Layer.ShadowRadius = 5;
			_visibleContentViewController.View.Layer.ShadowColor = UIColor.Black.CGColor;
			_visibleContentViewController.View.Layer.ShadowOpacity = .75f;

			_visibleContentViewController.View.Frame = View.Frame;
			if (!UIApplication.SharedApplication.StatusBarHidden) {
				RectangleF rect = _visibleContentViewController.View.Frame;
				rect.Y = UIApplication.SharedApplication.StatusBarFrame.Height;
				rect.Height -= UIApplication.SharedApplication.StatusBarFrame.Height;
				_visibleContentViewController.View.Frame = rect;
				_visibleContentViewController.View.BackgroundColor = UIColor.Green;
			}

			if (prevVC != null)
			{
				prevVC.View.RemoveFromSuperview ();
				prevVC.RemoveFromParentViewController ();
			}

			// If we are up and running, we need to swap to this view.
			AddChildViewController (_visibleContentViewController);
			View.AddSubview (_visibleContentViewController.View);

			_slidingGesture.ViewControllerToSwipe = _visibleContentViewController;
		}

		private void StopListeningForContentEvents()
		{
			if (_visibleContentViewController != null)
			{
				IContentView contentView = FindContentViewInstance(_visibleContentViewController);
				if (contentView != null)
				{
					contentView.ToggleFlyout -= TogglePanel;
				}

				// This case can happen if the flyout triggeres a content view change while open.
				_visibleContentViewController.View.RemoveGestureRecognizer(_tapToClose);
			}
		}

		private void StartListeningForContentEvents()
		{
			if (_visibleContentViewController != null)
			{
				IContentView contentView = FindContentViewInstance(_visibleContentViewController);
				if (contentView != null)
				{
					contentView.ToggleFlyout += TogglePanel;
				}
			}
		}

		#endregion

		#region PanelInteraction

		private PanelContainer ExistingContainerForType(PanelType type)
		{
			PanelContainer container = null;
			container = _panelContainers.Where (p => p.PanelType == type).FirstOrDefault();
			if (container == null) 
			{
				throw new ArgumentException("Unknown panel type", "type");
			}
			return container;
		}

		/// <summary>
		/// Removes the panel.
		/// </summary>
		/// <param name="container">Container.</param>
		public void RemovePanel(PanelContainer container)
		{
			container.View.RemoveFromSuperview ();
			container.RemoveFromParentViewController ();
			_panelContainers.Remove (container);
		}

		/// <summary>
		/// Toggles the panel.
		/// </summary>
		/// <param name="type">Type.</param>
		public void TogglePanel(PanelType type)
		{
			PanelContainer container = ExistingContainerForType(type);
			if (container.IsVisible) 
			{
				HidePanel (container);
			}
			else
			{
				// Any other panel already up? If so close them.
				if (CurrentActivePanelContainer != null && CurrentActivePanelContainer != container)
				{
					HidePanel (CurrentActivePanelContainer);
				}

				ShowPanel (container);
			}
		}

		public void InsertPanel(PanelContainer container)
		{
			_panelContainers.Add (container);
			AddChildViewController (container);

			container.Panel.TopViewSwapped += (object sender, EventArgs e) => {
				TopViewSwappedEventArgs eventArgs = (TopViewSwappedEventArgs)e;
				SetVisibleContentViewController(eventArgs.ViewController);
				if (eventArgs.HidePanel && CurrentActivePanelContainer != null) {
					HidePanel (CurrentActivePanelContainer);
				}
			};

			if (_visibleContentViewController != null)
			{
				View.InsertSubviewBelow (container.View, _visibleContentViewController.View);
			}
			else
			{
				View.AddSubview (container.View);
			}
		}

		public void ShowPanel(PanelContainer container)
		{
			container.Show ();

			UIView.Animate(AnimationSpeed, 0, UIViewAnimationOptions.CurveEaseInOut,
				delegate {
					_visibleContentViewController.View.Frame = container.GetTopViewPositionWhenSliderIsVisible(_visibleContentViewController.View.Frame);
				},
				delegate {
					_visibleContentViewController.View.AddGestureRecognizer(_tapToClose);
				});
		}

		public void HidePanel(PanelContainer container)
		{
			UIView.Animate(AnimationSpeed, 0, UIViewAnimationOptions.CurveEaseInOut,
			    delegate {
					_visibleContentViewController.View.Frame = container.GetTopViewPositionWhenSliderIsHidden(_visibleContentViewController.View.Frame);
				},
				delegate {
					_visibleContentViewController.View.RemoveGestureRecognizer(_tapToClose);
					container.Hide ();
				});
		}
		#endregion
	}
}

