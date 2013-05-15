using System;
using MonoTouch.UIKit;
using System.Collections.Generic;
using SlidingPanels.Lib.PanelContainers;
using System.Linq;
using System.Drawing;

namespace SlidingPanels.Lib
{
	public class SlidingGestureEventArgs : EventArgs
	{
		public PanelContainer PanelContainer {
			get;
			private set;
		}

		public SlidingGestureEventArgs(PanelContainer panelContainer)
		{
			PanelContainer = panelContainer;
		}
	}

	public class SlidingGestureRecogniser : UIPanGestureRecognizer
	{
		private List<PanelContainer> PanelContainers;
		protected PanelContainer CurrentActivePanelContainer {
			get;
			set;
		}

		public UIViewController ViewControllerToSwipe {
			get;
			set;
		}

		public event EventHandler ShowPanel;

		public event EventHandler HidePanel;

		public SlidingGestureRecogniser (List<PanelContainer> panelContainers, UITouchEventArgs shouldReceiveTouch)
		{
			PanelContainers = panelContainers;
			this.ShouldReceiveTouch += (sender, touch) => {
				if (ViewControllerToSwipe == null) { return false; }
				if (touch.View is UIButton) { return false; }
				return shouldReceiveTouch(sender, touch);
			};
		}

		public override void TouchesBegan (MonoTouch.Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesBegan (touches, evt);

			PointF touchPt;
			UITouch touch = touches.AnyObject as UITouch;
			if (touch != null) {
				touchPt = touch.LocationInView (this.View);
			}
			else
			{
				return;
			}
		
			CurrentActivePanelContainer = PanelContainers.Where (p => p.IsVisible).FirstOrDefault ();
			if (CurrentActivePanelContainer == null) {
				CurrentActivePanelContainer = PanelContainers.Where (p => p.CanStartPanning(touchPt, ViewControllerToSwipe.View.Frame)).FirstOrDefault ();
				if (CurrentActivePanelContainer != null) {
					CurrentActivePanelContainer.Show ();
					CurrentActivePanelContainer.PanningStarted (touchPt, ViewControllerToSwipe.View.Frame);
				}
			} else {
				CurrentActivePanelContainer.PanningStarted (touchPt, ViewControllerToSwipe.View.Frame);
			}
			Console.WriteLine ("aBegan");
		}

		public override void TouchesMoved (MonoTouch.Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesMoved (touches, evt);

			if (CurrentActivePanelContainer == null)
			{
				return;
			}

			PointF touchPt;
			UITouch touch = touches.AnyObject as UITouch;
			if (touch != null) {
				touchPt = touch.LocationInView (this.View);
			}
			else
			{
				return;
			}

			ViewControllerToSwipe.View.Frame = CurrentActivePanelContainer.Panning (touchPt, ViewControllerToSwipe.View.Frame);
		}

		public override void TouchesEnded (MonoTouch.Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesEnded (touches, evt);

			if (CurrentActivePanelContainer == null)
			{
				return;
			}

			PointF touchPt;
			UITouch touch = touches.AnyObject as UITouch;
			if (touch != null) {
				touchPt = touch.LocationInView (this.View);
			}
			else
			{
				return;
			}

			if (CurrentActivePanelContainer.PanningEnded (touchPt, ViewControllerToSwipe.View.Frame)) {
				if (ShowPanel != null) {
					ShowPanel (this, new SlidingGestureEventArgs (CurrentActivePanelContainer));
				}
			} else {
				if (HidePanel != null) {
					HidePanel (this, new SlidingGestureEventArgs (CurrentActivePanelContainer));
				}
			}
		}

		public override void TouchesCancelled (MonoTouch.Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesCancelled (touches, evt);
		}
	}
}

