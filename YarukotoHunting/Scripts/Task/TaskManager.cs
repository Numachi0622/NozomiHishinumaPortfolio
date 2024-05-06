using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskManager : MonoBehaviour
{
    private int count; // タスクが何回追加されたかをカウント
    private List<GameObject> pool = new List<GameObject>(); // オブジェクトプールで使用するプール

    [SerializeField] private EnemyGenerator enemyGenerator;
    [SerializeField] private GameObject taskList; // タスクリストのPrefab
    [SerializeField] private GameObject taskParent; // タスクリストの親オブジェクト

    private void Start()
    {
        count = PlayerPrefs.GetInt("Count",0);

        // 起動時にロード
        TaskData data;
        SaveSystem.Instance.Load();
        for(int i = 0;i < SaveSystem.Instance.TaskList.taskList.Count;i++)
        {
            // 既に保存してあるタスクリストを作成
            GenerateTaskList(SaveSystem.Instance.TaskList.taskList[i].name, false, SaveSystem.Instance.TaskList.taskList[i].isChecked, SaveSystem.Instance.TaskList.taskList[i]);

            // 既に保存してあるタスク（敵）を生成
            if (SaveSystem.Instance.TaskList.taskList[i].status == TaskData.UPDATE_STATUS.ALREADY)
            {
                data = SaveSystem.Instance.TaskList.taskList[i];
                enemyGenerator.GenerateEnemy(data.name,data.key,data.isChecked);
            }
        }
    }

    // 入力情報からタスクのパラメータを初期化
    public void InitializeTask(string _name)
    {
        TaskData data = new TaskData();

        // データクラスのパラメータを初期化
        data.name = _name;
        data.key = "Task" + count.ToString();
        data.isChecked = false;
        data.status = TaskData.UPDATE_STATUS.NEW;

        // 現在時刻を取得し、初期化
        data.registeredTime[0] = System.DateTime.Now.Year;
        data.registeredTime[1] = System.DateTime.Now.Month;
        data.registeredTime[2] = System.DateTime.Now.Day;
        data.registeredTime[3] = System.DateTime.Now.Hour;
        data.registeredTime[4] = System.DateTime.Now.Minute;

        // データクラスのオブジェクトをリストクラスへ追加
        SaveSystem.Instance.TaskList.taskList.Add(data);
    }

    // 追加されたタスク何番目のものかをカウント
    public void TaskCount()
    {
        count++;
        PlayerPrefs.SetInt("Count", count);
    }

    // 入力データを元にタスクリストを生成
    public void GenerateTaskList(string _name,bool _isFirst,bool _isChecked,TaskData _data)
    {
        GameObject _taskList = GetTaskListFromPool();
        if(!_taskList.activeSelf) _taskList.SetActive(true);
        _taskList.transform.parent = taskParent.transform;
        _taskList.transform.localScale = Vector3.one;
        // タスクリストにタスクデータを与える
        _taskList.GetComponent<EditTask>()?.SetTaskData(_data);

        // 新しく生成されたタスクリストのデータを取得
        if (_isFirst)
        {
            _taskList.GetComponent<EditTask>().DataReset(_name);
        }

        // タスクリストのタスク名を変更
        if (!_taskList.transform.GetChild(0).GetComponent<Text>()) return;
        Text name = _taskList.transform.GetChild(0).GetComponent<Text>();
        name.text = _name;

        // タスクリストのチェックマークを更新
        _taskList.GetComponent<EditTask>()?.SetCheck(_isChecked);
        _taskList.transform.GetChild(1).GetChild(0).gameObject.SetActive(_isChecked);
    }

    // タスクリストオブジェクトをプールから取得
    private GameObject GetTaskListFromPool()
    {
        for(int i = 0;i < pool.Count; i++)
        {
            // 非アクティブなものを再利用
            if (!pool[i].activeSelf) return pool[i];
        }

        // 非アクティブなものがなければ新しく生成
        GameObject newTaskList = Instantiate(taskList);
        pool.Add(newTaskList);
        return newTaskList;
    }
}
