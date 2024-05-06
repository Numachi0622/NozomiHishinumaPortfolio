using UnityEngine;
using UnityEngine.UI;

public class AddTask : MonoBehaviour
{
    private int maxNameLength = 27; // タスク名の最大文字数
    protected string inputName; // 入力する名前
    protected TaskManager taskManager;

    [SerializeField] private EnemyGenerator enemyGenerator;
    [SerializeField] protected UIAnimation UIAnim;
    [SerializeField] protected InputField input;
    [SerializeField] protected GameObject addTaskDisplay;
    [SerializeField] private GameObject taskParent;
    [SerializeField] protected GameObject messagePanel;
    [SerializeField] protected GameObject[] message;

    private void Awake()
    {
        taskManager = GetComponent<TaskManager>();
    }

    // 入力したタスク名を変数に一時的に代入
    public virtual void Input()
    {
        inputName = input.text;
    }

    // 追加したタスクを保存
    public virtual void Save()
    {
        // 入力文字がない場合保存できない
        if (inputName == null) return;
        // 入力文字数が27を超えたらメッセージを出す
        if(inputName.Length > maxNameLength)
        {
            UIAnim.MessageAnimation(messagePanel, message[0]);
            return;
        }
        // 既に登録されているタスク名を入力したらメッセージを出す
        for(int i = 0;i < SaveSystem.Instance.TaskList.taskList.Count;i++)
        {
            if(inputName == SaveSystem.Instance.TaskList.taskList[i].name)
            {
                UIAnim.MessageAnimation(messagePanel, message[1]);
                return;
            }
        }

        taskManager.TaskCount();
        taskManager.InitializeTask(inputName);

        SaveSystem.Instance.Save();

        // 保存したデータからタスクリストを生成
        int lastTaskNum = SaveSystem.Instance.TaskList.taskList.Count;
        taskManager.GenerateTaskList(inputName,true, false, SaveSystem.Instance.TaskList.taskList[lastTaskNum - 1]);

        input.text = null;
    }

    // タスク追加画面を表示
    public virtual void OpenAddTaskWindow()
    {
        addTaskDisplay.SetActive(true);
        Time.timeScale = 0;
        SaveSystem.Instance.Load();
        for(int i = 0;i < taskParent.transform.childCount;i++)
        {
            // 一度全てのタスクリストを非表示にする
            taskParent.transform.GetChild(i).gameObject.SetActive(false);
        }
        for (int i = 0; i < SaveSystem.Instance.TaskList.taskList.Count; i++)
        {
            // タスクリストを更新
            taskManager.GenerateTaskList(SaveSystem.Instance.TaskList.taskList[i].name,
                false,
                SaveSystem.Instance.TaskList.taskList[i].isChecked,
                SaveSystem.Instance.TaskList.taskList[i]);
        }
    }

    // タスク追加画面を非表示
    public virtual void CloseAddTaskWindow()
    {
        addTaskDisplay.SetActive(false);
        Time.timeScale = 1;

        // 戻るボタンを押したタイミングで新しく作成したタスク（敵）が生成される
        TaskData data;
        for(int i = 0;i < SaveSystem.Instance.TaskList.taskList.Count; i++)
        {
            if (SaveSystem.Instance.TaskList.taskList[i].status == TaskData.UPDATE_STATUS.NEW)
            {
                // statusがNEWのタスクは新しく敵として生成される
                data = SaveSystem.Instance.TaskList.taskList[i];
                enemyGenerator.GenerateEnemy(data.name,data.key,data.isChecked);
                data.status = TaskData.UPDATE_STATUS.ALREADY;
            }
            else if(SaveSystem.Instance.TaskList.taskList[i].status == TaskData.UPDATE_STATUS.EDITED)
            {
                // statusがEDITEDのタスクは生成されている敵の情報を書き換える
                data = SaveSystem.Instance.TaskList.taskList[i];
                for (int j = 0;j < enemyGenerator.gameObject.transform.childCount; j++)
                {
                    if (enemyGenerator.gameObject.transform.GetChild(j).GetComponent<EnemyStatus>().key == data.key)
                    {
                        enemyGenerator.gameObject.transform.GetChild(j).name = data.name;
                        EnemyStatus _status = enemyGenerator.gameObject.transform.GetChild(j).GetComponent<EnemyStatus>();
                        _status.DisplayTaskName(data.name);
                        _status.IsChecked(data.isChecked);
                        data.status = TaskData.UPDATE_STATUS.ALREADY;
                    }
                }
            }
        }
        SaveSystem.Instance.Save();
    }
}
