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
using System.Collections.Generic;
using System.Linq;
using MonoTouch.UIKit;
using SlidingPanels.Lib.PanelContainers;

namespace SlidingPanels.Lib
{
	public partial class SlidingPanelsNavigationViewController : UINavigationController
	{
		#region Constants/Properties

		/// <summary>
		/// How fast do we show/hide panels.
		/// </summary>
		const float AnimationSpeed = 0.25f;

		/// <summary>
		/// This is to work around an issue.  Since the panels are added to the 
		/// parent of this navigation controller (becoming sibling of this), we
	 	/// need to wait until View.SuperView is not null to start adding them.  
		/// We do that in the first time ViewWillAppear gets called.
		/// </summary>
		private bool _firstTime = true;

		/// <summary>
		/// The tap gesture is only enabled when  a panel is open.  Tapping the 
		/// visible (slided out) view will trigger the panel to close.
		/// </summary>
		private UITapGestureRecognizer _tapToClose;

		/// <summary>
		/// The sliding gesture  is  always enabled.  By default it will ignore
		/// the gesture if started over a button.  One can extend that logic to
		/// prevent a gesture by providing an Action method.
		/// <see cref="CanSwipeToShowPanel"/> 
		/// </summary>
		private SlidingGestureRecogniser _slidingGesture;

		/// <summary>
		/// The list of panels.
		/// </summary>
		private List<PanelContainer> _panelContainers;

		/// <summary>
		/// This is a handy Accessor to get the currently active panel, if any.
		/// </summary>
		/// <value>The current active panel container.</value>
		protected PanelContainer CurrentActivePanelContainer
		{
			get
			{
				return _panelContainers.FirstOrDefault (p => p.IsVisible);
			}
		}

		/// <summary>
		/// Provides a hook for application to override the decision to allow
		/// a panel to be swiped in or not.
		/// </summary>
		public Predicate<UITouch> CanSwipeToShowPanel;

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="SlidingPanels.Lib.SlidingPanelsNavigationViewController"/> class.
		/// </summary>
		/// <param name="controller">First controller to put on the stack.</param>
		public SlidingPanelsNavigationViewController(UIViewController controller) : base(controller)
		{
		}

		#region ViewLifecycle

		/// <summary>
		/// </summary>
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			_panelContainers = new List<PanelContainer> ();

			View.Layer.ShadowRadius = 5;
			View.Layer.ShadowColor = UIColor.Black.CGColor;
			View.Layer.ShadowOpacity = .75f;
		
			_tapToClose = new UITapGestureRecognizer();
			_tapToClose.AddTarget(() => { 
				HidePanel (CurrentActivePanelContainer); }
			);

			_slidingGesture = new SlidingGestureRecogniser (_panelContainers, ShouldReceiveTouch, this);

			_slidingGesture.ShowPanel += (object sender, EventArgs e) => {
				ShowPanel(((SlidingGestureEventArgs)e).PanelContainer);
			};

			_slidingGesture.HidePanel += (object sender, EventArgs e) => {
				HidePanel(((SlidingGestureEventArgs)e).PanelContainer);
			};
		}

		/// <summary>
		/// Called by the SlidingGestureRecogniser everytime a gesture is about
		/// to be started.  By default, we allow all.  One can override that by
		/// providing a delegate to the Action CanSwipeToShowPanel.
		/// </summary>
		/// <returns><c>true</c>, if receive touch was shoulded, <c>false</c> otherwise.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="touch">Touch.</param>
		bool ShouldReceiveTouch(UIGestureRecognizer sender, UITouch touch)
		{
			if (CanSwipeToShowPanel != null)
			{
				return CanSwipeToShowPanel (touch);
			}
			else
			{
				return true;
			}
		}

		/// <summary>
		/// At this point, it is safe to assume that the Superview is available
		/// for us to insert any panel that may have been added already.
		/// <see cref="_firstTime"/> 
		/// </summary>
		/// <param name="animated">If set to <c>true</c> animated.</param>
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
		}

		#endregion

		#region Panel stuff

		/// <summary>
		/// Return the container associated with the specified panel type.
		/// </summary>
		/// <returns>The container for type.</returns>
		/// <param name="type">Type.</param>
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

		/// <summary>
		/// Insert a panel in the view hierarchy.  If this is done early in
		/// the creation process,  we postponed adding  until later, at one
		/// point we are guarantee that Superview is not null.
		/// </summary>
		/// <param name="container">Container.</param>
		public void InsertPanel(PanelContainer container)
		{
			_panelContainers.Add (container);

			if (!_firstTime)
			{
				UIView parent = View.Superview;
				View.Superview.AddSubview (container.View);
				View.Superview.AddGestureRecognizer (_slidingGesture);
				View.RemoveFromSuperview ();
				parent.AddSubview (View);
			}
		}

		/// <summary>
		/// Shows the panel.
		/// </summary>
		/// <param name="container">Container.</param>
		public void ShowPanel(PanelContainer container)
		{
			container.ViewWillAppear (true);
			container.Show ();

			UIView.Animate(AnimationSpeed, 0, UIViewAnimationOptions.CurveEaseInOut,
			    delegate {
					View.Frame = container.GetTopViewPositionWhenSliderIsVisible(View.Frame);
				},
				delegate {
					View.AddGestureRecognizer(_tapToClose);
					container.ViewDidAppear (true);
				});
		}

		/// <summary>
		/// Hides the panel.
		/// </summary>
		/// <param name="container">Container.</param>
		public void HidePanel(PanelContainer container)
		{
			container.ViewWillDisappear (true);

			UIView.Animate(AnimationSpeed, 0, UIViewAnimationOptions.CurveEaseInOut,
			    delegate {
					View.Frame = container.GetTopViewPositionWhenSliderIsHidden(View.Frame);
				},
				delegate {
					View.RemoveGestureRecognizer(_tapToClose);
					container.Hide ();
					container.ViewDidDisappear (true);
				});
		}
		#endregion	

		#region overrides to pass to container

		public override void WillRotate (UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			base.WillRotate (toInterfaceOrientation, duration);
			_panelContainers.ForEach (c => c.WillRotate (toInterfaceOrientation, duration));
		}

		public override void DidRotate (UIInterfaceOrientation fromInterfaceOrientation)
		{
			base.DidRotate (fromInterfaceOrientation);
			_panelContainers.ForEach (c => c.DidRotate (fromInterfaceOrientation));
		}

		#endregion
	}
}
