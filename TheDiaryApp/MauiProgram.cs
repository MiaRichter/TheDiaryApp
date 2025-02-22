﻿using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Toolkit.Hosting;
using TheDiaryApp.Pages;
using TheDiaryApp.Handlers;
using TheDiaryApp.Controls;


namespace TheDiaryApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureSyncfusionToolkit()
			.ConfigureMauiHandlers(handlers =>
			{
                handlers.AddHandler(typeof(CustomEntry), typeof(CustomEntryHandler));
            })
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
		builder.Services.AddLogging(configure => configure.AddDebug());
#endif

		builder.Services.AddSingleton<ReportRepo>();
		builder.Services.AddSingleton<ExcelParser>();
		builder.Services.AddSingleton<ReplacementParser>();
        builder.Services.AddSingleton<InvertedBoolConverter>();
        builder.Services.AddSingleton<ScheduleViewModel>();
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<SettingsViewModel>();
        builder.Services.AddSingleton<SettingsPage>();
        builder.Services.AddSingleton<TeacherRepo>();
        builder.Services.AddSingleton<TeacherParser>();
        return builder.Build();
	}
}
