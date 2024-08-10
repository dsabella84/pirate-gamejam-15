using Godot;

public partial class Interactable : Area2D
{
    [Signal]
    public delegate void InteractionStartedEventHandler(Node2D initiator);

    [Signal]
    public delegate void InteractionEndedEventHandler(Node2D initiator);

    public void StartInteraction(Node2D initiator)
    {
        EmitSignal("InteractionStarted", initiator);
    }

    public void EndInteraction(Node2D initiator)
    {
        EmitSignal("InteractionEnded", initiator);
    }
}
