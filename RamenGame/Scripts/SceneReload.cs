using UnityEngine;
using UnityEngine.SceneManagement;

// リトライする際にシーンを再読み込みするクラス
public class SceneReload : MonoBehaviour
{
    // 再ロード
    public void Reload()
    {
        SceneManager.LoadScene("Game");
    }
}
