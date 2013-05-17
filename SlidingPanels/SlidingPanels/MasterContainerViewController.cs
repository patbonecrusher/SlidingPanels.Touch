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

			_slidingPanelVC.SetVisibleContentViewController (new UINavigationController(new ExampleContentA()));
		}
	}
}

