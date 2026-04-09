using System;
using System.Collections.Generic;
using System.Linq;
using Paws_Parcels.NPCDialogs;
using Paws_Parcels.Scripts.Saves.States;
using R3;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using TMPro;
using Zenject;

namespace UI.DialogueScreen
{
    [Serializable]
    public class CharacterPanel
    {
        public GameObject root;
        public TMP_Text characterNameText;
        public TMP_Text lineText;
        public Image portrait;
    }

    public class CustomLinePresenter : DialoguePresenterBase
    {
        [Header("Panels")]
        [SerializeField] private CharacterPanel playerPanel;
        [SerializeField] private CharacterPanel npcPanel;

        [Header("Shared UI")]
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Character Data")]
        [SerializeField] private List<CharacterPortraitData> characters;

        [Header("Typewriter")]
        [SerializeField] private int lettersPerSecond = 60;
        [SerializeField] private List<ActionMarkupHandler> eventHandlers = new();

        [Header("Mouth Animation")]
        [SerializeField] private float mouthToggleInterval = 0.08f;
        [SerializeField] private float mouthInertiaTime = 0.3f;

        [Header("Talk Audio")]
        [SerializeField] private AudioClip sharedTalkClip;
        [SerializeField] private AudioClip playerTalkClip;
        [SerializeField] private AudioClip npcTalkClip;
        [SerializeField, Range(0f, 1f)] private float talkVolume = 1f;
        [SerializeField] private bool loopTalkAudio = true;

        [Header("Portrait Sway")]
        [SerializeField] private float swayAngle = 3f;
        [SerializeField] private float swaySpeed = 5f;
        [SerializeField] private float pulseAmount = 0.05f;
        [SerializeField] private float pulseSpeed = 4f;

        [Header("Fade")]
        [SerializeField] private bool useFadeEffect = true;
        [SerializeField] private float fadeUpDuration = 0.25f;
        [SerializeField] private float fadeDownDuration = 0.1f;

        [Header("Auto Advance")]
        [SerializeField] private bool autoAdvance = false;
        [SerializeField] private float autoAdvanceDelay = 1f;

        private CharacterPortraitData _currentCharacter;
        private CharacterPanel _activePanel;
        private string _currentEmotion = "idle";
        private bool _isTyping;
        private float _mouthTimer;
        private bool _mouthOpen;
        private bool _isVisible;
        private AudioSource _playerTalkSource;
        private AudioSource _npcTalkSource;
        private AudioSource _currentTalkSource;
        private ReadOnlyReactiveProperty<float> _masterVolume;

        private List<IActionMarkupHandler> ActionMarkupHandlers
        {
            get
            {
                var pauser = new PauseEventProcessor();
                var handlers = new List<IActionMarkupHandler> { pauser };
                handlers.AddRange(eventHandlers);
                return handlers;
            }
        }

        [Inject]
        private void Construct([InjectOptional] SettingsStateProvider settingsStateProvider)
        {
            _masterVolume = settingsStateProvider?.SettingsProxy?.MasterVolume;
            if (_masterVolume != null)
            {
                _masterVolume.Subscribe(_ => ApplyTalkAudioVolume()).AddTo(this);
            }
        }

        private void Awake()
        {
            if (canvasGroup != null) canvasGroup.alpha = 0;
            playerPanel.root.SetActive(false);
            npcPanel.root.SetActive(false);

            Typewriter = new LetterTypewriter
            {
                ActionMarkupHandlers = ActionMarkupHandlers,
                CharactersPerSecond = lettersPerSecond,
            };

            _playerTalkSource = EnsureTalkAudioSource(playerPanel);
            _npcTalkSource = EnsureTalkAudioSource(npcPanel);
            ApplyTalkAudioVolume();
        }

        private void Update()
        {
            if (_activePanel?.portrait == null) return;

            if (_isTyping && _currentCharacter != null)
            {
                _mouthTimer += Time.deltaTime;
                if (_mouthTimer >= mouthToggleInterval)
                {
                    _mouthTimer = 0f;
                    _mouthOpen = !_mouthOpen;

                    var emotion = GetCurrentEmotion();
                    if (emotion != null)
                    {
                        _activePanel.portrait.sprite = _mouthOpen ? emotion.mouthOpen : emotion.mouthClosed;
                    }
                }

                float z = Mathf.Sin(Time.time * swaySpeed) * swayAngle;
                _activePanel.portrait.rectTransform.localRotation = Quaternion.Euler(0f, 0f, z);

                float scale = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
                _activePanel.portrait.rectTransform.localScale = new Vector3(scale, scale, 1f);
            }
            else
            {
                var rt = _activePanel.portrait.rectTransform;
                float t = Time.deltaTime * swaySpeed;
                rt.localRotation = Quaternion.Lerp(rt.localRotation, Quaternion.identity, t);
                rt.localScale = Vector3.Lerp(rt.localScale, Vector3.one, t);
            }
        }

