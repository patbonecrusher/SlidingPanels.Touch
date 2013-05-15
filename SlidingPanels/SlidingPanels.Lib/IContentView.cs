using System;

namespace SlidingPanels.Lib
{
	public interface IContentView
	{
		event Action<PanelType> ToggleFlyout;
	}
}

