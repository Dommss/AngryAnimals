using Godot;

public partial class SignalManager : Node {
    [Signal] public delegate void UpdateDebugLabelEventHandler(string text);
    [Signal] public delegate void AnimalDiedEventHandler();
    [Signal] public delegate void CupDestroyedEventHandler();
}