        public override async YarnTask RunLineAsync(LocalizedLine line, LineCancellationToken token)
        {
            string charName = line.CharacterName;
            _currentCharacter = characters.FirstOrDefault(c =>
                c.localizedNames != null && c.localizedNames.Contains(charName));

            if (_currentCharacter != null && _currentCharacter.isPlayer)
            {
                _activePanel = playerPanel;
                playerPanel.root.SetActive(true);
                npcPanel.root.SetActive(false);
            }
            else
            {
                _activePanel = npcPanel;
                npcPanel.root.SetActive(true);
                playerPanel.root.SetActive(false);
            }

            if (_activePanel.characterNameText != null)
            {
                _activePanel.characterNameText.text = charName ?? "";
            }

            SetMouthClosed();

            var text = line.TextWithoutCharacterName;

            Typewriter ??= new LetterTypewriter
            {
                ActionMarkupHandlers = ActionMarkupHandlers,
                CharactersPerSecond = lettersPerSecond,
            };
            Typewriter.TextElement = _activePanel.lineText;
            Typewriter.PrepareForContent(text);

            if (!_isVisible && canvasGroup != null)
            {
                if (useFadeEffect)
                    await Effects.FadeAlphaAsync(canvasGroup, 0, 1, fadeUpDuration, token.HurryUpToken);
                else
                    canvasGroup.alpha = 1;
                _isVisible = true;
            }

            _isTyping = true;
            _mouthTimer = 0f;
            _mouthOpen = false;
            StartTalkingAudio();
            await Typewriter.RunTypewriter(text, token.HurryUpToken).SuppressCancellationThrow();

            if (mouthInertiaTime > 0f)
                await YarnTask.Delay((int)(mouthInertiaTime * 1000), token.HurryUpToken).SuppressCancellationThrow();

            _isTyping = false;
            StopTalkingAudio();
            SetMouthClosed();

            if (autoAdvance)
                await YarnTask.Delay((int)(autoAdvanceDelay * 1000), token.NextContentToken).SuppressCancellationThrow();
            else
                await YarnTask.WaitUntilCanceled(token.NextContentToken).SuppressCancellationThrow();

            Typewriter.ContentWillDismiss();
            Typewriter.ContentDidDismiss();
        }

        public override YarnTask OnDialogueStartedAsync()
        {
            if (canvasGroup != null) canvasGroup.alpha = 0;
            playerPanel.root.SetActive(false);
            npcPanel.root.SetActive(false);
            _currentEmotion = "idle";
            _isVisible = false;
            StopTalkingAudio();
            return YarnTask.CompletedTask;
        }

        public override async YarnTask OnDialogueCompleteAsync()
        {
            if (canvasGroup != null)
            {
                if (useFadeEffect)
                    await Effects.FadeAlphaAsync(canvasGroup, 1, 0, fadeDownDuration, default);
                else
                    canvasGroup.alpha = 0;
            }
            playerPanel.root.SetActive(false);
            npcPanel.root.SetActive(false);
            _isVisible = false;
            StopTalkingAudio();
        }

        public void SetEmotion(string emotion)
        {
            _currentEmotion = emotion;
            SetMouthClosed();
        }

        private void SetMouthClosed()
        {
            if (_activePanel?.portrait == null || _currentCharacter == null) return;

            var emotion = GetCurrentEmotion();
            if (emotion != null)
            {
                _activePanel.portrait.sprite = emotion.mouthClosed;
            }
        }

        private EmotionSprites GetCurrentEmotion()
        {
            if (_currentCharacter == null) return null;
            return _currentCharacter.emotions?.FirstOrDefault(e => e.emotionName == _currentEmotion);
        }

        private AudioSource EnsureTalkAudioSource(CharacterPanel panel)
        {
            if (panel?.root == null)
            {
                return null;
            }

            var audioSource = panel.root.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = panel.root.AddComponent<AudioSource>();
            }

            audioSource.playOnAwake = false;
            audioSource.loop = loopTalkAudio;
            audioSource.spatialBlend = 0f;
            ApplyTalkAudioVolume(audioSource);

            return audioSource;
        }

        private void StartTalkingAudio()
        {
            if (_currentCharacter == null)
            {
                StopTalkingAudio();
                return;
            }

            var audioSource = _currentCharacter != null && _currentCharacter.isPlayer
                ? _playerTalkSource
                : _npcTalkSource;

            var talkClip = ResolveTalkClip();
            if (audioSource == null || talkClip == null)
            {
                StopTalkingAudio();
                return;
            }

            if (_currentTalkSource != null && _currentTalkSource != audioSource)
            {
                _currentTalkSource.Stop();
                if (_currentTalkSource.clip != null)
                {
                    _currentTalkSource.time = 0f;
                }
            }

            audioSource.loop = loopTalkAudio;
            ApplyTalkAudioVolume(audioSource);
            audioSource.clip = talkClip;
            audioSource.Stop();
            if (audioSource.clip != null)
            {
                audioSource.time = 0f;
            }
            audioSource.Play();

            _currentTalkSource = audioSource;
        }

        private void StopTalkingAudio()
        {
            StopTalkSource(_playerTalkSource);
            StopTalkSource(_npcTalkSource);
            _currentTalkSource = null;
        }

        private AudioClip ResolveTalkClip()
        {
            if (_currentCharacter != null && _currentCharacter.isPlayer)
            {
                return playerTalkClip != null ? playerTalkClip : sharedTalkClip;
            }

            return npcTalkClip != null ? npcTalkClip : sharedTalkClip;
        }

        private static void StopTalkSource(AudioSource audioSource)
        {
            if (audioSource == null)
            {
                return;
            }

            audioSource.Stop();
            if (audioSource.clip != null)
            {
                audioSource.time = 0f;
            }
        }

        private void ApplyTalkAudioVolume()
        {
            ApplyTalkAudioVolume(_playerTalkSource);
            ApplyTalkAudioVolume(_npcTalkSource);
        }

        private void ApplyTalkAudioVolume(AudioSource audioSource)
        {
            if (audioSource == null)
            {
                return;
            }

            audioSource.volume = talkVolume * (_masterVolume?.CurrentValue ?? 1f);
        }
    }
}
