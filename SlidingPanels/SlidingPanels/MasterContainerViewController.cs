using System;
using MonoTouch.UIKit;
using SlidingPanels.Lib;

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

			_slidingPanelVC.InsertPanel (PanelType.LeftPanel, new LeftPanelViewController ());
			_slidingPanelVC.InsertPanel (PanelType.RightPanel, new RightPanelViewController ());

			_slidingPanelVC.SetVisibleContentViewController (new UINavigationController(new ExampleContentA()));
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
		}
	}
}

