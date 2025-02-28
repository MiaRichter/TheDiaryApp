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
    }
}