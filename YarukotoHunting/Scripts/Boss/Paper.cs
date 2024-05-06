using UnityEngine;
using DG.Tweening;

public class Paper : MonoBehaviour
{
    private Transform tf;
    private float fireRange = 30f;
    private float toPlayerDamage = 50f;
    private float toEnemyDamage = 500f;

    private void Awake()
    {
        tf = transform;
    }

    public void Fire(Transform _destTf, Transform _originTf)
    {
        Vector3 dest = _originTf.position + (_destTf.position - _originTf.position).normalized * fireRange;
        dest.y = _originTf.position.y;
        tf.DOMove(dest, 3f).OnComplete(() => gameObject.SetActive(false));
    }

    private void OnTriggerEnter(Collider other)
    {
        gameObject.SetActive(false);
        tf.DOKill();
        StatusManager status;
        if (other.transform.parent.GetComponent<StatusManager>())
            status = other.transform.parent.GetComponent<StatusManager>();
        else
        {
            status = null;
            return;
        }

        if (other.CompareTag("Enemy"))
        {
            status.SetDamage(toEnemyDamage);
            status.ReceiveDamage();
            return;
        }
        status.SetDamage(toPlayerDamage);
        status.ReceiveDamage();
    }
}
