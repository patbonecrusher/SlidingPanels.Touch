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
using System.Collections.Generic;
using System.Linq;
using MonoTouch.CoreGraphics;
using MonoTouch.UIKit;
using SlidingPanels.Lib.PanelContainers;
using System.Drawing;

namespace SlidingPanels.Lib
{
    /// <summary>
    ///     Sliding Panels Main View Controller
    /// </summary>
    public class SlidingPanelsNavigationViewController : UINavigationController
    {
        #region Constants

        /// <summary>
        ///     How fast do we show/hide panels.
        /// </summary>
        private const float AnimationSpeed = 0.25f;

        #endregion

        #region Data Members

        /// <summary>
        ///     This is to work around an issue.  Since the panels are added to the
        ///     parent of this navigation controller (becoming sibling of this), we
        ///     need to wait until View.SuperView is not null to start adding them.
        ///     We do that in the first time ViewWillAppear gets called.
        /// </summary>
        private bool _firstTime = true;

        /// <summary>
        ///     The list of panels.
        /// </summary>
        private List<PanelContainer> _panelContainers;

        /// <summary>
        ///     The sliding gesture  is  always enabled.  By default it will ignore
        ///     the gesture if started over a button.  One can extend that logic to
        ///     prevent a gesture by providing an Action method.
        ///     <see cref="CanSwipeToShowPanel" />
        /// </summary>
        private SlidingGestureRecogniser _slidingGesture;

        /// <summary>
        ///     The tap gesture is only enabled when  a panel is open.  Tapping the
        ///     visible (slided out) view will trigger the panel to close.
        /// </summary>
        private UITapGestureRecognizer _tapToClose;

        #endregion

        #region Properties

        /// <summary>
        ///     Provides a hook for application to override the decision to allow
        ///     a panel to be swiped in or not.
        /// </summary>
        public Predicate<UITouch> CanSwipeToShowPanel;

        public CGColor ShadowColor { get { return View.Layer.ShadowColor; } set { View.Layer.ShadowColor = value; } }

        public float ShadowOpacity { get { return View.Layer.ShadowOpacity; } set { View.Layer.ShadowOpacity = value; } }

        /// <summary>
        ///     This is a handy Accessor to get the currently active panel, if any.
        /// </summary>
        /// <value>The current active panel container.</value>
        protected PanelContainer CurrentActivePanelContainer
        {
            get { return _panelContainers.FirstOrDefault(p => p.IsVisible); }
        }

        #endregion

        #region Construction/Destruction

        /// <summary>
        ///     Initializes a new instance of the <see cref="SlidingPanels.Lib.SlidingPanelsNavigationViewController" /> class.
        /// </summary>
        /// <param name="controller">First controller to put on the stack.</param>
        public SlidingPanelsNavigationViewController(UIViewController controller) : base(controller)
        {
			if (UIDevice.CurrentDevice.CheckSystemVersion (6, 0)) 
			{
				InteractivePopGestureRecognizer.Enabled = false;
			}

			ShadowColor = UIColor.Black.CGColor;
			ShadowOpacity = .75f;

        }

        #endregion

        #region ViewLifecycle

        /// <summary>
        ///     Called when the view is first loaded
        /// </summary>
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _panelContainers = new List<PanelContainer>();

            _tapToClose = new UITapGestureRecognizer();
            _tapToClose.AddTarget(() => HidePanel(CurrentActivePanelContainer));

			_slidingGesture = new SlidingGestureRecogniser(_panelContainers, ShouldReceiveTouch, this, View);

            _slidingGesture.ShowPanel += (sender, e) => ShowPanel(((SlidingGestureEventArgs) e).PanelContainer);

            _slidingGesture.HidePanel += (sender, e) => HidePanel(((SlidingGestureEventArgs) e).PanelContainer);

			View.ClipsToBounds = true;
			View.Layer.ShadowColor = ShadowColor;
			View.Layer.MasksToBounds = false;
			View.Layer.ShadowOpacity = ShadowOpacity;

			RectangleF shadow = View.Bounds;
			shadow.Inflate(new SizeF(3,3));
			View.Layer.ShadowPath = UIBezierPath.FromRoundedRect(shadow, 0).CGPath;
		}

        /// <summary>
        ///     Called by the SlidingGestureRecogniser everytime a gesture is about
        ///     to be started.  By default, we allow all.  One can override that by
        ///     providing a delegate to the Action CanSwipeToShowPanel.
        /// </summary>
        /// <returns><c>true</c>, if receive touch was shoulded, <c>false</c> otherwise.</returns>
        /// <param name="sender">Sender.</param>
        /// <param name="touch">Touch.</param>
        private bool ShouldReceiveTouch(UIGestureRecognizer sender, UITouch touch)
        {
            if (CanSwipeToShowPanel != null)
            {
                return CanSwipeToShowPanel(touch);
            }
            return true;
        }

