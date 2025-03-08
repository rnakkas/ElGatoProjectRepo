using Godot;
using ElGatoProject.Resources;

namespace ElGatoProject.Singletons;

public partial class Globals : Node
{
    public static Globals Instance { get; private set; }

    public RandomNumberGenerator Rng = new();

    public int PlayerScore = 0;

    public bool IsPlayerDying;
    
    public override void _Ready()
    {
        Instance = this;
    }
}
