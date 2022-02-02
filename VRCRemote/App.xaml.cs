using System;
using System.IO;
using System.Windows;
using CefSharp;
using CefSharp.SchemeHandler;
using CefSharp.Wpf;

namespace VRCRemote
{
    /// <summary>
    ///     App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            var missing = DependencyChecker.CheckDependencies(checkOptional:true, packLoadingDisabled: false, path: "",
                resourcesDirPath: "", browserSubProcessPath: "CefSharp.BrowserSubprocess.exe", localePackFile: @"locales\\ko.pak");
            if (missing.Count > 0)
            {
                MessageBox.Show($"프로그램 실행에 필요한 다음의 파일을 찾지 못했습니다. {string.Join(",", missing)}","오류",MessageBoxButton.OK,MessageBoxImage.Error);
                return;
            }
            Init();
        }

        private static void Init()
        {
            var settings = new CefSettings();
            settings.LogSeverity = LogSeverity.Verbose;
            settings.CachePath = Path.Combine(Environment.CurrentDirectory, "cache");
            settings.CefCommandLineArgs.Add("enable-media-stream");

            settings.RegisterScheme(new CefCustomScheme
            {
                SchemeName = "local",
                DomainName = "resources",
                SchemeHandlerFactory = new FolderSchemeHandlerFactory(@"views",
                    hostName: "resources",
                    defaultPage: "signin.html")
            });

            CefSharpSettings.ShutdownOnExit = true;
            Cef.Initialize(settings);
        }
    }
}