using UnityEngine;

namespace Paws_Parcels.Scripts.CarMovement.Ramming
{
    public interface IRammable
    {
        float MinImpactSpeed { get; }
        void OnRammed(float impactSpeed, Vector3 impactDirection);
    }
}
