using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using SlidingPanels.Lib.PanelContainers;
using System.Drawing;

namespace SlidingPanels.Lib
{
	public partial class SlidingPanelsNavigationViewController : UINavigationController
	{
		public static SlidingPanelsNavigationViewController Instance;

		public SlidingPanelsNavigationViewController(UIViewController controller) : base(controller)
		{
			Instance = this;
		}

		const float AnimationSpeed = 0.25f;

		private UITapGestureRecognizer _tapToClose;
		private SlidingGestureRecogniser _slidingGesture;

		private List<PanelContainer> _panelContainers;
		protected PanelContainer CurrentActivePanelContainer
		{
			get
			{
				return _panelContainers.FirstOrDefault (p => p.IsVisible);
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			_panelContainers = new List<PanelContainer> ();

			View.BackgroundColor = UIColor.Brown;
			_tapToClose = new UITapGestureRecognizer();
			_tapToClose.AddTarget(() => { 
				HidePanel (CurrentActivePanelContainer); }
			);

			_slidingGesture = new SlidingGestureRecogniser (_panelContainers, ShouldReceiveTouch);
			_slidingGesture.ShowPanel += (object sender, EventArgs e) => {
				this.ShowPanel(((SlidingGestureEventArgs)e).PanelContainer);
			};
			_slidingGesture.HidePanel += (object sender, EventArgs e) => {
				this.HidePanel(((SlidingGestureEventArgs)e).PanelContainer);
			};


		}

		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();
		}

		bool ShouldReceiveTouch(UIGestureRecognizer sender, UITouch touch)
		{
			return true;
		}

		#region ContentViewInteraction

		private bool _firstTime = true;


		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			//			View.Superview.AddGestureRecognizer (_slidingGesture);
			_slidingGesture.ViewControllerToSwipe = this;

			View.Layer.ShadowRadius = 5;
			View.Layer.ShadowColor = UIColor.Black.CGColor;
			View.Layer.ShadowOpacity = .75f;
		}

		private void StopListeningForContentEvents()
		{
			if (TopViewController != null)
			{

				// This case can happen if the flyout triggeres a content view change while open.
				TopViewController.View.RemoveGestureRecognizer(_tapToClose);
			}
		}

		private void StartListeningForContentEvents()
		{
			if (TopViewController != null)
			{
			}
		}

		#endregion

		#region PanelInteraction

		private PanelContainer ExistingContainerForType(PanelType type)
		{
			PanelContainer container = null;
			container = _panelContainers.FirstOrDefault (p => p.PanelType == type);
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

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear(animated);

			if (_firstTime)
			{
				foreach(PanelContainer container in _panelContainers)
				{
					View.Superview.AddSubview (container.View);
					View.Superview.AddGestureRecognizer (_slidingGesture);

				}

				UIView parent = View.Superview;
				View.RemoveFromSuperview ();
				parent.AddSubview (View);

				_firstTime = false;
			}
			//			if (_firstTime)
			//			{
			//				_contentController = new UINavigationController (_topController);
			//				TopViewController.AddChildViewController (_contentController);
			//				TopViewController.View.AddSubview (_contentController.View);
			//
			//				RectangleF adjFrame = UIScreen.MainScreen.Bounds;
			//				if (!UIApplication.SharedApplication.StatusBarHidden)
			//				{
			//					adjFrame.Height = UIScreen.MainScreen.ApplicationFrame.Height;
			//				}
			//				_contentController.View.Frame = adjFrame;
			//				_slidingGesture.ViewControllerToSwipe = TopViewController;
			//
			//				_firstTime = false;
			//			}
		}

		public void InsertPanel(PanelContainer container)
		{
			_panelContainers.Add (container);

			container.Panel.TopViewSwapped += (object sender, EventArgs e) => {
				TopViewSwappedEventArgs eventArgs = (TopViewSwappedEventArgs)e;
				//SetVisibleContentViewController(eventArgs.ViewController);
				if (eventArgs.HidePanel && CurrentActivePanelContainer != null) {
					HidePanel (CurrentActivePanelContainer);
				}
			};

			if (!_firstTime)
			{
				UIView parent = View.Superview;
				View.Superview.AddSubview (container.View);
				View.Superview.AddGestureRecognizer (_slidingGesture);
				View.RemoveFromSuperview ();
				parent.AddSubview (View);
			}
		}

		public override void WillChange (NSKeyValueChange changeKind, NSIndexSet indexes, NSString forKey)
		{
			base.WillChange (changeKind, indexes, forKey);
		}

		public void ShowPanel(PanelContainer container)
		{
			container.Show ();

			UIView.Animate(AnimationSpeed, 0, UIViewAnimationOptions.CurveEaseInOut,
			               delegate {
				View.Frame = container.GetTopViewPositionWhenSliderIsVisible(View.Frame);
			},
			delegate {
				View.AddGestureRecognizer(_tapToClose);
			});
		}

		public void HidePanel(PanelContainer container)
		{
			UIView.Animate(AnimationSpeed, 0, UIViewAnimationOptions.CurveEaseInOut,
			               delegate {
				View.Frame = container.GetTopViewPositionWhenSliderIsHidden(View.Frame);
			},
			delegate {
				View.RemoveGestureRecognizer(_tapToClose);
				container.Hide ();
			});
		}
		#endregion	
	}
}
