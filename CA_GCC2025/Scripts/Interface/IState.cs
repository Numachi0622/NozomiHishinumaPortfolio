using R3;
public interface IState<T>
{
    public T CurrentState { get; }
    public void SetState(T state);
}
