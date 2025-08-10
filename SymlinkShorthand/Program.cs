using System;
using Avalonia;

namespace SymlinkShorthand
{
    internal sealed class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            AppBuilder app = BuildAvaloniaApp();

            // Args handling
            if (args.Length > 0)
            {
                SymlinkShorthand.ViewModels.MainWindowViewModel.ArgsTarget = args[0];
            }

            app.StartWithClassicDesktopLifetime(args);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();
    }
}
