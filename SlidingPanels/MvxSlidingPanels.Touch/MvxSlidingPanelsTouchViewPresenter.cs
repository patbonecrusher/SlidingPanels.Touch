using System;
using Cirrious.MvvmCross.Touch.Views.Presenters;
using MonoTouch.UIKit;
using SlidingPanels.Lib;
using SlidingPanels.Lib.PanelContainers;
using Cirrious.MvvmCross.Touch.Views;
using MvxSlidingPanelsSample.Core.ViewModels;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.CrossCore.Exceptions;

namespace MvxSlidingPanels.Touch
{
	public class MvxSlidingPanelsTouchViewPresenter : MvxTouchViewPresenter
	{

		public override UINavigationController MasterNavigationController {
			get {
				return NavController;
			}
		}

		public SlidingPanelsNavigationViewController NavController {
			get;
			private set;
		}

		public UIViewController RootController {
			get;
			private set;
		}

		public MvxSlidingPanelsTouchViewPresenter(UIApplicationDelegate applicationDelegate, UIWindow window) :
			base(applicationDelegate, window)
		{
			// specialized construction logic goes here
		}

		public override void ChangePresentation (Cirrious.MvvmCross.ViewModels.MvxPresentationHint hint)
		{
			base.ChangePresentation(hint);
		}

		protected override void ShowFirstView (UIViewController viewController)
		{
			NavController = new SlidingPanelsNavigationViewController (viewController);
			RootController = new UIViewController ();

			base.SetWindowRootViewController (RootController);

			RootController.AddChildViewController (NavController);
			RootController.View.AddSubview (NavController.View);

			AddPanel<LeftPanelViewModel>(PanelType.LeftPanel, viewController as MvxViewController);
			AddPanel<RightPanelViewModel>(PanelType.RightPanel, viewController as MvxViewController);
			AddPanel<BottomPanelViewModel>(PanelType.BottomPanel, viewController as MvxViewController);
		}

		protected void AddPanel<T>(PanelType panelType, MvxViewController mvxController) where T : MvxViewModel
		{
			UIViewController viewToAdd = (UIViewController) mvxController.CreateViewControllerFor<T>();

			switch (panelType)
			{
				case PanelType.LeftPanel:
				NavController.InsertPanel(new LeftPanelContainer(viewToAdd));
				break;

				case PanelType.RightPanel:
				NavController.InsertPanel(new RightPanelContainer(viewToAdd));
				break;

				case PanelType.BottomPanel:
				NavController.InsertPanel(new BottomPanelContainer(viewToAdd));
				break;

				default:
				throw new Exception("blah!");
			};
		}

		public override void Show(IMvxTouchView view)
		{
			var viewController = view as UIViewController;
			if (viewController == null)
				throw new MvxException("Passed in IMvxTouchView is not a UIViewController");

			if (MasterNavigationController == null)
				ShowFirstView(viewController);
			else
				MasterNavigationController.PushViewController(viewController, true /*animated*/);
		}
	}
}

