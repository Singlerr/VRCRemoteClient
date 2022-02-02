using System.Windows;
using CefSharp;

namespace VRCRemote
{
    /// <summary>
    ///     MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private static MainWindow _instance;

        public MainWindow()
        {
            InitializeComponent();
            InitBrowser();
            _instance = this;
        }

        public static MainWindow GetInstance()
        {
            return _instance;
        }

        public void LoadUrl(string url)
        {
            Dispatcher.Invoke(delegate
            {
                Browser.LoadUrl(url);
            });
        }

        public void ExecuteJavascript(string script)
        {
            Dispatcher.Invoke(delegate { Browser.ExecuteScriptAsync(script); });
        }

        private void InitBrowser()
        {
            Browser.LoadUrl("local://resources/");
            Browser.JavascriptObjectRepository.ResolveObject += (sender, e) =>
            {
                var repo = e.ObjectRepository;
                if (e.ObjectName == "exposedObject")
                {
                    var bindingOptions = BindingOptions.DefaultBinder;
                    repo.Register("exposedObject", new ExposedObject(), true, bindingOptions);
                }
                else if (e.ObjectName == "readOnly")
                {
                    repo.Register("readOnly", new ReadOnly(null, 0), true, BindingOptions.DefaultBinder);
                }
            };
   
            Browser.JavascriptObjectRepository.ObjectBoundInJavascript += (sender, e) => { };
        }
    }
}