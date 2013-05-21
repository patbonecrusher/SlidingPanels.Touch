using System;
using Cirrious.MvvmCross.ViewModels;

namespace MvxSlidingPanelsSample.Core.ViewModels
{
	public class SecondViewModel : BaseViewModel
	{
		private string _centerText;
		public string CenterText
		{
			get
			{
				return _centerText;
			}
			set
			{
				_centerText = value;
				RaisePropertyChanged(() => CenterText);
			}
		}


		public SecondViewModel()
		{
			DisplayName = "Second View";
			CenterText = "This is additional Functionality";
		}
	}
}

