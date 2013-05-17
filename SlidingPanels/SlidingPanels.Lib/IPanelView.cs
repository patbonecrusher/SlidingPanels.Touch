using System;
using System.Drawing;
using MonoTouch.UIKit;

namespace SlidingPanels.Lib
{
	public interface IPanelView
	{
		/// <summary>
		/// Panel fire this event when they want to replace the main view with something else.
		/// </summary>
		event EventHandler TopViewSwapped;

		/// <summary>
		/// When requested by the sliding panel controller, panel must refresh their content.
		/// </summary>
		void RefreshContent();

		/// <summary>
		/// Gets the desired size of the panel.
		/// </summary>
		/// <returns>The size.</returns>
		SizeF Size { get; }
	}
}

