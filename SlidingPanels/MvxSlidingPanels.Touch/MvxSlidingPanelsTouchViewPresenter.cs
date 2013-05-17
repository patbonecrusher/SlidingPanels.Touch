using System;
using Cirrious.MvvmCross.Touch.Views.Presenters;
using MonoTouch.UIKit;
using SlidingPanels.Lib;
using SlidingPanels.Lib.PanelContainers;
using Cirrious.MvvmCross.Touch.Views;
using MvxSlidingPanelsSample.Core.ViewModels;

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

		public override void Show (Cirrious.MvvmCross.Touch.Views.IMvxTouchView view)
		{
			if (view is ILeftPanelView)
			{
				_slidingPanelVC.InsertPanel(new LeftPanelContainer((UIViewController) view));
			}
			else if (view is IRightPanelView)
			{
				_slidingPanelVC.InsertPanel(new RightPanelContainer((UIViewController) view));
			}
			else if (view is IBottomPanelView)
			{
				_slidingPanelVC.InsertPanel(new BottomPanelContainer((UIViewController) view));
			}
			else if (view is IContentView)
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

