using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Cirrious.MvvmCross.Touch.Views;
using Cirrious.MvvmCross.Binding.BindingContext;
using MvxSlidingPanelsSample.Core.ViewModels;
using SlidingPanels.Lib;

namespace MvxSlidingPanels.Touch.Views
{
	public partial class FirstView : MvxViewController, IContentView
    {
        static bool UserInterfaceIdiomIsPhone
        {
            get
            {
                return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone;
            }
        }

        public FirstView ()
            : base (UserInterfaceIdiomIsPhone ? "FirstView_iPhone" : "FirstView_iPad", null)
        {
        }

        public override void DidReceiveMemoryWarning ()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();
            
            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad();
            
            // Perform any additional setup after loading the view, typically from a nib.
			var set = this.CreateBindingSet<FirstView, FirstViewModel>();
			set.Bind(DisplayText).To(vm => vm.DisplayName);
			set.Apply();
        }

		public event Action<PanelType> ToggleFlyout;
    }
}

