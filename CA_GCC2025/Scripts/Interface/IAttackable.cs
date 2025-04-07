
using UnityEngine;

namespace Interface
{
    public interface IAttackable
    {
        /// <summary>
        /// 攻撃
        /// </summary>
        public void Attack();

        /// <summary>
        /// 攻撃の衝撃を加える
        /// 攻撃Colliderを有効化
        /// </summary>
        /// <param name="waitTime"></param>
        public void AttackImpact(float waitTime);

        /// <summary>
        /// 攻撃を終える
        /// </summary>
        public void AttackEnd();
    }
}