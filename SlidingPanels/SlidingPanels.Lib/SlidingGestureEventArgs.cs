using System;
using SlidingPanels.Lib.PanelContainers;

namespace SlidingPanels.Lib
{
	public class SlidingGestureEventArgs : EventArgs
	{
		public PanelContainer PanelContainer {
			get;
			private set;
		}

		public SlidingGestureEventArgs(PanelContainer panelContainer)
		{
			PanelContainer = panelContainer;
		}
	}
}
