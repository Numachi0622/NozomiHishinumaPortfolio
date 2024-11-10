using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum State
{
    Title,
    Game,
    Result
}

public class StateManager : MonoBehaviour
{
    public static StateManager Instance;
    private ReactiveProperty<State> state = new ReactiveProperty<State>();
    public ReactiveProperty<State> State => state;
    [SerializeField] private State currentState;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void SetState(State state)
    {
        this.state.Value = state;
        currentState = state;
    }

    private void Update(){
        if (Input.GetKeyDown(KeyCode.C))
        {
            SceneManager.LoadScene("Calibration");
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            SceneManager.LoadScene("Game");
        }
    }
}
