using UnityEngine;
using UnityEngine.SceneManagement;

// ���g���C����ۂɃV�[�����ēǂݍ��݂���N���X
public class SceneReload : MonoBehaviour
{
    // �ă��[�h
    public void Reload()
    {
        SceneManager.LoadScene("Game");
    }
}
