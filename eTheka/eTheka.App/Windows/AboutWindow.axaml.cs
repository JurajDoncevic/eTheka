using Avalonia.Controls;
using System;

namespace eTheka.App.Windows;
public partial class AboutWindow : Window
{
    public AboutWindow()
    {
        InitializeComponent();
    }


    public void ClickedLink(string linkString)
    {
        OpenUriInBrowser(new Uri(linkString));
    }

    public void OpenUriInBrowser(Uri? uri)
    {
        if (uri is not null)
        {
            var strUri = uri.OriginalString;
            var sInfo = new System.Diagnostics.ProcessStartInfo(strUri)
            {
                UseShellExecute = true,
            };
            System.Diagnostics.Process.Start(sInfo);
        }
    }
}
