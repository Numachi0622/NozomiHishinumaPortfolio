using UnityEngine;

public class ThirdBossAttackManager : AttackManager
{
    [SerializeField] private Transform playerTf;
    [SerializeField] CoinGenerator coinGenerator;

    public void CoinAttack()
    {
        StartCoroutine(coinGenerator.GenerateCoin(playerTf));
    }

}
