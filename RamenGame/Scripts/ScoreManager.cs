using UnityEngine;

// �X�R�A���v�Z����N���X
public class ScoreManager : MonoBehaviour
{
    [SerializeField] private Cut cut;

    // �z��̗v�f�ɓn���֐�
    public string Score()
    {
        if (cut.count >= 80) 
            return "S";
        else if (cut.count >= 60) 
            return "A";
        else if (cut.count >= 40) 
            return "B";
        else if (cut.count >= 20) 
            return "C";
        else if (cut.count > 0) 
            return "D";
        else return "E";
    }
}
