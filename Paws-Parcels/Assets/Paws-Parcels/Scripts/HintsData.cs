using System.Collections.Generic;
using Car_Player;
using UnityEngine;

namespace DefaultNamespace
{
    public class HintsData : MonoBehaviour
    {
        public static HintsData instance { get; private set; }

        [field: SerializeField] public List<HintData> Hints { get; private set; }
        
        
        
        
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(this);
            }
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }
    }
}