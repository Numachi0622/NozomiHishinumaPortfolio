using UnityEngine;

public class Fish : MonoBehaviour
{
    [SerializeField] private string kanjiName;
    [SerializeField] private string kanaName;
    private int id;

    public void Init(FishData fishData)
    {
        this.kanjiName = fishData.GetKanjiName;
        this.kanaName = fishData.GetKanaName;
        this.id = fishData.GetId;
    }

    public int GetId => id;
    public string KanaName => kanaName;
    public string KanjiName => kanjiName;
}
