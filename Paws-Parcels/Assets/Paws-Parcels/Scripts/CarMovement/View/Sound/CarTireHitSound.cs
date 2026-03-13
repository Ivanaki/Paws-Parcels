using Paws_Parcels.Extentions;
using Paws_Parcels.Scripts.Saves.States;
using R3;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Paws_Parcels.Car.View.Sound
{
    public class CarTireHitSound : MonoDisposable
    {
        [SerializeField]
        private AudioSource m_AudioSource;

        [Header("Settings")]
        [SerializeField]
        [Min(0.0f)]
        private float m_HitMin;

        [SerializeField]
        [Min(0.0f)]
        private float m_HitMax;

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float m_VolumeMin;

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float m_VolumeMax;

        [SerializeField]
        [Range(0.0f, 3.0f)]
        private float m_PitchMin;

        [SerializeField]
        [Range(0.0f, 3.0f)]
        private float m_PitchMax;

        [SerializeField]
        [Min(0.0f)]
        private float m_MinReplayTime;

        [Header("View State")] 
        [SerializeField]
        private CarViewState m_ViewState;
        
        [SerializeField]
        private int m_WheelIndex;

        private ReadOnlyReactiveProperty<float> _masterVolume;
        
        [Inject]
        private void Construct(SettingsStateProvider settingsStateProvider)
        {
            _masterVolume = settingsStateProvider.SettingsProxy.MasterVolume;
        }

        private float _last;
        
        protected override void OnEnable()
        {
            base.OnEnable();

            _disposable.Add(_masterVolume.Subscribe(_ =>
            {
                m_AudioSource.volume = _last * _masterVolume.CurrentValue;
            }));
        }
        
        private void Start()
        {
            if (!m_ViewState)
            {
                Debug.LogError($"[{nameof(CarTireHitSound)}] Missing reference to ViewState. Assign it in edit mode.");
            }
        }

        private void Update()
        {
            if (!m_ViewState) return;
            
            float springVelocity = m_ViewState.Springs[m_WheelIndex].Velocity;
            
            if (springVelocity < -m_HitMin)
            {
                float hitVelocity = -springVelocity;
                float hitNormalized = Mathf.Clamp01(hitVelocity / m_HitMax);

                if (!m_AudioSource.isPlaying || m_AudioSource.time > m_MinReplayTime)
                {
                    _last = Mathf.Lerp(m_VolumeMin, m_VolumeMax, hitNormalized);
                    m_AudioSource.volume = _last * _masterVolume.CurrentValue;
                    m_AudioSource.pitch = Random.Range(m_PitchMin, m_PitchMax);
                    m_AudioSource.Play();
                }
            }
        }
    }
}
