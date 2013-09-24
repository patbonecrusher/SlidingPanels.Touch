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
using System.Drawing;
using System.Linq;

namespace SlidingPanels.Lib.Layouts
{
	public class Layout
	{
		private List<Containers.Container> _panelContainers;
		private UIView _parentView;

		public UIView ParentView
		{
			get 
			{
				return _parentView;
			}
		}

		public Layout ()
		{
			_panelContainers = new List<SlidingPanels.Lib.Containers.Container> ();
		}

		public virtual void InsertPanelsIntoParentView(UIView parent)
		{
			_parentView = parent;
			foreach(Containers.Container container in _panelContainers)
			{
				InsertPanelIntoParentView (container, _parentView);
			}
		}

		public virtual void AddPanelContainer(Containers.Container panelContainer)
		{
			if (!_panelContainers.Contains(panelContainer))
			{
				_panelContainers.Add (panelContainer);
				InsertPanelIntoParentView (panelContainer, _parentView);
			}
		}

		public virtual bool ContainsPanel(int panelID) {
			return (_panelContainers.FirstOrDefault (p => p.PanelID == panelID) != null);
		}

		public virtual void ShowPanel (int panelID)
		{
		}

		protected virtual void InsertPanelIntoParentView(Containers.Container container, UIView parent) 
		{
			if (_parentView != null)
			{
				RectangleF newPosition = new RectangleF();
				newPosition.Location = container.Constraints.StartingPosition;
				newPosition.Size = new SizeF (
					container.Constraints.Size.Width,
					_parentView.Frame.Size.Height
					);
				container.Content.View.Frame = newPosition;
				if (_parentView != container.Content.View)
				{
					_parentView.AddSubview (container.Content.View);
				}
				container.Content.View.Hidden = false;
			}
		}
	}
}

