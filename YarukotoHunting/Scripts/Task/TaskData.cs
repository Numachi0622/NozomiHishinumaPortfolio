using System.Collections.Generic;

[System.Serializable]
public class TaskData
{
    public string name; // Taskの名前
    public string key; // Taskを判別するためのキー
    public bool isChecked; // Taskが完了しているかを判定
    public int[] registeredTime = new int[5]; // タスクを登録した時間  0:year 1:month 2:day 3:hour 4:minute
    public enum UPDATE_STATUS
    {
        NEW, // 新しく作られたタスク
        ALREADY, // 既に作られているタスク
        EDITED // 編集されたタスク
    }
    public UPDATE_STATUS status;
}

[System.Serializable]
public class TaskList
{
    public List<TaskData> taskList = new List<TaskData>(); // TaskDataをまとめるリスト
}
