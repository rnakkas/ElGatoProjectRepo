using Godot;
using System;

namespace ElGatoProject.Singletons;

public partial class Globals : Node
{
    public static Globals Instance { get; private set; }

    public RandomNumberGenerator Rng = new();

    public override void _Ready()
    {
        Instance = this;
    }
}
