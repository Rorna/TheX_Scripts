public interface IPlayerMovementState : IState
{
    public void HandleMoveInput();
    public void HandleJumpInput();
}
