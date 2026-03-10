using UnityEngine;

namespace Paws_Parcels.Car.Logic
{
    public class CarSettings : MonoBehaviour
    {
        [Header("AXLES")]
        
        [Tooltip("(Z-axis): distance between front and rear axles.")]
        [Min(0.1f)] public float Wheelbase = 2.5f;

        [Tooltip("(X-axis): distance between left and right wheels on an axle.")]
        [Min(0.1f)] public float TrackWidth = 1.5f;

        [Tooltip("(Y-axis): position of the axles relative to the car's center. Usually negative (below center).")]
        public float AxleHeight = -0.35f;
        
        [Header("FORCES OFFSET")]
        
        [Tooltip("Offset for drive/brake forces. Adds forward/back tilt.")]
        [Min(0.0f)] public float DriveForcesOffset = 0.2f;
        
        [Tooltip("Offset for grip forces. Adds side tilt.")]
        [Min(0.0f)] public float GripForcesOffset = 0.2f;
        
        [Header("SUSPENSION")] 
        
        [Tooltip("Target spring length at equilibrium.")]
        [Min(0.0f)] public float SpringRestLength = 0.5f;
        
        [Tooltip("Max extension while grounded. Must be >= rest length.")]
        [Min(0.0f)] public float SpringMaxLength = 0.6f;
        
        [Tooltip("Spring strength. Try ~10x the car mass.")]
        [Min(0.0f)] public float SpringConstant = 1000.0f;
        
        [Tooltip("Damping strength. Controls oscillation and bounce.")]
        [Min(0.0f)] public float SpringDamperConstant = 100.0f;
        
        [Header("GROUNDING")]
        
        [Tooltip("Fallback material when no special ground is detected.")]
        public PhysicsMaterial DefaultGroundMaterial;
        
        [Header("ENGINE")]
        
        [Tooltip("Strength accelerating forward.")]
        [Min(0.0f)] public float DriveStrength = 500.0f;
        
        [Tooltip("Strength accelerating backwards.")]
        [Min(0.0f)] public float ReverseDriveStrength = 500.0f;
        
        [Tooltip("Strength when braking.")]
        [Min(0.0f)] public float BrakeStrength = 500.0f;
        
        [Tooltip("Forward speed cap in m/s.")]
        [Min(0.0f)] public float MaxSpeed = 25.0f;
        
        [Tooltip("Reverse speed cap in m/s.")]
        [Min(0.0f)] public float ReverseMaxSpeed = 20.0f;
        
        [Tooltip("Constant deceleration in m/s² simulating rolling resistance. Higher = stops faster.")]
        [Min(0.0f)] public float RollingResistanceStrength = 3.0f;
        
        [Header("STEERING")] 
        
        [Tooltip("Turning circle radius at low speed.")]
        [Min(0.0f)] public float TurnRadiusMin = 7.0f;
        
        [Tooltip("Turning circle radius at max speed (bigger at high speed).")]
        [Min(0.0f)] public float TurnRadiusMax = 14.0f;
        
        [Tooltip("Slope angle where lateral compensation from gravity is full.")]
        [Range(0.0f, 90.0f)] public float FullCompensationSlopeAngle = 20.0f;
        
        [Tooltip("Grip at low speed (1 = max, 0 = none).")]
        [Range(0.0f, 1.0f)] public float GripMin = 0.25f;
        
        [Tooltip("Grip at max speed (lower = more sliding).")]
        [Range(0.0f, 1.0f)] public float GripMax = 0.15f; 

        [Tooltip("Smoothing time for grip transitions.")]
        [Range(0.0001f, 1.0f)] public float GripSmoothTime = 0.2f;
        
        [Header("HANDBRAKE")]
        
        [Tooltip("Grip multiplier for the leading axle (throttle direction).")]
        [Range(0.0f, 1.0f)] public float HandbrakeLeadingGrip = 0.3f;

        [Tooltip("Grip multiplier for the trailing axle (opposite direction).")]
        [Range(0.0f, 1.0f)] public float HandbrakeTrailingGrip = 0.2f;

        [Tooltip("Traction multiplier for the leading axle (throttle direction).")]
        [Range(0.0f, 1.0f)] public float HandbrakeLeadingTraction = 0.8f;

        [Tooltip("Traction multiplier for the trailing axle (opposite direction).")]
        [Range(0.0f, 1.0f)] public float HandbrakeTrailingTraction = 0.7f;
        
        public float GetTurnRadius(float speedNormalized)
        {
            return Mathf.Lerp(TurnRadiusMin, TurnRadiusMax, speedNormalized);
        }

        public float GetGrip(float speedNormalized)
        {
            return Mathf.Lerp(GripMin, GripMax, speedNormalized);
        }
    }
}
