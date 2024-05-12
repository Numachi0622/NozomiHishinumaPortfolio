using Newtonsoft.Json.Linq;
using OVR.OpenVR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalMonitorManager : MonoBehaviour
{
    //NormalMonitorManaher�̃C���X�^���X
    public static NormalMonitorManager instance;

    //�v�[������I�u�W�F�N�g
    [SerializeField] GameObject prefab;

    //�v�[���̃I�u�W�F�N�g���X�g
    private List<GameObject> objectPool;

    //�v�[�������
    private const int initialSize = 1;

    //�I�u�W�F�N�g�̏o�����W
    Vector3 firstPos = new Vector3(0, 5, 0);

    //���̖ڂ̓G�����J�E���g
    public int monitorCount;

    //�G��Monitor��Material
    [SerializeField] Material[] color = new Material[5];

    //�G��Monitor��mesh
    [SerializeField] Mesh[] mesh = new Mesh[5];

    //�G����MeshRenderer
    private MeshRenderer meshRenderer;

    //�G����Meshfilter
    private MeshFilter meshFilter;

    //�S�[���h�G�o���m��
    private int goldEnemyProbability = 1;

    //�����Ŋl������R�C��
    public int _currentCoin;

    [SerializeField] private GameInformation gameInformation;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private AudioClip clip;
    [SerializeField] private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();  
        _currentCoin = 0;
    }

    //Awake�֐��ŃI�u�W�F�N�g�v�[���̏�����
    private void Awake()
    {
        //�C���X�^���X��
        if(instance == null)
        {
            instance = this;
        }

        //2�̖ڂ̓G����J�E���g
        monitorCount = 1;

        objectPool = new List<GameObject>();

        //initialSize���ŏ��ɐ���
        for(int i = 0;i < initialSize; i++)
        {
            GameObject obj = Instantiate(prefab,firstPos,Quaternion.identity);

            meshRenderer = obj.transform.GetChild(0).GetComponent<MeshRenderer>();
            meshFilter = obj.transform.GetChild(0).GetComponent<MeshFilter>();

            int monitorNum = Random.Range(0, 5);
            meshRenderer.material = color[monitorNum];
            meshFilter.mesh = mesh[monitorNum];

            if(monitorNum == 4)
            {
                //4 => �S�[���h�G
                //�S�[���h�G�̃^�O�ɕύX
                obj.tag = "Gold";
            }
            else
            {
                obj.tag = "Normal";
            }

            //�ŏ��̂ЂƂ����\��
            if (i == 0)
            {
                obj.SetActive(true);
            }
            else
            {
                obj.SetActive(false);
            }

            //���X�g�Ƀ��j�^�[��ǉ�
            objectPool.Add(obj);
        }
    }

    //�I�u�W�F�N�g���v�[������擾
    private GameObject GetObjectFromPool()
    {
        for(int i = 0;i < objectPool.Count; i++)
        {
            //��\���ɂȂ��Ă�����̂�T���A��������ė��p
            if (!objectPool[i].activeSelf)
            {
                return objectPool[i];
            }
        }
        //��\���ɂȂ��Ă�����̂��Ȃ���ΐV�������
        GameObject newObj = Instantiate(prefab);
        objectPool.Add(newObj);
        return newObj;
    }

    //����Ȃ��Ȃ����I�u�W�F�N�g���v�[���ɖ߂�
    public void ReturnObjectToPool(GameObject obj)
    {
        //��\��
        obj.SetActive(false);

        //�|�����^�C�~���O�ŃR�C�����Z
        CoinCalculation(obj);

        if (monitorCount > 19 - 1)
        {
            // �w���՗p���[�h
            // ���{�X���Ȃ��Ń��X�{�X�֑J��
            if (gameManager.FestivalMode)
            {
                gameManager.SetState(GameManager.STATE.LAST_BOSS);
                return;
            }

            //�{�X�֏�ԑJ��
            if (gameInformation.progress == 3)
                gameManager.SetState(GameManager.STATE.LAST_BOSS);
            else
                gameManager.SetState(GameManager.STATE.MIDDLE_BOSS);
        }
    }

    //�I�u�W�F�N�g��V�����\��
    public void AppearanceObject()
    {
        //���̖ڂ����J�E���g
        monitorCount++;

        //19�̓|���Əo�����Ȃ��Ȃ�
        if(monitorCount > 19 - 1)
        {
            return;
        }
        //�I�u�W�F�N�g�v�[�����玟�ɕ\������I�u�W�F�N�g���擾
        GameObject newObj = GetObjectFromPool();

        //�I�u�W�F�N�g�̍��W��������
        newObj.transform.position = firstPos;

        //�F�A�`���ύX
        meshRenderer = newObj.transform.GetChild(0).GetComponent<MeshRenderer>();
        meshFilter = newObj.transform.GetChild(0).GetComponent<MeshFilter>();

        //�S�[���h�G�̊m�����v�Z
        goldEnemyProbability = GoldEnemyProbabilityCalculation(gameInformation.goldEnemyProbabilityLevel);

        //�m���ŃS�[���h�G���o��
        if (Random.Range(0,10) < goldEnemyProbability)
        {
            //4�Ԗڂ̃}�e���A�����S�[���h
            //�}�e���A�����S�[���h�ɕύX
            meshRenderer.material = color[4];

            //�S�[���h�G�̌`��ɕύX
            meshFilter.mesh = mesh[4];

            //�S�[���h�G�p�^�O�ɕύX
            newObj.tag = "Gold";
        }
        else
        {
            //�ʏ�̃��j�^�[�̐F�������_���ŕύX
            int monitorNum = Random.Range(0, 4);
            meshRenderer.material = color[monitorNum];
            meshFilter.mesh = mesh[monitorNum];

            //�m�[�}���^�O�ɕύX
            newObj.tag = "Normal";
        }

        //�I�u�W�F�N�g��\��
        newObj.SetActive(true);

        //SE���Đ�
        audioSource.PlayOneShot(clip);
    }

    private void CoinCalculation(GameObject monitor)
    {
        if (monitor.CompareTag("Normal"))
        {
            _currentCoin += CoinMagnificationCalculation(gameInformation.coinUpLevel);
        }
        else if (monitor.CompareTag("Gold"))
        {
            //�S�[���h�G�͊l����2�{
            _currentCoin += CoinMagnificationCalculation(gameInformation.coinUpLevel) * 2;
        }
    }

    //�S�[���h�G�̊m�����v�Z
    private int GoldEnemyProbabilityCalculation(int level)
    {
        //���x���ʂɊm�����v�Z
        switch (level)
        {
            case 1:
                goldEnemyProbability = 1;
                break;
            case 2:
                goldEnemyProbability = 2;
                break;
            case 3:
                goldEnemyProbability = 3;
                break;
            case 4:
                goldEnemyProbability = 4;
                break;
            case 5:
                goldEnemyProbability = 5;
                break;
        }

        return goldEnemyProbability;
    }

    //���x���ʂ̃R�C���l���ʌv�Z
    private int CoinMagnificationCalculation(int level)
    {
        float coin = 10;

        //�ŏ��̔{���͓��{
        float mag = 1;
        for (int i = 1; i < level; i++) 
        {
            mag += 0.5f;
        }
        return (int)(coin * mag);
    }
}
