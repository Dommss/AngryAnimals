using Godot;

public partial class Animal : RigidBody2D {
    private SignalManager _signalManager;
    public override void _Ready() {
        _signalManager = GetNode<SignalManager>("/root/SignalManager");
    }

    public override void _PhysicsProcess(double delta) {
        UpdateDebugLabel();
    }

    private void UpdateDebugLabel() {
        var s = "g_pos: " + Utilities.VectorToString(GlobalPosition) + "\n";

        s += "ang: " + AngularVelocity.ToString("F1") + "\n";
        s += "linear: " + Utilities.VectorToString(LinearVelocity);

        _signalManager.EmitSignal(SignalManager.SignalName.UpdateDebugLabel, s);
    }
}
