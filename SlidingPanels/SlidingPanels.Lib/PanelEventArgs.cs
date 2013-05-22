using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using SlidingPanels.Lib.PanelContainers;
using System.Drawing;

namespace SlidingPanels.Lib
{
	public class PanelEventArgs : EventArgs
	{
		public UIViewController Panel
		{
			get;
			private set;
		}

		public PanelEventArgs(UIViewController panel)
		{
			Panel = panel;
		}
	}
}
