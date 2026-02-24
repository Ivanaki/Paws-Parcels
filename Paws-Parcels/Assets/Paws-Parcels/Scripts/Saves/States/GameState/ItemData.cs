using System;
using UnityEngine;

namespace Paws_Parcels.Scripts.Saves.States.GameState
{
    [Serializable]
    public class ItemData
    {
        public string ItemTypeId;
        public string MarkId;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 LinearVelocity;
        public Vector3 AngularVelocity;
    }
}