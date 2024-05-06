using UnityEngine;

// 切るアクション関連の処理をまとめたクラス
public class Cut : MonoBehaviour
{
    [SerializeField] private MeshRenderer myKnifeRenderer; // 自身の手に持つ包丁のMeshRendererコンポーネント
    [SerializeField] private GameObject staticKnife; // 机に置かれている包丁オブジェクト
    [SerializeField] private GameManager gameManager; // GameManagerクラス
    [SerializeField] private AudioClip cutSE; // 切ったときのSE
    public int count { get; private set; } // 切った回数

    private void Start()
    {
        count = 0;
    }

    // 手に包丁を持たせるメソッド
    public void SetKnife()
    {
        if (!gameManager.gameStartable) return;
        // 机の包丁を非アクティブ化
        staticKnife.SetActive(false);
        // 手の包丁を表示
        myKnifeRenderer.enabled = true;   
    }

    // 切ったときの処理の実行するメソッド
    public void CutAction()
    {
        count++;
        SoundManager.instance.PlaySE(cutSE);
        // VRのコントローラーを振動させる
        OVRInput.SetControllerVibration(0, 1f, OVRInput.Controller.RTouch);
    }
}
