// © SS220, An EULA/CLA with a hosting restriction, full text: https://raw.githubusercontent.com/SerbiaStrong-220/space-station-14/master/CLA.txt
using System.Numerics;
using Content.Client.MainMenu.UI;
using Content.Client.UserInterface.Controls;
using Content.Shared.SS220.SmartGasMask;
using Content.Shared.SS220.SmartGasMask.Prototype;
using Robust.Client.AutoGenerated;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Prototypes;

namespace Content.Client.SS220.SmartGasMask;

[GenerateTypedNameReferences]
public sealed partial class SmartGasMaskMenu : RadialMenu
{
    public event Action<ProtoId<AlertSmartGasMaskPrototype>>? SendAlertSmartGasMaskRadioMessageAction;

    [Dependency] private readonly EntityManager _entityManager = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

    public EntityUid Entity { get; set; }

    public SmartGasMaskMenu()
    {
        IoCManager.InjectDependencies(this);
        RobustXamlLoader.Load(this);
    }

    public void SetEntity(EntityUid uid)
    {
        Entity = uid;
        RefreshUI();
    }

    private void RefreshUI()
    {
        var spriteSystem = _entityManager.System<SpriteSystem>();

        if (!_entityManager.TryGetComponent<SmartGasMaskComponent>(Entity, out var comp))
            return;

        foreach (var smartProtoString in comp.SelectablePrototypes)
        {
            if (!_prototypeManager.TryIndex(smartProtoString, out var alertProto))
                continue;

            var button = new SmartGasMaskMenuButton()
            {
                SetSize = new Vector2(64, 64),
                ToolTip = Loc.GetString(alertProto.Name),
            };

            var entProtoView = new TextureRect()
            {
                SetSize = new Vector2(48, 48),
                VerticalAlignment = VAlignment.Center,
                HorizontalAlignment = HAlignment.Center,
                Texture = spriteSystem.Frame0(alertProto.Icon),
                TextureScale = new Vector2(2f, 2f)
            };

            button.AddChild(entProtoView);
            Main.AddChild(button);

            button.OnButtonUp += _ =>
            {
                SendAlertSmartGasMaskRadioMessageAction?.Invoke(alertProto.ID);
                Close();
            };
        }
    }
}

public sealed class SmartGasMaskMenuButton : RadialMenuTextureButtonWithSector
{
}

