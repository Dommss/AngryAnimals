using Godot;

public partial class Level : Node2D {
    private SignalManager _signalManager;

    private Label _debugLabel;

    public override void _Ready() {
        _signalManager = GetNode<SignalManager>("/root/SignalManager");

        _debugLabel = GetNode<Label>("DebugLabel");
        
        _signalManager.UpdateDebugLabel += OnUpdateDebugLabel;
    }

    private void OnUpdateDebugLabel(string text) {
        _debugLabel.Text = text;
    }
}