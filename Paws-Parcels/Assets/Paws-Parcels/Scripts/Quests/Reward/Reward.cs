using UnityEngine;

namespace Paws_Parcels.Scripts.Quests
{
    public enum RewardType
    {
        Resource = 0,
        Item = 1
    }
    
    [CreateAssetMenu(fileName = "Reward", menuName = "Paws_parcels/Reward")]
    public class Reward : ScriptableObject
    {
        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public RewardType Type { get; private set; }
        [field: SerializeField] public Texture2D Image { get; private set; }
        [field: SerializeField] public string RewardNameCode { get; private set; }
        [field: SerializeField] public string RewardDescriptionCode { get; private set; }
    }
}