using System.Collections.Generic;
using Paws_Parcels.Extentions;
using Paws_Parcels.Scripts.Saves.States;
using R3;
using UnityEngine;
using Zenject;

namespace Paws_Parcels.Car.View.Sound
{
    public class CarHitSound : MonoDisposable
    {
        [SerializeField]
        private AudioSource m_AudioSource;

        [SerializeField]
        private List<GameObject> m_IgnoredObjects = new();

        [Header("Settings")]
        [SerializeField]
        [Range(0.0f, 3.0f)]
        private float m_PitchMin;

        [SerializeField]
        [Range(0.0f, 3.0f)]
        private float m_PitchMax;

        [SerializeField]
        [Min(0.0f)]
        private float m_MaxHitVelocity = 25.0f;

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float m_BottomHitDotProductThreshold = 0.8f;

        private ReadOnlyReactiveProperty<float> _masterVolume;
        private float _hitVelocityNormalized;
        
        [Inject]
        private void Construct(SettingsStateProvider settingsStateProvider)
        {
            _masterVolume = settingsStateProvider.SettingsProxy.MasterVolume;
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();

            _disposable.Add(_masterVolume.Subscribe(_ =>
                m_AudioSource.volume = _hitVelocityNormalized * _masterVolume.CurrentValue));
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (m_IgnoredObjects.Contains(collision.gameObject))
            {
                return;
            }

            foreach (ContactPoint contact in collision.contacts)
            {
                bool isBottomContact = Vector3.Dot(transform.up, contact.normal) > m_BottomHitDotProductThreshold;

                if (isBottomContact)
                {
                    continue;
                }

                if (!m_AudioSource.isPlaying)
                {
                    float hitVelocityNormalized = collision.relativeVelocity.magnitude / m_MaxHitVelocity;
                    m_AudioSource.volume = hitVelocityNormalized * _masterVolume.CurrentValue;
                    m_AudioSource.pitch = Mathf.Lerp(m_PitchMin, m_PitchMax, hitVelocityNormalized);
                    m_AudioSource.Play();
                    
                    _hitVelocityNormalized = hitVelocityNormalized;
                }
            }
        }

        public void AddToIgnored(GameObject go)
        {
            m_IgnoredObjects.Add(go);
        }

        public void RemoveFromIgnored(GameObject go)
        {
            m_IgnoredObjects.Remove(go);
        }
    }
}
