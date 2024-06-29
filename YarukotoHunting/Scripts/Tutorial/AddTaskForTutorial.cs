using UnityEngine;

public class AddTaskForTutorial : AddTask
{
    private string tutorialTaskName = "チュートリアル";
    [SerializeField] private Animator animator;
    [SerializeField] private NovelTextManager novelTextManager;
    [SerializeField] private GameObject taskListForTutorial;
    [SerializeField] private GameObject tutorialEnemy;

    public override void OpenAddTaskWindow()
    {
        base.addTaskDisplay.SetActive(true);
        animator.enabled = false;
    }

    public override void Input()
    {
        if(base.input.text != tutorialTaskName)
        {
            messagePanel.SetActive(true);
            base.UIAnim.MessageAnimation(messagePanel, message[0]);
            return;
        }
        base.Input();
        novelTextManager.OnClickNextButton(null);
    }

    public override void Save()
    {
        taskListForTutorial.SetActive(true);
        base.input.text = null;
    }

    public override void CloseAddTaskWindow()
    {
        base.addTaskDisplay.SetActive(false);
        if(!tutorialEnemy.activeSelf) tutorialEnemy.SetActive(true);
    }
}

//テストコメント
