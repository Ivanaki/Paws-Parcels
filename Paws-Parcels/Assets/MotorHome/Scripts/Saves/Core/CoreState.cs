using UnityEngine;

namespace MotorHome.Scripts.Saves.Core
{
    [System.Serializable]
    public class CoreState
    {
        public int Progress;
        
        public CarPlayerSwitcher.CarOrPlayerState CarOrPlayerState;
        
        public Vector3DTO CarPosition;
        public QuaternionDTO CarRotation;
        public Vector3DTO CarVelocity;
        public Vector3DTO CarAngularVelocity;
        
        public Vector3DTO PlayerPosition;
        public QuaternionDTO PlayerRotation;
        //public Vector3DTO PlayerVelocity;
        public float PlayerCameraRotationYaw;
        public float PlayerCameraRotationPitch;

        public bool IsDoorsInCarOpen;

        public ItemData[] Items;
        
        public QuestEnum[] Quests;

        public bool IsCompassActivate;

        public float GasValue;
        public int Money;
        
        public QuestEnum CompassSelected;

        public bool IsCutSceneIsPlayed;
 
        public CoreState() { }

        public CoreState(CoreState copyState)
        {
            Progress = copyState.Progress;
            CarOrPlayerState = copyState.CarOrPlayerState;
            CarPosition = copyState.CarPosition;
            CarRotation = copyState.CarRotation;
            CarVelocity = copyState.CarVelocity;
            CarAngularVelocity = copyState.CarAngularVelocity;
            PlayerPosition = copyState.PlayerPosition;
            PlayerRotation = copyState.PlayerRotation;
            //PlayerVelocity = copyState.PlayerVelocity;
            PlayerCameraRotationYaw = copyState.PlayerCameraRotationYaw;
            PlayerCameraRotationPitch = copyState.PlayerCameraRotationPitch;
            IsDoorsInCarOpen = copyState.IsDoorsInCarOpen;
            Items = copyState.Items;
            Quests = copyState.Quests;
            IsCutSceneIsPlayed = copyState.IsCutSceneIsPlayed;
        }
    }
}