using ElGatoProject.Utilties;
using Godot;

namespace ElGatoProject.Singletons;

public partial class EventsBus : Node
{
	public static EventsBus Instance { get; private set; } // Needed for c# singleton autoload

	[Signal] 
	public delegate void PlayerMaxHealthUpdateEventHandler(int maxHealth);
	[Signal] 
	public delegate void PlayerCurrentHealthUpdateEventHandler(int currentHealth);
	[Signal]
	public delegate void PlayerDiedEventHandler();
	[Signal]
	public delegate void PlayerScoreUpdateEventHandler(int score);
	[Signal]
	public delegate void PlayerCurrentWeaponUpdateEventHandler(string weaponType);
	[Signal]
	public delegate void PlayerAmmoUpdateEventHandler(int ammo);

	[Signal]
	public delegate void
		PlayerEnteredEnemySpawnTriggerEventHandler(string enemyType, Vector2 playerPosition);
	
	public override void _Ready()
	{
		// Needed for c# singleton autoload
		Instance = this;
	}
}
