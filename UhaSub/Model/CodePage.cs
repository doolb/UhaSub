using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UhaSub.ViewModel;

namespace UhaSub.Model
{
    public class CodePage
    {
        public string Name { get; set; }

        public int Code { get; set; }

        private ICommand selectCodePage;
        public ICommand SelectCodePage
        {
            get
            {
                return selectCodePage ??
                (
                    selectCodePage = new Command
                    {
                        ExecuteDelegate = x =>
                        {
                            MainViewModel.Instance.NewWindowVM.CodePage = this.Code;
                            MainViewModel.Instance.NewWindowVM.Preview();
                        }
                    }
                );
            }
        }

        public CodePage(string name,int code_page)
        {
            Name = name;
            Code = code_page;
        }

        private static List<CodePage> codePages;
        public static List<CodePage> CodePages
        {
            get
            {
                if (codePages != null) return codePages;

                codePages = new List<CodePage>();
                codePages.Add(new CodePage("简体中文 (GB-2312)", 936));
                codePages.Add(new CodePage("繁體中文 (BIG-5)", 950));

                return codePages;
            }
        }
    }
}
