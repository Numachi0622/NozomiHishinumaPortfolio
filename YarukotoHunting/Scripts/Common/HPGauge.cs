using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class HPGauge : MonoBehaviour
{
    [SerializeField] private Slider hpGauge;
    [SerializeField] private Text hpText;

    // HPの値をSliderに反映
    public void SetHP(float _hp,float _maxHp)
    {
        hpGauge.maxValue = _maxHp;
        hpGauge.value = _hp;
        // プレイヤーのみ残りHPを表示
        if (hpText == null) return;
        hpText.text = hpGauge.value.ToString() + "/" + hpGauge.maxValue.ToString();
    }

    // HPゲージを減らす
    public void DecreaseHP(float _hp)
    {
        hpGauge.DOValue(_hp, 0.5f);
        // プレイヤーのみ残りHPを表示
        if (hpText == null) return;
        if (_hp <= 0) _hp = 0;
        hpText.text = _hp + "/" + hpGauge.maxValue.ToString();
    }

    // HPを回復
    public void RecoverHP(float _maxHp)
    {
        hpGauge.maxValue = _maxHp;
        hpGauge.DOValue(hpGauge.maxValue,1f);
        if (hpText == null) return;
        hpText.text = hpGauge.maxValue.ToString() + "/" + hpGauge.maxValue.ToString();
    }
}
