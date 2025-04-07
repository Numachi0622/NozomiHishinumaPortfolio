namespace Interface
{
    public interface IComboAttackable : IAttackable
    {
        public void ComboCount();

        public void AttackEndPerCombo();
    }
}