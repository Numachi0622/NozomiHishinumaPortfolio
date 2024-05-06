using UnityEngine;
using UnityEngine.UI;

public class Scroller : MonoBehaviour
{
    [SerializeField] private RawImage bg;
    [SerializeField] private float dx, dy;

    private void Update()
    {
        bg.uvRect = new Rect(bg.uvRect.position + new Vector2(dx,dy) * Time.deltaTime,bg.uvRect.size);
    }
}
