using System;
using Cirrious.MvvmCross.Touch.Views.Presenters;
using MonoTouch.UIKit;
using SlidingPanels.Lib;
using SlidingPanels.Lib.PanelContainers;
using Cirrious.MvvmCross.Touch.Views;
using MvxSlidingPanelsSample.Core.ViewModels;
using Cirrious.MvvmCross.ViewModels;

namespace MvxSlidingPanels.Touch
{
	public class MvxSlidingPanelsTouchViewPresenter : MvxTouchViewPresenter
	{
		public SlidingPanelsNavigationViewController NavController
		{
			get
			{
				return base.MasterNavigationController as SlidingPanelsNavigationViewController;
			}
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
			base.ShowFirstView(viewController);

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

		UIViewController rootController = new UIViewController ();
		protected override UINavigationController CreateNavigationController (UIViewController viewController)
		{
			SlidingPanelsNavigationViewController navController = new SlidingPanelsNavigationViewController (viewController);
			return new SlidingPanelsNavigationViewController (viewController);
		}
	}
}

