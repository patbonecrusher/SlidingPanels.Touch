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
using SlidingPanels.Lib;
using SlidingPanels.Lib.PanelContainers;
using SlidingPanels.Panels;

namespace SlidingPanels
{
	public class MasterContainerViewController : UIViewController
	{
		private SlidingPanelsViewController _slidingPanelVC;

		public MasterContainerViewController ()
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			_slidingPanelVC = new SlidingPanelsViewController ();

			_slidingPanelVC.View.Frame = UIScreen.MainScreen.Bounds;
			AddChildViewController (_slidingPanelVC);
			View.AddSubview (_slidingPanelVC.View);

			//_slidingPanelVC.InsertPanel (PanelType.LeftPanel, new LeftPanelViewController ());
			//_slidingPanelVC.InsertPanel (PanelType.RightPanel, new RightPanelViewController ());

			_slidingPanelVC.InsertPanel (new LeftPanelContainer(new LeftPanelViewController ()));
			_slidingPanelVC.InsertPanel (new RightPanelContainer(new RightPanelViewController ()));
			_slidingPanelVC.InsertPanel (new BottomPanelContainer(new BottomPanelViewController ()));

			//_slidingPanelVC.SetVisibleContentViewController (new ExampleContentA());
			//_slidingPanelVC.SetVisibleContentViewController (new UINavigationController(new ExampleContentA()));
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			NavigationController.NavigationBarHidden = true;
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			_slidingPanelVC.SetVisibleContentViewController (new UINavigationController(new ExampleContentA()));
		}
	}
}

