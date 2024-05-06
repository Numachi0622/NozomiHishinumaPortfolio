using UnityEngine;
using UnityEngine.UI;

public class EditTask : MonoBehaviour
{
    private int maxNameLength = 27; // タスク名の最大文字数
    [SerializeField] private bool isChecked; // チェックがついているかどうか
    private GameObject taskParent; // 生成する敵の親となるオブジェクト
    private GameObject addTaskPanel;
    private GameObject messagePanel;
    private GameObject[] message = new GameObject[2];
    private TaskData data; // タスクリストと対になるTaskData
    private UIAnimation UIAnim;

    public string taskName { get; private set; } // タスク名
    [SerializeField] string testname;

    [SerializeField] private GameObject checkMark; // チェックマークのオブジェクト
    [SerializeField] private InputField editInput; // タスク名を編集する際のInputField
    [SerializeField] private Text taskNameText; // タスク名を記述するText

    private void Start()
    {
        taskName = taskNameText.text;
    }

    public void DataReset(string _taskName)
    {
        taskName = _taskName;
        // 起動時のisCheckedの値をそれぞれ取得
        isChecked = data.isChecked;
        checkMark.SetActive(isChecked);
    }

    // チェックボックスを押したときの処理
    public void OnTapCheckBox()
    {
        // bool値を反転
        isChecked = !isChecked;
        checkMark.SetActive(isChecked);
        //TaskDataのbool値を書き換える
        data.isChecked = isChecked;
        if(data.status == TaskData.UPDATE_STATUS.ALREADY) 
            data.status = TaskData.UPDATE_STATUS.EDITED;
        SaveSystem.Instance.Save();
    }

    // Editボタンが押された時の処理
    public void OnTapEditButton()
    {
        editInput.gameObject.SetActive(true);
    }

    // 編集の終了処理
    public void FinishEdit()
    {
        // 入力文字がない場合保存できない
        if (editInput.text == null) return;
        // 入力文字数が27を超えたらメッセージを出す
        if (editInput.text.Length > maxNameLength)
        {
            GetMessageObject();
            UIAnim.MessageAnimation(messagePanel, message[0]);
            return;
        }
        for (int i = 0; i < SaveSystem.Instance.TaskList.taskList.Count; i++)
        {
            // 既に登録されているタスク名を入力したらメッセージを出す
            if (editInput.text == SaveSystem.Instance.TaskList.taskList[i].name)
            {
                GetMessageObject();
                UIAnim.MessageAnimation(messagePanel, message[1]);
                return;
            }
        }

        // 入力情報からタスクの名前を書き換える
        data.name = editInput.text;
        if(data.status == TaskData.UPDATE_STATUS.ALREADY)
            data.status = TaskData.UPDATE_STATUS.EDITED;
        taskName = data.name;
        taskNameText.text = taskName;

        SaveSystem.Instance.Save();
        editInput.gameObject.SetActive(false);
    }

    // タスクを消去
    public void OnTapRemoveButton()
    {
        SaveSystem.Instance.TaskList.taskList.Remove(data);

        // 敵も同時に消滅させる
        taskParent = GameObject.FindGameObjectWithTag("EnemyGenerator");
        for(int i = 0; i < taskParent.transform.childCount; i++)
        {
            if(taskParent.transform.GetChild(i).name == taskName)
            {
                taskParent.transform.GetChild(i).gameObject.SetActive(false);
                break;
            }
        }
        editInput.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
    }

    // メッセージを出す際に必要なオブジェクトを取得する
    public void GetMessageObject()
    {
        UIAnim = transform.root.GetComponent<UIAnimation>();
        addTaskPanel = GameObject.FindGameObjectWithTag("AddTaskPanel");
        messagePanel = addTaskPanel.transform.GetChild(1).gameObject;
        for (int i = 0; i < messagePanel.transform.childCount; i++)
        {
            message[i] = messagePanel.transform.GetChild(i).gameObject;
        }
    }

    // タスクリスト更新の際にisChecked変数を書き換える
    public void SetCheck(bool _isChecked)
    {
        this.isChecked = _isChecked;
    }

    // タスクリスト更新の際にTaskDataのインスタンスを与える
    public void SetTaskData(TaskData _data)
    {
        this.data = _data;
    }
}
