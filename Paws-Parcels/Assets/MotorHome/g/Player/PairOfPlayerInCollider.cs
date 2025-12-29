namespace MotorHome.Scripts
{
    [System.Serializable]
    public class PairOfPlayerInCollider
    {
        public PlayerInCollider First;
        public PlayerInCollider Second;

        public PairOfPlayerInCollider(PlayerInCollider first, PlayerInCollider second)
        {
            First = first;
            Second = second;
        }
    }
}