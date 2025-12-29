using UnityEngine;

namespace MotorHome.Scripts
{
    [System.Serializable]
    public class DialogData
    {
        [field: SerializeField] public DialogEnum Dialog { get; private set; }
        [field: SerializeField] public PhrasesOneManData[] Phrases { get; private set; }
    }
}