using Godot;

public partial class GameManager : Node {
    public readonly string GroupCup = "Cup";
    public readonly string GroupAnimal = "Animal";

    public PackedScene MainScene = GD.Load<PackedScene>("res://Main/Main.tscn");

    public void LoadMainScene() {
        GetTree().ChangeSceneToPacked(MainScene);
    }
}
