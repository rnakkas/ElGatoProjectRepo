namespace ElGatoProject.StateSystem.Interfaces;

public interface IState
{
    // Initalise the state with statemachine
    void Initialize(StateMachine stateMachine);
    
    // Called when the state becomes active.
    void Enter();

    // Called when the state is being exited.
    void Exit();

    // Called every frame to update state logic.
    void Update(float delta);

    // Called every physics frame.
    void PhysicsUpdate(float delta);

}