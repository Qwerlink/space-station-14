// © SS220, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/SerbiaStrong-220/space-station-14/master/CLA.txt
using Content.Client.SS220.Language;
using Content.Shared.SS220.Language;
using Content.Shared.SS220.Language.Components;
using Robust.Client.AutoGenerated;
using Robust.Client.Player;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Utility;

namespace Content.Client.SS220.UserInterface.System.Chat.Controls.LanguageSettings;

[GenerateTypedNameReferences]
public sealed partial class LanguageSettingsPopup : Popup
{
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IPlayerManager _player = default!;
    [Dependency] private readonly LanguageManager _languageManager = default!;

    private Dictionary<LanguagePrototype, Button> _buttonsDict = new();
    private Button? _selectedLanguage;

    public LanguageSettingsPopup()
    {
        IoCManager.InjectDependencies(this);
        RobustXamlLoader.Load(this);
        Update();
    }

    public void Update()
    {
        var uid = _player.LocalEntity;
        if (uid == null ||
            !_entityManager.TryGetComponent<LanguageComponent>(uid, out var comp))
        {
            UpdateLanguageContainer(null);
            UpdateSelectedLanguage(null);
            return;
        }

        var availableLanguages = new List<LanguagePrototype>();
        foreach (var def in comp.AvailableLanguages)
        {
            if (!def.CanSpeak ||
                !_languageManager.TryGetLanguageById(def.Id, out var language))
                continue;

            availableLanguages.Add(language);
        }
        UpdateLanguageContainer(availableLanguages);

        if (comp.SelectedLanguage is { } selectedLanguageId)
        {
            _languageManager.TryGetLanguageById(selectedLanguageId, out var selectedLanguage);
            UpdateSelectedLanguage(selectedLanguage);
        }
        else
            UpdateSelectedLanguage(null);
    }

    private void UpdateLanguageContainer(List<LanguagePrototype>? availableLanguages)
    {
        LanguageContainer.DisposeAllChildren();
        _buttonsDict.Clear();

        if (availableLanguages == null ||
            availableLanguages.Count <= 0)
        {
            LanguageLabel.Text = Loc.GetString("language-settings-ui-no-languages");
            return;
        }

        LanguageLabel.Text = Loc.GetString("language-settings-ui-select-default");
        foreach (var language in availableLanguages)
        {
            var languageKey = $"{LanguageManager.KeyPrefix}{language.Key}";
            var text = Loc.GetString("language-settings-ui-field-name",
                ("name", Loc.GetString(language.Name)), ("key", languageKey));
            var button = new Button() { Text = text };

            if (language.Description is { } desc)
            {
                var tooltip = new Tooltip();
                tooltip.SetMessage(FormattedMessage.FromMarkupPermissive(Loc.GetString(language.Description)));
                button.TooltipSupplier = _ => tooltip;
            }

            _buttonsDict.Add(language, button);
            button.OnPressed += _ =>
            {
                UpdateSelectedLanguage(language);
                var languageSystem = _entityManager.System<LanguageSystem>();
                languageSystem.SelectLanguage(language.ID);
            };
            LanguageContainer.AddChild(button);
        }
    }

    private void UpdateSelectedLanguage(LanguagePrototype? language)
    {
        if (language == null)
        {
            if (_selectedLanguage != null)
                _selectedLanguage.Disabled = false;

            _selectedLanguage = null;
            return;
        }

        if (_buttonsDict.TryGetValue(language, out var button))
        {
            if (_selectedLanguage != null)
                _selectedLanguage.Disabled = false;

            button.Disabled = true;
            _selectedLanguage = button;
        }
    }
}


