using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PageLink : MonoBehaviour,IPointerClickHandler
{
    private string url;

    public void OnPointerClick(PointerEventData eventData)
    {
        url = GetComponent<Text>().text.Replace("　","");
        Application.OpenURL(url);
        Debug.Log(url);
    }
}
