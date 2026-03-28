using R3;
using UnityEngine;

namespace Quests
{
    public class InWorldPointDestination : DestinationObject
    {
        [SerializeField] private string _destinationId;
        [SerializeField] private string _uiShowTypeId;
        
        public override string DestinationId => _destinationId;
        public override string UiShowTypeId => _uiShowTypeId;
    }
}