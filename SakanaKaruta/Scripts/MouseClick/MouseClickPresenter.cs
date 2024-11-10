using UniRx;
using UnityEngine;

public class MouseClickPresenter : MonoBehaviour
{
    private MouseClickView mouseClickView;
    private MouseClickModel mouseClickModel;
    [SerializeField] private FishColliderModel fishColliderModel;
    [SerializeField] private QuestionModel questionModel;
    [SerializeField] private QuestionView questionView;
    [SerializeField] private ScoreModel scoreModel;

    private void Awake()
    {
        this.mouseClickModel = new MouseClickModel();
        this.mouseClickView = GetComponent<MouseClickView>();
        
        this.mouseClickView.OnClick.Subscribe(pos =>
        {
            if(StateManager.Instance.State.Value == State.Result) return;
            
            Vector3 screenPos = new Vector3(pos.x, pos.y, Mathf.Abs(Camera.main.transform.position.z));
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

            // ÉqÉbÉgÇµÇΩãõèÓïÒÇàÍíUï€éùÇ∑ÇÈ
            foreach (Collider2D col in fishColliderModel.FishColliders)
            {
                if (!col.bounds.Contains(worldPos))
                {
                    if (Vector2.Distance(col.transform.position, worldPos) <= 5f)
                    {
                        col.GetComponent<FishMove>().Escape();
                    }
                    continue;
                }

                this.mouseClickView.ShowHitEffect(worldPos);
                mouseClickModel.HitColliders.Add(col);
                if (StateManager.Instance.State.Value == State.Title)
                {
                    StateManager.Instance.SetState(State.Game);
                    this.mouseClickView.DestroyFish(col.GetComponent<Fish>(), fishColliderModel);
                    mouseClickModel.HitColliders.Clear();
                    return;
                }
            }

            // ï€éùÇµÇΩãõèÓïÒÇ©ÇÁê≥âÇ™ä‹Ç‹ÇÍÇƒÇ¢ÇÈÇ©îªíË 
            Fish answerFish = null;
            foreach (Collider2D col in mouseClickModel.HitColliders)
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
                this.mouseClickView.HideFish(answerFish, fishColliderModel, scoreModel,questionModel, () =>
                {
                    answerFish.GetComponent<FishView>().ResetLayer();
                    var previousFishData = questionModel.FishData;
                    questionView.ShowHint(null, false);
                    questionModel.Init();
                    questionView.UpdateQuestion(questionModel.FishData, previousFishData);
                });
            }
            else
            {
                foreach(Collider2D col in mouseClickModel.HitColliders)
                {
                    this.mouseClickView.ShowBadUI(worldPos);
                    col.GetComponent<FishMove>().Escape();
                }
            }
            mouseClickModel.HitColliders.Clear();
        })
        .AddTo(this);
    }
}
