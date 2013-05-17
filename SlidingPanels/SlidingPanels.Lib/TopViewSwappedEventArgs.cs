using System;
using MonoTouch.UIKit;

namespace SlidingPanels.Lib
{
	public class TopViewSwappedEventArgs : EventArgs
	{
		public UIViewController ViewController {
			get; 
			private set;
		}

		public bool HidePanel {
			get;
			private set;
		}

		public TopViewSwappedEventArgs(UIViewController viewController, bool hidePanel = true)
		{
			ViewController = viewController;
			HidePanel = hidePanel;
		}
	}

}

