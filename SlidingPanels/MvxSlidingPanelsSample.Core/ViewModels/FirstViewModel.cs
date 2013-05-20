using System;
using Cirrious.MvvmCross.ViewModels;

namespace MvxSlidingPanelsSample.Core.ViewModels
{
    public class FirstViewModel : BaseViewModel
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


		public FirstViewModel()
		{
			DisplayName = "First View";
			CenterText = "Slide Left, Right or Up to reveal the panel underneath.";
		}
    }
}

