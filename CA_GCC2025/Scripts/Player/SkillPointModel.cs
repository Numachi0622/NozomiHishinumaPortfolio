using System.Collections;
using System.Collections.Generic;
using R3;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Utility;

public class SkillPointModel
{
    /// <summary>
    /// スキルポイント
    /// </summary>
    private ReactiveProperty<float> _sp;
    public ReadOnlyReactiveProperty<float> Sp => _sp;

    /// <summary>
    /// スキルポイントのレベル
    /// </summary>
    private ReactiveProperty<int> _spLevel;
    public ReadOnlyReactiveProperty<int> SpLevel => _spLevel;
    

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="sp"></param>
    public SkillPointModel(float sp)
    {
        _sp = new ReactiveProperty<float>(sp);
        _spLevel = new ReactiveProperty<int>(Mathf.FloorToInt(sp));
    }

    /// <summary>
    /// 新たな値をセット
    /// </summary>
    /// <param name="sp">更新後のSP</param>
    public void Set(float sp)
    {
        _sp.Value = Mathf.Round(Mathf.Clamp(sp, 0, GameConst.MAX_SKILL_POINT) * 10) / 10f;
        _spLevel.Value = Mathf.FloorToInt(_sp.Value);
    }

    /// <summary>
    /// SP増加
    /// </summary>
    /// <param name="value">増加量</param>
    public void Increase(float value)
    {
        Set(_sp.Value + value);
    }

    /// <summary>
    /// SP減少
    /// </summary>
    /// <param name="value">減少量</param>
    public void Decrease(float value)
    {
        Set(_sp.Value - value);
    }
}
