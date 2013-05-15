using System;
using MonoTouch.UIKit;
using SlidingPanels.Lib.PanelContainers;
using System.Collections.Generic;
using System.Linq;

namespace SlidingPanels.Lib
{
	public enum PanelType {
		LeftPanel, RightPanel
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
				return _panelContainers.Where (p => p.IsVisible).FirstOrDefault ();
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

			StopListeningForContentEvents ();
			_visibleContentViewController = visibleContentViewController;
			StartListeningForContentEvents ();

			_visibleContentViewController.View.Layer.ShadowRadius = 5;
			_visibleContentViewController.View.Layer.ShadowColor = UIColor.Black.CGColor;
			_visibleContentViewController.View.Layer.ShadowOpacity = .75f;

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

		/// <summary>
		/// Inserts the panel.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="panel">Panel.</param>
		public void InsertPanel(PanelType type, UIViewController panel)
		{
			// Panel must be realizing interface IPanel.
			if (!(panel is IPanelView))
			{
				throw new ArgumentException("panel doesn't realize IPanel", "panel");
			}

			// Eventually switch this to a Factory!
			PanelContainer container = null;
			switch(type)
			{
			case PanelType.LeftPanel:
				container = new LeftPanelContainer (panel);
				break;
			case PanelType.RightPanel:
				container = new RightPanelContainer (panel);
				break;
			default:
				throw new ArgumentException("Unknown panel type", "type");
			}

			// Check to see if we already have a panel of that type and if so
			// remove it before adding the other one.
			PanelContainer existing = _panelContainers.Where (p => p.PanelType == type).FirstOrDefault();
			if (existing != null) 
			{
				this.RemovePanel (existing);
			}

			_panelContainers.Add (container);
			AddChildViewController (container);

			if (_visibleContentViewController != null)
			{
				View.InsertSubviewBelow (container.View, _visibleContentViewController.View);
			}
			else
			{
				View.AddSubview (container.View);
			}
		}

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
		/// <param name="type">Type.</param>
		public void RemovePanel(PanelType type)
		{
			RemovePanel (ExistingContainerForType(type));
		}

		/// <summary>
		/// Removes the panel.
		/// </summary>
		/// <param name="container">Container.</param>
		protected void RemovePanel(PanelContainer container)
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

		/// <summary>
		/// Shows the panel.
		/// </summary>
		/// <param name="type">Type.</param>
		public void ShowPanel(PanelType type)
		{
			PanelContainer container = ExistingContainerForType(type);
			if (!container.IsVisible) 
			{
				// Any other panel already up? If so close them.
				if (CurrentActivePanelContainer != null && CurrentActivePanelContainer != container)
				{
					HidePanel (CurrentActivePanelContainer);
				}

				ShowPanel (container);
			}
			else
			{
				// noop
			}
		}

		/// <summary>
		/// Hides the panel.
		/// </summary>
		/// <param name="type">Type.</param>
		public void HidePanel(PanelType type)
		{
			PanelContainer container = ExistingContainerForType(type);
			if (container.IsVisible) 
			{
				HidePanel (container);
			}
			else
			{
				// noop
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

