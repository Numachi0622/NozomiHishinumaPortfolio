using UnityEngine;

// 客を選出する機能を持ったクラス
public class CustomerSelect : MonoBehaviour
{
    [SerializeField] private GameObject[] customerPrefab = new GameObject[5]; // 5種類の客のPrefab
    private GameObject customer; // 生成する客オブジェクト
    private int customerId; // どの客かを識別する変数

    // ゲーム開始時に客を選出するメソッド
    public void Select()
    {
        customerId = Random.Range(0, customerPrefab.Length);
        customer = Instantiate(customerPrefab[customerId], transform.position, Quaternion.Euler(0,-90,0));
    }

    // GameManagerのEventTriggerから呼ばれるメソッド
    public void EatAnimationEvent()
    {
        customer.GetComponent<Customer>()?.EatAnimation();
    }

}
