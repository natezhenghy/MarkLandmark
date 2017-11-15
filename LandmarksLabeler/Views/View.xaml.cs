using LandmarksLabeler.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace LandmarksLabeler.Views
{
    public sealed partial class View : Page
    {
        private readonly ViewModel _viewModel = new ViewModel();

        public View()
        {
            InitializeComponent();
            DataContext = _viewModel;
        }

        private void LM_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            var lm = (Ellipse)sender;
            var trans = (CompositeTransform)lm.RenderTransform;

            if (trans == null) return;
            trans.TranslateX += e.Delta.Translation.X;
            trans.TranslateY += e.Delta.Translation.Y;
        }
    }
}
