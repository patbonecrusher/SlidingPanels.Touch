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
using MonoTouch.UIKit;
using System.Drawing;

namespace SlidingPanels.Lib.PanelContainers
{
    /// <summary>
    /// Base class for Left, Right or Bottom Panel Containers
    /// This is an abstract class and cannot be used directly
    /// </summary>
    public abstract class PanelContainer : UIViewController
    {

        #region Constants

        /// <summary>
        /// Designates the edge tolerance in pts.  This defaults to 40 pts.
        /// </summary>
        private float _edgeTolerance = 40F;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the view controller contained inside this panel
        /// </summary>
        /// <value>The panel V.</value>
        public UIViewController PanelVC { 
            get; 
            private set; 
        }

        /// <summary>
        /// Gets the type of the panel (Left, Right or Bottom)
        /// </summary>
        /// <value>The type of the panel.</value>
        public PanelType PanelType { 
            get; 
            private set; 
        }

        /// <summary>
        /// Gets a value indicating whether the panel is currently showing
        /// </summary>
        /// <value><c>true</c> if this instance is visible; otherwise, <c>false</c>.</value>
        public virtual bool IsVisible { 
            get { 
                return !View.Hidden; 
            } 
        }

        /// <summary>
        /// Gets the size of the panel
        /// </summary>
        /// <value>The size.</value>
        public virtual SizeF Size { 
            get; 
            private set; 
        }

        /// <summary>
        /// Gets the distance from the Left, right or bottom edge of screen 
        /// which is sensitive to sliding gestures (in pts).
        /// </summary>
        /// <value>The edge tolerance.</value>
        public virtual float EdgeTolerance {
            get {
                return _edgeTolerance;
            }
            set {
                _edgeTolerance = value;
            }
        }

        #endregion

        #region Construction / Destruction

        /// <summary>
        /// Initializes a new instance of the <see cref="SlidingPanels.Lib.PanelContainers.PanelContainer"/> class.
        /// </summary>
        /// <param name="panel">Panel.</param>
        /// <param name="panelType">Panel type.</param>
        protected PanelContainer (UIViewController panel, PanelType panelType)
        {
            PanelVC = panel;
            PanelType = panelType;

            Size = panel.View.Frame.Size;
        }

        #endregion

        #region View Lifecycle

        /// <summary>
        /// Called when the panel view is first loaded
        /// </summary>
        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            View.Frame = UIScreen.MainScreen.Bounds;

            AddChildViewController (PanelVC);
            View.AddSubview (PanelVC.View);

            Hide ();
        }

        /// <summary>
        /// Called every time the Panel is about to be shown
        /// </summary>
        /// <param name="animated">If set to <c>true</c> animated.</param>
        public override void ViewWillAppear (bool animated)
        {
            RectangleF frame = UIScreen.MainScreen.Bounds;

            if (InterfaceOrientation != UIInterfaceOrientation.Portrait) {
                frame.Width = UIScreen.MainScreen.Bounds.Height;
                frame.Height = UIScreen.MainScreen.ApplicationFrame.Width;
                frame.X = UIScreen.MainScreen.ApplicationFrame.Y;

                if (UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeLeft) {
                    frame.Y = UIScreen.MainScreen.ApplicationFrame.X;
                } else {
                    frame.Y = UIScreen.MainScreen.Bounds.Width - UIScreen.MainScreen.ApplicationFrame.Width;
                }

            }

            View.Frame = frame;
            PanelVC.ViewWillAppear (animated);
            base.ViewWillAppear (animated);
        }

        /// <summary>
        /// Called every time after the Panel is shown
        /// </summary>
        /// <param name="animated">If set to <c>true</c> animated.</param>
        public override void ViewDidAppear (bool animated)
        {
            PanelVC.ViewDidAppear (animated);
            base.ViewDidAppear (animated);
        }

        /// <summary>
        /// Called whenever the Panel is about to be hidden
        /// </summary>
        /// <param name="animated">If set to <c>true</c> animated.</param>
        public override void ViewWillDisappear (bool animated)
        {
            PanelVC.ViewWillDisappear (animated);
            base.ViewWillDisappear (animated);
        }

        /// <summary>
        /// Called every time after the Panel is hidden
        /// </summary>
        /// <param name="animated">If set to <c>true</c> animated.</param>
        public override void ViewDidDisappear (bool animated)
        {
            PanelVC.ViewDidDisappear (animated);
            base.ViewDidDisappear (animated);
        }

        #endregion

        #region Visibility Control

        /// <summary>
        /// Toggle the visibility of this panel
        /// </summary>
        public virtual void Toggle ()
        {
            if (View.Hidden) {
                Show ();
            } else {
                Hide ();
            }
        }

        /// <summary>
        /// Makes this Panel visible
        /// </summary>
        public virtual void Show ()
        {
            View.Layer.ZPosition = -1;
            View.Hidden = false;
        }

        /// <summary>
        /// Hides this Panel
        /// </summary>
        public virtual void Hide ()
        {
            View.Hidden = true;
        }

        #endregion

        #region Abstract Method definitions

        /// <summary>
        /// Returns a rectangle representing the location and size of the top view 
        /// when this Panel is showing
        /// </summary>
        /// <returns>The top view position when slider is visible.</returns>
        /// <param name="topViewCurrentFrame">Top view current frame.</param>
        public abstract RectangleF GetTopViewPositionWhenSliderIsVisible (RectangleF topViewCurrentFrame);

        /// <summary>
        /// Returns a rectangle representing the location and size of the top view 
        /// when this Panel is hidden
        /// </summary>
        /// <returns>The top view position when slider is visible.</returns>
        /// <param name="topViewCurrentFrame">Top view current frame.</param>
        public abstract RectangleF GetTopViewPositionWhenSliderIsHidden (RectangleF topViewCurrentFrame);

        /// <summary>
        /// Determines whether this instance can start sliding given the touch position and the 
        /// current location/size of the top view. 
        /// Note that touchPosition is in Screen coordinate.
        /// </summary>
        /// <returns><c>true</c> if this instance can start sliding otherwise, <c>false</c>.</returns>
        /// <param name="touchPosition">Touch position.</param>
        /// <param name="topViewCurrentFrame">Top view's current frame.</param>
        public abstract bool CanStartSliding (PointF touchPosition, RectangleF topViewCurrentFrame);

        /// <summary>
        /// Called when sliding has started on this Panel
        /// </summary>
        /// <param name="touchPosition">Touch position.</param>
        /// <param name="topViewCurrentFrame">Top view current frame.</param>
        public abstract void SlidingStarted (PointF touchPosition, RectangleF topViewCurrentFrame);

        /// <summary>
        /// Called while the user is sliding this Panel
        /// </summary>
        /// <param name="touchPosition">Touch position.</param>
        /// <param name="topViewCurrentFrame">Top view current frame.</param>
        public abstract RectangleF Sliding (PointF touchPosition, RectangleF topViewCurrentFrame);

        /// <summary>
        /// Determines if a slide is complete
        /// </summary>
        /// <returns><c>true</c>, if sliding has ended, <c>false</c> otherwise.</returns>
        /// <param name="touchPosition">Touch position.</param>
        /// <param name="topViewCurrentFrame">Top view current frame.</param>
        public abstract bool SlidingEnded (PointF touchPosition, RectangleF topViewCurrentFrame);

        #endregion

    }
}

