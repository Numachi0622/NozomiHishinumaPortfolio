using UniRx;
using UnityEngine;

public class QuestionModel : MonoBehaviour
{
    [SerializeField] private float limitTime = 10f;
    public float LimitTime => limitTime;
    private ReactiveProperty<float> remainingTime = new ReactiveProperty<float>();
    public ReactiveProperty<float> RemainingTime => remainingTime;

    private FishDataList fishDataList;
    private FishData fishData;
    public FishData FishData => fishData;

    private bool isStop = true;
    private int count = 0;

    public float TimeElapsedRate { get; private set; }

    private void Awake()
    {
        fishDataList = new FishDataList();
        fishDataList.GetFishDataList.Shuffle();
        QuestionData.Init(fishDataList.GetFishDataList);
    }

    private void Update()
    {
        if(isStop || StateManager.Instance.State.Value != State.Game) return;
        remainingTime.Value -= Time.deltaTime;
    }

    public void Init()
    {
        SetStop(false);
        TimeElapsedRate = remainingTime.Value / limitTime;
        remainingTime.Value = limitTime;
        fishData = QuestionData.QuestionFishData[count];

        FishData newFishData;
        do
        {
            newFishData = fishDataList.GetFishDataList[Random.Range(0, fishDataList.GetFishDataList.Count)];
        }while(QuestionData.LastPlace().GetId == newFishData.GetId);
        QuestionData.QuestionFishData.Add(newFishData);
        count++;
    }

    public void SetStop(bool isStop)
    {
        this.isStop = isStop;
    }
}
