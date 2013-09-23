// /// Copyright (C) 2013 Pat Laplante & Franc Caico
// ///
// ///	Permission is hereby granted, free of charge, to  any person obtaining a copy 
// /// of this software and associated documentation files (the "Software"), to deal 
// /// in the Software without  restriction, including without limitation the rights 
// /// to use, copy,  modify,  merge, publish,  distribute,  sublicense, and/or sell 
// /// copies of the  Software,  and  to  permit  persons  to   whom the Software is 
// /// furnished to do so, subject to the following conditions:
// ///
// ///		The above  copyright notice  and this permission notice shall be included 
// ///     in all copies or substantial portions of the Software.
// ///
// ///		THE  SOFTWARE  IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// ///     OR   IMPLIED,   INCLUDING  BUT   NOT  LIMITED   TO   THE   WARRANTIES  OF 
// ///     MERCHANTABILITY,  FITNESS  FOR  A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// ///     IN NO EVENT SHALL  THE AUTHORS  OR COPYRIGHT  HOLDERS  BE  LIABLE FOR ANY 
// ///     CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT 
// ///     OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION  WITH THE SOFTWARE OR 
// ///     THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// /// -----------------------------------------------------------------------------
//
using System;
using MonoTouch.UIKit;
using System.Collections.Generic;

namespace SlidingPanels.Lib
{
	public class SlidingPanelViewController : UIViewController
	{
		#region Data Members

		/// <summary>
		/// This is to work around an issue.  Since the panels are added to the 
		/// parent of this navigation controller (becoming sibling of this), we
		/// need to wait until View.SuperView is not null to start adding them.  
		/// We do that in the first time ViewWillAppear gets called.
		/// </summary>
		private bool _firstTime = true;

		/// <summary>
		/// 
		/// </summary>
		private List<Layouts.Layout> _layouts;

		/// <summary>
		/// The list of panels.
		/// </summary>
		private List<Containers.Container> _panelContainers;

		#endregion

		#region Construction/Destruction

		public SlidingPanelViewController ()
		{
		}

		public void AddLayout(Layouts.Layout layout)
		{
			_layouts.Add (layout);
		}

		#endregion

		#region ViewLifecycle

		/// <summary>
		/// Called when the view is first loaded
		/// </summary>
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			_panelContainers = new List<Containers.Container> ();
			_layouts = new List<Layouts.Layout> ();
		}

		/// <summary>
		/// At this point, it is safe to assume that the Superview is available
		/// for us to insert any panel that may have been added already.
		/// <see cref="_firstTime"/> 
		/// </summary>
		/// <param name="animated">If set to <c>true</c> animated.</param>
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear(animated);

			if (_firstTime)
			{
				foreach(Layouts.Layout layout in _layouts) 
				{
					layout.InsertPanelsIntoParentView (View);
				}

				// NOT SURE ABOUT THIS?
				UIView parent = View.Superview;
				View.RemoveFromSuperview ();
				parent.AddSubview (View);

				_firstTime = false;
			}
		}

		#region overrides to pass to container

		/// <summary>
		/// Called when the view will rotate.
		/// This override forwards the WillRotate callback on to each of the panel containers
		/// </summary>
		/// <param name="toInterfaceOrientation">To interface orientation.</param>
		/// <param name="duration">Duration.</param>
		public override void WillRotate (UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			base.WillRotate (toInterfaceOrientation, duration);
			//_panelContainers.ForEach (c => c.WillRotate (toInterfaceOrientation, duration));
		}

		/// <summary>
		/// Called after the view rotated
		/// This override forwards the DidRotate callback on to each of the panel containers
		/// </summary>
		/// <param name="fromInterfaceOrientation">From interface orientation.</param>
		public override void DidRotate (UIInterfaceOrientation fromInterfaceOrientation)
		{
			base.DidRotate (fromInterfaceOrientation);
			//_panelContainers.ForEach (c => c.DidRotate (fromInterfaceOrientation));
		}

		#endregion
		#endregion

	}
}

