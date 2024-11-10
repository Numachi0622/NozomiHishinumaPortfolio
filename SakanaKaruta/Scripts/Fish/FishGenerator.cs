using System.Collections.Generic;
using UnityEngine;
using UniRx;
using DG.Tweening;
using UnityEngine.SceneManagement;


public class FishGenerator : MonoBehaviour
{
    [SerializeField] private GameObject fishPrefab;
    [SerializeField] private GameObject firstFishPrefab;
    private List<GameObject> fishPool = new List<GameObject>();
    private FishDataList fishDataList;
    private FishColliderModel fishColliderModel;
    private FishVisibilityModel fishVisibilityModel;
    private int generateCount = 12;

    private void Awake()
    {
        fishColliderModel = GetComponent<FishColliderModel>();
        fishVisibilityModel = GetComponent<FishVisibilityModel>();
    }

    private void Start()
    {
        fishDataList = new FishDataList();
        FirstGenerate();
        StateManager.Instance.State.Subscribe(state =>
        {
            if (state != State.Game) return;
            for (int i = 0; i < generateCount; i++)
            {
                Generate(i);
            }
            fishVisibilityModel.OnCompleteSetVisibilityList();
        });
    }

    private void Generate()
    {
        DOVirtual.DelayedCall(1f, () =>
        {
            if (StateManager.Instance.State.Value != State.Game || SceneManager.GetActiveScene().name != "Game") return;
            GameObject fish = Instantiate(fishPrefab);

            // ���̏���^����
            FishData fishData = QuestionData.QuestionFishData[generateCount];
            fish.GetComponent<Fish>().Init(fishData);
            fish.GetComponent<FishView>().Init(fishData);
            fish.GetComponent<FishMove>().enabled = true;
            var visibility = fish.GetComponent<FishVisibility>();
            visibility.SetVisibilityModel(this.fishVisibilityModel);
            visibility.OnDisableAction = Generate;

            fishColliderModel.FishColliders.Add(fish.GetComponent<Collider2D>());
            fishVisibilityModel.FishVisibilityList.Add(fish.GetComponent<FishVisibility>());

            generateCount++;
        });
    }

    private void Generate(int index)
    {
        if (StateManager.Instance.State.Value != State.Game) return;
        GameObject fish = Instantiate(fishPrefab);

        // ���̏���^����
        FishData fishData = fishDataList.GetFishDataList[index];
        fish.GetComponent<Fish>().Init(fishData);
        fish.GetComponent<FishView>().Init(fishData);
        var visibility = fish.GetComponent<FishVisibility>();
        visibility.SetVisibilityModel(this.fishVisibilityModel);
        visibility.OnDisableAction = Generate;
        
        fishColliderModel.FishColliders.Add(fish.GetComponent<Collider2D>());
        fishVisibilityModel.FishVisibilityList.Add(fish.GetComponent<FishVisibility>());
    }

    private void FirstGenerate()
    {
        GameObject firstFish = Instantiate(firstFishPrefab);
        fishColliderModel.FishColliders.Add(firstFish.GetComponent<Collider2D>());
    }

    private GameObject GetFishFromPool()
    {
        for (int i = 0; i < fishPool.Count; i++)
        {
            if (!fishPool[i].activeSelf)
            {
                fishPool[i].gameObject.SetActive(true);
                return fishPool[i];
            }
        }
        GameObject newFish = Instantiate(fishPrefab);
        fishPool.Add(newFish);
        return newFish;
    }
}
