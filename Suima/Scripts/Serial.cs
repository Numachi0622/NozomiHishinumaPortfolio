using UnityEngine;
using System.IO.Ports;
using System;
using DG.Tweening;
using UniRx;
using UnityEngine.Events;

public class Serial : MonoBehaviour
{
    [SerializeField] private SerialData serialData;
    private string port;
    private int bps;
    [SerializeField] private Attacker attacker;
    private SerialPort serialPort;
    private bool[] isClose = new bool[2];
    private bool[] isStartClosed = new bool[2] { false, false };
    [SerializeField] private GameObject[] tutorialUI = new GameObject[2];
    [SerializeField] private GameManager gameManager;

    private bool isAttackable => gameManager.GetCurrentState() == GameState.Game;
    private string[] states = new string[2];
    private bool[] isClosable = new bool[2] { false, false };

    private SerialPort serial;
    private bool isLoop = true;
    private bool isStart = false;

    [SerializeField] private UnityEvent doorStartEvent, windowStartEvent, tutorialStartEvent;
    

    void Start()
    {
        port = Application.isEditor ? serialData.Port : PlayerPrefs.GetString("Port", "COM5");
        bps = serialData.Bps;
        serial = new SerialPort(port, bps, Parity.None, 8, StopBits.One);//シリアルポートを作成

        try
        {
            serial.Open();//シリアル通信を開始
            Observable.Start(() => ReadData()).Subscribe().AddTo(this);
        }
        catch (Exception e) // 例外が発生した時
        {
            Debug.Log(e);
        }

        DOVirtual.DelayedCall(1f, () =>
        {
            isStart = true;
        });
    }

    private void Update()
    {
        if (!isStart) return;

        if (serial.IsOpen)
        {
            if (isClose[0])
            {
                if (!isStartClosed[0])
                {
                    isStartClosed[0] = true;
                    doorStartEvent?.Invoke();
                    doorStartEvent = null;
                }

                if (isClosable[0]) 
                {
                    StartCoroutine(attacker.Attack(0));
                    isClosable[0] = false;
                }
            }
            else
            {
                isClosable[0] = true;
            }

            if (isClose[1])
            {
                if (!isStartClosed[1] && isClosable[1])
                {
                    isStartClosed[1] = true;
                    windowStartEvent?.Invoke();
                    windowStartEvent = null;
                }

                if (isClosable[1])
                {
                    StartCoroutine(attacker.Attack(1));
                    isClosable[1] = false;
                }
            }
            else
            {
                isClosable[1] = true;
            }

            if (isStartClosed[0] && isStartClosed[1] && !isAttackable)
            {
                foreach (var ui in tutorialUI)
                {
                    ui.SetActive(true);
                }
                tutorialStartEvent?.Invoke();
                tutorialStartEvent = null;
            }
        }
        else
        {
            // コントローラー無し、デバッグ用
            if (Input.GetKeyDown(KeyCode.A))
            {
                isClose[0] = true;
                isStartClosed[0] = true;
                doorStartEvent?.Invoke();
                doorStartEvent = null;
                StartCoroutine(attacker.Attack(0));
            }
            else
            {
                isClose[0] = false;
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                isClose[1] = true;
                isStartClosed[1] = true;
                windowStartEvent?.Invoke();
                windowStartEvent = null;
                StartCoroutine(attacker.Attack(1));
            }
            else
            {
                isClose[1] = false;
            }

            if (isStartClosed[0] && isStartClosed[1] && !isAttackable)
            {
                foreach (var ui in tutorialUI)
                {
                    ui.SetActive(true);
                }
                tutorialStartEvent?.Invoke();
                tutorialStartEvent = null;
            }
        }
    }

    public void ReadData()
    {
        while (isLoop)
        {
            string data = serial.ReadLine();

            //if (data == "1,1") continue;

            states = data.Split(",");

            if (states.Length != 2) continue;

            for (int i = 0; i < 2; i++)
            {
                int state = 1;
                int.TryParse(states[i], out state);
                isClose[i] = state == 0;
            }
        }
    }

    void OnDestroy()
    {
        isLoop = false;
        serial.Close();//シリアル通信を終了
    }
}