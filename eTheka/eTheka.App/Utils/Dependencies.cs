using eTheka.App.ViewModels;
using MediatR.Registration;
using Microsoft.Extensions.DependencyInjection;
using System;
using eTheka.Services;
using eTheka.Messaging.Requests;
using MediatR;
using System.Collections.Generic;
using eTheka.Domain;
using eTheka.App.Messaging.Notifications;
using eTheka.Messaging.Notifications;
using eTheka.App.Messaging.Requests;

namespace eTheka.App.Utils;
public static class Dependencies
{

    private static readonly Lazy<IServiceScope> Scope = 
        new(() => BuildServiceProvider().CreateScope());

    public static TService GetRequiredService<TService>() 
        where TService : notnull
        => Scope.Value.ServiceProvider.GetRequiredService<TService>();

    private static IServiceProvider BuildServiceProvider()
    {
        IServiceCollection services =
            new ServiceCollection()
            .AddSingleton<MainViewModel>()
            .AddSingleton<MarkdownEditorViewModel>()
            .AddSingleton<MarkdownPreviewViewModel>()
            .AddSingleton<FiguresManagementViewModel>()
            .AddSingleton<MarkdownTranslationService>()
            .AddSingleton<FileService>()
            .AddSingleton<INotificationHandler<OperationOutcomeNotification>>(serviceProvider => serviceProvider.GetRequiredService<MainViewModel>())
            .AddSingleton<IRequestHandler<GetCurrentMarkdownRequest, string>>(serviceProvider => serviceProvider.GetRequiredService<MarkdownEditorViewModel>())
            .AddSingleton<IRequestHandler<GetCurrentFiguresRequest, IEnumerable<Figure>>>(serviceProvider => serviceProvider.GetRequiredService<FiguresManagementViewModel>())
            .AddSingleton<IRequestHandler<GenerateHtmlPreviewRequest>>(serviceProvider => serviceProvider.GetRequiredService<MarkdownPreviewViewModel>())
            .AddSingleton<INotificationHandler<MarkdownFileOpenedNotification>>(serviceProvider => serviceProvider.GetRequiredService<MarkdownEditorViewModel>())
            .AddSingleton<INotificationHandler<MarkdownFileOpenedNotification>>(serviceProvider => serviceProvider.GetRequiredService<FiguresManagementViewModel>());
        //.AddSingleton<ConfirmationViewModel>()
        // by reflection mediatr creates a new handler, hence it always creates a new VM - hardcode that thing to be a singleton :)
        //.AddSingleton<INotificationHandler<GenerateMarkdownPreviewNotification>>(serviceProvider => serviceProvider.GetRequiredService<MarkdownPreviewViewModel>())
        //.AddSingleton<IRequestHandler<GetCurrentMarkdownRequest, string>>(serviceProvider => serviceProvider.GetRequiredService<MarkdownEditorViewModel>())
        //.AddSingleton<IRequestHandler<GetCurrentFiguresRequest, IEnumerable<Figure>>>(serviceProvider => serviceProvider.GetRequiredService<ImageManagementViewModel>());
        // of course the automatic assembly loading for MediatR has to be removed, since it will additionally load the handlers by reflection


        var mediatrServiceConfig = new MediatRServiceConfiguration();
        ServiceRegistrar.AddRequiredServices(services, mediatrServiceConfig);

        return services.BuildServiceProvider();
    }
}
