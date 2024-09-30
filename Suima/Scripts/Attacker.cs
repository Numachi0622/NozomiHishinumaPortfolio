using System.Collections;
using UnityEngine;

public class Attacker : MonoBehaviour
{
    private bool isAttackable = true;
    [SerializeField] private Collider[] attackCollider = new Collider[2];
    private WaitForSeconds colliderActiveTime = new WaitForSeconds(0.05f);
    private WaitForSeconds coolTime = new WaitForSeconds(0.4f);

    public IEnumerator Attack(int index)
    {
        if (!isAttackable) yield break;
        if (attackCollider[index].enabled) yield break;
        attackCollider[index].enabled = true;
        yield return colliderActiveTime;
        attackCollider[index].enabled = false;

        isAttackable = false;
        yield return coolTime;
        isAttackable = true;
    }
}
