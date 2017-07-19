using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UhaSub.ViewModel
{
    public class SettingViewModel : BaseViewModel
    {
        public int SelectedOptionsIndex { get; set; }
        public SettingViewModel(int index=0,int option_index =0)
        {
            this.SelectedIndex = index;
            this.SelectedOptionsIndex = option_index;
        }


    }
}
