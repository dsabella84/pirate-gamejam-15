using Godot;

public partial class Partygoer : StaticBody2D
{
    private Resource _dialogResource = null!;

    /// <inheritdoc />
    public override void _Ready()
    {
        _dialogResource = GD.Load<Resource>("res://scenes/dialogue/Clubhouse.dialogue");
    }

    private void _OnInteractableInteractionStarted(Node2D initiator)
    {
        if (initiator is not Player) return;

        if (PlayerData.Current!.PartygoerPasswordSeen)
        {
            // todo: start dialog (_dialogResource, "partygoer")
        }
        else
        {
            // todo: start dialog (_dialogResource, "partygoer_after_password_seen")
        }
    }
}
