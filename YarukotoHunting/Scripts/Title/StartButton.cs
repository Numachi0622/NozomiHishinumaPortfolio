using UnityEngine;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    [SerializeField] private GameObject checkMark;

    public void DisplayCheckMark()
    {
        checkMark.SetActive(true);
        GetComponent<Image>().raycastTarget = false;
    }
}
