// © SS220, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/SerbiaStrong-220/space-station-14/master/CLA.txt
using Content.Shared.Administration;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using Content.Client.UserInterface.Controls;

namespace Content.Client.SS220.DialogWindowDescUI;

/// <summary>
/// A modified <see cref="DialogWindow"/> window in which you can send an additional description to explain a particular action being performed
/// </summary>
[GenerateTypedNameReferences]
public sealed partial class DialogWindowDesc : FancyWindow
{
    private List<(string, LineEdit)> _promptLines;

    private bool _finished;

    public Action<Dictionary<string, string>>? OnConfirmed;

    public Action? OnCancelled;

    public DialogWindowDesc(string title, string dsscEntty, List<QuickDialogEntry> entries, bool ok = true)
    {
       RobustXamlLoader.Load(this);

        Title = title;

        OkButton.Visible = ok;

        _promptLines = new(entries.Count);

        for (int i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];

            var box = new BoxContainer();
            box.AddChild(new Label() { Text = entry.Prompt, Align = Label.AlignMode.Center, HorizontalExpand = true});
            Prompts.AddChild(box);

            var boxDesc = new BoxContainer();
            boxDesc.AddChild(new Label() { Text = dsscEntty, Align = Label.AlignMode.Center, FontColorOverride = Color.Gray, HorizontalExpand = true });
            Prompts.AddChild(boxDesc);

            var boxEmpty = new BoxContainer();
            boxEmpty.AddChild(new BoxContainer() {MinHeight=20});
            Prompts.AddChild(boxEmpty);

            var boxEdit = new BoxContainer();
            var edit = new LineEdit() { HorizontalExpand = true };
            boxEdit.AddChild(edit);

            _promptLines.Add((entry.FieldId, edit));
            Prompts.AddChild(boxEdit);
        }

        OkButton.OnPressed += _ => Confirm();

        OnClose += () =>
        {
            if (!_finished)
                OnCancelled?.Invoke();
        };

        _promptLines[0].Item2.GrabKeyboardFocus();

        OpenCentered();
    }

    private void Confirm()
    {
        var results = new Dictionary<string, string>();
        foreach (var (field, edit) in _promptLines)
        {
            results[field] = edit.Text;
        }

        _finished = true;
        OnConfirmed?.Invoke(results);
        Close();
    }
}

