using Godot;

public partial class Policeman : StaticBody2D
{
    private Resource _dialogueResource = null!;

    /// <inheritdoc />
    public override void _Ready()
    {
        _dialogueResource = GD.Load("res://scenes/dialogue/PoliceOfficer.dialogue");
    }

    private void OnInteraction(Node2D initiator)
    {
        if (initiator is not Player) return;

        if (PlayerData.Current!.NewspaperPickedUp)
        {
            if (PlayerData.Current.PolicemanQuestActive)
            {
                // todo: play dialog "police_officer_first_time_after_newspaper"
            }
            else
            {
                // todo: play dialog "police_after_finding_newspaper"
            }

            QueueFree();
        }
        else
        {
            // todo: play dialog "police_officer"
            PlayerData.Current.PolicemanQuestActive = true;
        }
    }
}
