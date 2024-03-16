using DirectContactClient.Views;
#if ANDROID
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
#endif

namespace DirectContactClient
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            /*Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("MyCustomization", (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Colors.Transparent.ToAndroid());
#endif
            });*/

            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("CursorColor", (handler, view) =>
            {
#if __ANDROID__
                handler.PlatformView.TextCursorDrawable.SetTint(Colors.Black.ToAndroid());
#endif
            });
            MainPage = new AppShell();
        }
    }
}
