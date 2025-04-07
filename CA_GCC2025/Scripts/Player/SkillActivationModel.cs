using System;
using UnityEngine;

public class SkillActivationModel : MonoBehaviour
{
    /// <summary>
    /// スキルの処理を格納
    /// </summary>
    public Action OnSkillActivate;

    /// <summary>
    /// 登録されたをスキル発動
    /// </summary>
    public void SkillActivate()
    {
        OnSkillActivate?.Invoke();
    }
}