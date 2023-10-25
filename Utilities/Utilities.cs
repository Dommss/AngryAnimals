using Godot;

[GlobalClass]
public partial class Utilities
{
    public static string VectorToString(Vector2 vec) {
        return "X: " + vec.X.ToString("F1") + " - Y: " + vec.Y.ToString("F1");
    }
}
