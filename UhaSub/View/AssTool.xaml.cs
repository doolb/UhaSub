using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UhaSub.ui
{
    /// <summary>
    /// Interaction logic for AssTool.xaml
    /// </summary>
    public partial class AssTool : UserControl
    {
        public AssTool()
        {
            InitializeComponent();
        }


        #region Dependency Propertys

        #region Selected Ass Dependency Property

        public Ass Ass
        {
            get { return (Ass)this.GetValue(AssProperty); }
            set { this.SetValue(AssProperty, value); }
        }
        public static readonly DependencyProperty AssProperty = DependencyProperty.Register(
          "Ass", typeof(Ass), typeof(AssTool), new PropertyMetadata(OnAssChanged));

        private static void OnAssChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as AssTool).OnAssChanged(e);
        }

        private void OnAssChanged(DependencyPropertyChangedEventArgs e)
        {
            Ass ass = (Ass)e.NewValue;
            if (ass == null) return;
            preview.Text = ass.Text;

        }

        #endregion

        #endregion

    }
}