        /// <summary>
        ///     At this point, it is safe to assume that the Superview is available
        ///     for us to insert any panel that may have been added already.
        ///     <see cref="_firstTime" />
        /// </summary>
        /// <param name="animated">If set to <c>true</c> animated.</param>
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            if (_firstTime)
            {
                foreach (PanelContainer container in _panelContainers)
                {
                    View.Superview.AddSubview(container.View);
                    View.Superview.AddGestureRecognizer(_slidingGesture);
                }

                UIView parent = View.Superview;
                parent.BringSubviewToFront(View);

                _firstTime = false;
            }
        }

        #region overrides to pass to container

        /// <summary>
        ///     Called when the view will rotate.
        ///     This override forwards the WillRotate callback on to each of the panel containers
        /// </summary>
        /// <param name="toInterfaceOrientation">To interface orientation.</param>
        /// <param name="duration">Duration.</param>
        public override void WillRotate(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillRotate(toInterfaceOrientation, duration);
            _panelContainers.ForEach(c => c.WillRotate(toInterfaceOrientation, duration));
        }

        /// <summary>
        ///     Called after the view rotated
        ///     This override forwards the DidRotate callback on to each of the panel containers
        /// </summary>
        /// <param name="fromInterfaceOrientation">From interface orientation.</param>
        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);
            _panelContainers.ForEach(c => c.DidRotate(fromInterfaceOrientation));
        }

        #endregion

        #endregion

        #region Panel stuff

        /// <summary>
        ///     Return the container associated with the specified panel type.
        /// </summary>
        /// <returns>The container for type.</returns>
        /// <param name="type">Type.</param>
        private PanelContainer ExistingContainerForType(PanelType type)
        {
            PanelContainer container = _panelContainers.FirstOrDefault(p => p.PanelType == type);
            if (container == null)
            {
                throw new ArgumentException("Unknown panel type", "type");
            }
            return container;
        }

        /// <summary>
        ///     Removes the panel.
        /// </summary>
        /// <param name="container">Container.</param>
        public void RemovePanel(PanelContainer container)
        {
            container.View.RemoveFromSuperview();
            container.RemoveFromParentViewController();
            _panelContainers.Remove(container);
        }

        /// <summary>
        ///     Toggles the panel.
        /// </summary>
        /// <param name="type">Type.</param>
        public void TogglePanel(PanelType type)
        {
            PanelContainer container = ExistingContainerForType(type);
            if (container.IsVisible)
            {
                HidePanel(container);
            }
            else
            {
                // Any other panel already up? If so close them.
                if (CurrentActivePanelContainer != null && CurrentActivePanelContainer != container)
                {
                    HidePanel(CurrentActivePanelContainer);
                }

                ShowPanel(container);
            }
        }

        public bool IsPanelVisible(PanelType type)
        {
            return CurrentActivePanelContainer != null && CurrentActivePanelContainer == ExistingContainerForType(type);
        }

        /// <summary>
        ///     Insert a panel in the view hierarchy.  If this is done early in
        ///     the creation process,  we postponed adding  until later, at one
        ///     point we are guarantee that Superview is not null.
        /// </summary>
        /// <param name="container">Container.</param>
        public void InsertPanel(PanelContainer container)
        {
            _panelContainers.Add(container);

            if (!_firstTime)
            {
                UIView parent = View.Superview;
                View.Superview.AddSubview(container.View);
                View.Superview.AddGestureRecognizer(_slidingGesture);
                parent.BringSubviewToFront(View);
            }
        }

        /// <summary>
        ///     Shows the panel.
        /// </summary>
        /// <param name="container">Container.</param>
        public void ShowPanel(PanelContainer container)
        {
            container.ViewWillAppear(true);
            container.Show();

            UIView.Animate(AnimationSpeed, 0, UIViewAnimationOptions.CurveEaseInOut,
                delegate { View.Frame = container.GetTopViewPositionWhenSliderIsVisible(View.Frame); },
                delegate
                {
                    View.AddGestureRecognizer(_tapToClose);
                    container.ViewDidAppear(true);
                });
        }

        /// <summary>
        ///     Hides the panel.
        /// </summary>
        /// <param name="container">Container.</param>
        public void HidePanel(PanelContainer container)
        {
            container.ViewWillDisappear(true);

            UIView.Animate(AnimationSpeed, 0, UIViewAnimationOptions.CurveEaseInOut,
                delegate { View.Frame = container.GetTopViewPositionWhenSliderIsHidden(View.Frame); },
                delegate
                {
                    View.RemoveGestureRecognizer(_tapToClose);
                    container.Hide();
                    container.ViewDidDisappear(true);
                });
        }

        /// <summary>
        ///     Hides the panel.
        /// </summary>
        /// <param name="panelType">Type of the panel to hide.</param>
        public void HidePanel(PanelType panelType)
        {
            PanelContainer container = ExistingContainerForType(panelType);
            if (container != null && container.IsVisible)
            {
                HidePanel(container);
            }
        }

        #endregion
    }
}
