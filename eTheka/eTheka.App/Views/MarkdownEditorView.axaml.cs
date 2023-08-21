using Avalonia.Controls;
using AvaloniaEdit;
using AvaloniaEdit.TextMate;
using eTheka.App.Utils;
using eTheka.App.ViewModels;
using System;
using TextMateSharp.Grammars;

namespace eTheka.App.Views;
public partial class MarkdownEditorView : UserControl
{
    private readonly MarkdownEditorViewModel _viewModel;

    public MarkdownEditorView()
        : this(Dependencies.GetRequiredService<MarkdownEditorViewModel>())
    {

    }

    public MarkdownEditorView(MarkdownEditorViewModel viewModel)
    {
        _viewModel = viewModel;
        DataContext = _viewModel;
        InitializeComponent();
        SetupEditor();
    }

    private void SetupEditor()
    {
        //First of all you need to have a reference for your TextEditor for it to be used inside AvaloniaEdit.TextMate project.
        var _textEditor = this.FindControl<TextEditor>("Editor");

        RegistryOptions? _registryOptions = null;
        //Here we initialize RegistryOptions with the theme we want to use.
        if (Avalonia.Application.Current!.ActualThemeVariant.Key.Equals("Light"))
        {
            _registryOptions = new RegistryOptions(ThemeName.LightPlus);
        }
        if (Avalonia.Application.Current!.ActualThemeVariant.Key.Equals("Dark"))
        {
            _registryOptions = new RegistryOptions(ThemeName.DarkPlus);
        }
        _registryOptions ??= new RegistryOptions(ThemeName.Light);

        //Initial setup of TextMate.
        var _textMateInstallation = _textEditor.InstallTextMate(_registryOptions);

        //Here we are getting the language by the extension and right after that we are initializing grammar with this language.
        //And that's all, you are ready to use AvaloniaEdit with syntax highlighting!
        _textMateInstallation.SetGrammar(_registryOptions.GetScopeByLanguageId(_registryOptions.GetLanguageByExtension(".md").Id));
    }

    public void Editor_TextChanged(object? sender, EventArgs eventArgs)
    {
        if (sender is TextEditor textEditor)
        {
            var caretOffset = textEditor.CaretOffset; // where the typing this is
            _viewModel.MarkdownText = textEditor.Document.Text;
            textEditor.CaretOffset = caretOffset;
        }

    }
}
