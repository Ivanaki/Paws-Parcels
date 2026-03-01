using UnityEngine;

namespace Quests
{
    public class InWorldPointDestination : DestinationObject
    {
        [SerializeField] private string _destinationId;
        [SerializeField] private string _uiShowTypeId;
        [SerializeField] private string _uiShowText;

        public override string DestinationId => _destinationId;
        public override string UiShowTypeId => _uiShowTypeId;
        public override string UiShowText => _uiShowText;
    }
}