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
		private SlidingPanelsViewController _slidingPanelVC;



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
			_slidingPanelVC = new SlidingPanelsViewController ();

			_slidingPanelVC.View.Frame = UIScreen.MainScreen.Bounds;
			//viewController.View.Frame =  UIScreen.MainScreen.Bounds;
			viewController.AddChildViewController (_slidingPanelVC);
			viewController.View.AddSubview (_slidingPanelVC.View);

			//_slidingPanelVC.InsertPanel (PanelType.LeftPanel, new LeftPanelViewController ());
			//_slidingPanelVC.InsertPanel (PanelType.RightPanel, new RightPanelViewController ());

			/*
			IMvxTouchView vc = viewController as IMvxTouchView;
			if (vc != null)
			{

				_slidingPanelVC.InsertPanel(new RightPanelContainer(vc.CreateViewControllerFor(RightPanelViewModel)));
		        _slidingPanelVC.InsertPanel(new BottomPanelContainer(vc.CreateViewControllerFor(BottomPanelViewModel)));

				_slidingPanelVC.SetVisibleContentViewController (viewController);
			}
			*/
			base.ShowFirstView(viewController);
		}

		protected void AddPanel<T>(PanelType panelType) where T : MvxViewModel
		{
			MvxViewController vc = _slidingPanelVC.ParentViewController as MvxViewController;
			UIViewController viewToAdd = (UIViewController) vc.CreateViewControllerFor<T>();

			if (vc != null)
			{
				switch (panelType)
				{
					case PanelType.LeftPanel:
						_slidingPanelVC.InsertPanel(new LeftPanelContainer(viewToAdd));
						break;

					case PanelType.RightPanel:
						_slidingPanelVC.InsertPanel(new RightPanelContainer(viewToAdd));
						break;

					case PanelType.BottomPanel:
						_slidingPanelVC.InsertPanel(new BottomPanelContainer(viewToAdd));
						break;

					default:
						throw new Exception("blah!");
				};
			}
		}

		protected override void OnMasterNavigationControllerCreated ()
		{
			base.OnMasterNavigationControllerCreated();

			AddPanel<LeftPanelViewModel>(PanelType.LeftPanel);
			AddPanel<RightPanelViewModel>(PanelType.RightPanel);
			AddPanel<BottomPanelViewModel>(PanelType.BottomPanel);

			//ShowViewModel(typeof(FirstViewModel));
			MvxViewController vc = _slidingPanelVC.ParentViewController as MvxViewController;
			UIViewController viewToAdd = (UIViewController) vc.CreateViewControllerFor<FirstViewModel>();

			if (vc != null)
			{
				_slidingPanelVC.SetVisibleContentViewController (viewToAdd);
			}


		}

		public override void Show (Cirrious.MvvmCross.Touch.Views.IMvxTouchView view)
		{
			if (view is IContentView)
			{
				_slidingPanelVC.SetVisibleContentViewController ((UIViewController) view);
			}
			else
			{
				base.Show(view);
			}
		}
    }
}

