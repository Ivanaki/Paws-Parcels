using UnityEngine;

namespace Paws_Parcels.NPCDialogs
{
    [CreateAssetMenu(fileName = "CharacterPortrait", menuName = "Dialogue/Character Portrait")]
    public class CharacterPortraitData : ScriptableObject
    {
        [Tooltip("All localized names for this character (e.g. Заправщик, Gas Station Attendant)")]
        public string[] localizedNames;

        public bool isPlayer;
        public EmotionSprites[] emotions;
    }

    [System.Serializable]
    public class EmotionSprites
    {
        public string emotionName;
        public Sprite mouthClosed;
        public Sprite mouthOpen;
    }
}
