using log4net.Config;
using System.Windows;

namespace MarkLandmark
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            XmlConfigurator.Configure();
        }
    }
}
