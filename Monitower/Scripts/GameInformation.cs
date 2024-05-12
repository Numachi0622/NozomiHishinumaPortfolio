using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameInformation : MonoBehaviour
{
    //�X�L���̃��x���ϐ�
    public int coinUpLevel; //�R�C���l���ʃX�L���̃��x��
    public int goldEnemyProbabilityLevel; //�S�[���h�G�m���A�b�v�X�L���̃��x��
    public int bossBattleTimeLevel; //�{�X�펞�ԑ��ʃX�L���̃��x��
    public int rocketMagnificationLevel; //���[�U�[�_���{���̃��x��
    public int powerUpLevel; //�p���[�㏸�̃��x��
    public int powerUpTimeLevel; //n�b�����̃��x��
    public int rocketNumLevel;//���[�U�[�̉�
    public int weakPointNumLevel; //��_��
    public int weakPointMagnificationLevel;//��_�{���̃��x��

    public int numberOfPlays; //������
    public int progress; //�i�s�x
    public int havingTotalCoin; //�����Ă�R�C���̍��v

    private void Start()
    {
        havingTotalCoin = Refresh("TOTAL_COIN");
        progress = Refresh("PROGRESS");
        numberOfPlays = Refresh("NUMBER_OF_PLAYS");
    }

    //���x���X�V���ɌĂяo���֐�
    public int Refresh(string key)
    {
        int value;
        if (key == "NUMBER_OF_PLAYS" || key == "PROGRESS" || key == "TOTAL_COIN")
        {
            value = PlayerPrefs.GetInt(key);
        }
        else
        {
            value = PlayerPrefs.GetInt(key, 1);
        }
        return value;
    }
}
