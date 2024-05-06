using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private List<Fish> gottenFish = new List<Fish>();

    public void SetFishList(Fish _fish)
    {
        gottenFish.Add(_fish);
    }

    public string Score()
    {
        switch (gottenFish.Count)
        {
            case 5:
                return "S";
            case 4:
                return "A";
            case 3:
                return "B";
            case 2:
                return "C";
            case 1:
                return "D";
            default: return "E";
        }
    }
}
