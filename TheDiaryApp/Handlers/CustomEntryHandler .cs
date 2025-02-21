using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

#if ANDROID
using AndroidX.AppCompat.Widget; // Добавляем пространство имен для AppCompatEditText
#endif

#if IOS
using UIKit;
#endif

namespace TheDiaryApp.Handlers
{
    public partial class CustomEntryHandler : EntryHandler
    {
#if ANDROID
        protected override void ConnectHandler(AppCompatEditText platformView)
        {
            base.ConnectHandler(platformView);

            // Убираем встроенную границу на Android
            platformView.Background = null;
        }
#endif

#if IOS
        protected override void ConnectHandler(UITextField platformView)
        {
            base.ConnectHandler(platformView);

            // Убираем встроенную границу на iOS
            platformView.BorderStyle = UITextBorderStyle.None;
        }
#endif
    }
}