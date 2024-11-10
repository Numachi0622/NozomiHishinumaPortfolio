using UnityEngine;
using UniRx;

public class HitPresenter : MonoBehaviour
{
    private HitModel hitModel;
    [SerializeField] private MouseClickView mouseClickView;
    [SerializeField] private FishColliderModel fishColliderModel;
    [SerializeField] private QuestionModel questionModel;
    [SerializeField] private QuestionView questionView;
    [SerializeField] private ScoreModel scoreModel;
 
    private void Awake()
    {
        hitModel = GetComponent<HitModel>();
    }

    private void Start()
    {
        this.hitModel.HitPos.Subscribe(pos =>
        {
            if (StateManager.Instance.State.Value == State.Result) return;
            
            foreach (Collider2D col in fishColliderModel.FishColliders)
            {
                if (!col.bounds.Contains(pos))
                {
                    if(Vector2.Distance(col.transform.position, pos) <= 3f && StateManager.Instance.State.Value == State.Game)
                    {
                        col.GetComponent<FishMove>().Escape();
                    }
                    continue;
                }

                this.mouseClickView.ShowHitEffect(pos);
                Debug.Log(col.GetComponent<Fish>().KanaName + "�G�ꂽ");
                hitModel.HitColliders.Add(col);
                if (StateManager.Instance.State.Value == State.Title)
                {
                    StateManager.Instance.SetState(State.Game);
                    this.mouseClickView.DestroyFish(col.GetComponent<Fish>(), fishColliderModel);
                    hitModel.HitColliders.Clear();
                    return;
                }
            }

            Fish answerFish = null;
            foreach(Collider2D col in hitModel.HitColliders)
            {
                var fish = col.gameObject.GetComponent<Fish>();
                int fishId = fish.GetId;
                int answerId = questionModel.FishData.GetId;
                if (fishId != answerId) continue;
                answerFish = fish;
                break;
            }

            if (answerFish != null)
            {
                answerFish.GetComponent<FishView>().SetLayer();
                questionModel.SetStop(true);
                this.hitModel.SetPosPossible(false);
                this.mouseClickView.HideFish(answerFish, fishColliderModel, scoreModel, questionModel, () =>
                {
                    answerFish.GetComponent<FishView>().ResetLayer();
                    var previousFishData = questionModel.FishData;
                    questionView.ShowHint(null, false);
                    questionModel.Init();
                    questionView.UpdateQuestion(questionModel.FishData, previousFishData);
                    this.hitModel.SetPosPossible(true);
                });
            }
            else
            {
                foreach (Collider2D col in hitModel.HitColliders)
                {
                    col.GetComponent<FishMove>().Escape();
                    this.mouseClickView.ShowBadUI(pos);
                }
            }
            hitModel.HitColliders.Clear();
        })
        .AddTo(this);
    }
}
