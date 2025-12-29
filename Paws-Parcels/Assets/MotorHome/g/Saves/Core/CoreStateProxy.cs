using R3;
using UnityEngine;

namespace MotorHome.Scripts.Saves.Core
{
    public class CoreStateProxy
    {
        public ReactiveProperty<int> Progress { get; }
        
        public CarPlayerSwitcher.CarOrPlayerState CarOrPlayerState;
        
        public Vector3 CarPosition;
        public Quaternion CarRotation;
        public Vector3 CarVelocity;
        public Vector3 CarAngularVelocity;
        
        public Vector3 PlayerPosition;
        public Quaternion PlayerRotation;
        public Vector3 PlayerVelocity;

        public CoreStateProxy(CoreState coreState)
        {
            Progress = new ReactiveProperty<int>(coreState.Progress);
            
            Progress.Skip(1).Subscribe(value => coreState.Progress = value);
        }
    }
}