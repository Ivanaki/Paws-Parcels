using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Paws_Parcels.NPCDialogs;
using Paws_Parcels.Scripts.Saves.States;
using R3;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Markup;
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
        private const string DefaultEmotion = "idle";

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
        private string _playerEmotion = DefaultEmotion;
        private string _npcEmotion = DefaultEmotion;
        private string _pendingEmotionForNextLine;
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
            var text = ResolveLineText(line);

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

            ApplyPendingEmotionForCurrentCharacter();

            if (_activePanel.characterNameText != null)
            {
                _activePanel.characterNameText.text = charName ?? "";
            }

            SetMouthClosed();

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
            await RunTypewriterSafely(line.TextID, text, token.HurryUpToken);

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
            ResetEmotions();
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
            ResetEmotions();
            StopTalkingAudio();
        }

        public void SetEmotion(string emotion)
        {
            _pendingEmotionForNextLine = NormalizeEmotion(emotion);
        }

        public void SetEmotion(string target, string emotion)
        {
            if (TryParseEmotionTarget(target, out var emotionTarget) == false)
            {
                Debug.LogWarning($"Unknown emotion target '{target}'. Use player, npc or current.");
                emotionTarget = EmotionTarget.Current;
            }

            SetEmotion(emotionTarget, emotion);
        }

        public void SetPlayerEmotion(string emotion)
        {
            SetEmotion(EmotionTarget.Player, emotion);
        }

        public void SetNpcEmotion(string emotion)
        {
            SetEmotion(EmotionTarget.Npc, emotion);
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

            var emotionName = GetEmotionForCurrentCharacter();
            return _currentCharacter.emotions?.FirstOrDefault(e => e.emotionName == emotionName)
                ?? _currentCharacter.emotions?.FirstOrDefault(e => e.emotionName == DefaultEmotion);
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

        private async YarnTask RunTypewriterSafely(string lineId, MarkupParseResult text, CancellationToken cancellationToken)
        {
            if (_activePanel?.lineText == null)
            {
                return;
            }

            var textElement = _activePanel.lineText;
            var handlers = Typewriter?.ActionMarkupHandlers ?? ActionMarkupHandlers;

            textElement.maxVisibleCharacters = 0;
            textElement.text = text.Text;

            foreach (var handler in handlers)
            {
                handler.OnLineDisplayBegin(text, textElement);
            }

            textElement.ForceMeshUpdate();

            var visibleCharacterCount = textElement.textInfo.characterCount;
            if (visibleCharacterCount < 0)
            {
                visibleCharacterCount = 0;
            }

            if (visibleCharacterCount == 0 && string.IsNullOrWhiteSpace(text.Text) == false)
            {
                Debug.LogWarning(
                    $"TMP returned zero visible characters for line {lineId}. Showing the full text immediately.");
                textElement.maxVisibleCharacters = int.MaxValue;

                foreach (var handler in handlers)
                {
                    handler.OnLineDisplayComplete();
                }

                return;
            }

            var secondsPerCharacter = lettersPerSecond > 0
                ? 1d / lettersPerSecond
                : 0d;
            var accumulatedDelay = secondsPerCharacter;

            for (var i = 0; i < visibleCharacterCount; i++)
            {
                while (!cancellationToken.IsCancellationRequested &&
                       accumulatedDelay < secondsPerCharacter)
                {
                    var timeBeforeYield = Time.timeAsDouble;
                    await YarnTask.Yield();
                    var timeAfterYield = Time.timeAsDouble;
                    accumulatedDelay += timeAfterYield - timeBeforeYield;
                }

                foreach (var handler in handlers)
                {
                    await handler.OnCharacterWillAppear(i, text, cancellationToken)
                        .SuppressCancellationThrow();
                }

                textElement.maxVisibleCharacters = i + 1;
                accumulatedDelay -= secondsPerCharacter;
            }

            textElement.maxVisibleCharacters = visibleCharacterCount;

            foreach (var handler in handlers)
            {
                handler.OnLineDisplayComplete();
            }
        }

        private static MarkupParseResult ResolveLineText(LocalizedLine line)
        {
            var textWithoutCharacterName = line.TextWithoutCharacterName;
            if (string.IsNullOrWhiteSpace(textWithoutCharacterName.Text) == false)
            {
                return textWithoutCharacterName;
            }

            var fallbackText = ExtractFallbackText(line);
            if (string.IsNullOrWhiteSpace(fallbackText) == false)
            {
                Debug.LogWarning(
                    $"Line {line.TextID} produced an empty TextWithoutCharacterName. Falling back to raw localized text.");
                return new MarkupParseResult(fallbackText, new List<MarkupAttribute>());
            }

            return line.Text;
        }

        private static string ExtractFallbackText(LocalizedLine line)
        {
            if (string.IsNullOrWhiteSpace(line.RawText))
            {
                return line.Text.Text;
            }

            var rawText = line.RawText.Trim();
            var characterName = line.CharacterName?.Trim();

            if (string.IsNullOrWhiteSpace(characterName) == false &&
                rawText.StartsWith(characterName, StringComparison.Ordinal))
            {
                var contentStartIndex = characterName.Length;

                while (contentStartIndex < rawText.Length &&
                       (rawText[contentStartIndex] == ':' || char.IsWhiteSpace(rawText[contentStartIndex])))
                {
                    contentStartIndex++;
                }

                if (contentStartIndex < rawText.Length)
                {
                    return rawText.Substring(contentStartIndex);
                }
            }

            var separatorIndex = rawText.IndexOf(':');
            if (separatorIndex >= 0 && separatorIndex < rawText.Length - 1)
            {
                return rawText.Substring(separatorIndex + 1).TrimStart();
            }

            return rawText;
        }

        private void ApplyPendingEmotionForCurrentCharacter()
        {
            if (string.IsNullOrWhiteSpace(_pendingEmotionForNextLine) || _currentCharacter == null)
            {
                return;
            }

            SetEmotion(_currentCharacter.isPlayer ? EmotionTarget.Player : EmotionTarget.Npc, _pendingEmotionForNextLine);
            _pendingEmotionForNextLine = null;
        }

        private void SetEmotion(EmotionTarget target, string emotion)
        {
            var resolvedTarget = ResolveEmotionTarget(target);
            if (resolvedTarget == EmotionTarget.Unknown)
            {
                Debug.LogWarning("Cannot set emotion because the current dialogue speaker is unknown.");
                return;
            }

            var normalizedEmotion = NormalizeEmotion(emotion);
            switch (resolvedTarget)
            {
                case EmotionTarget.Player:
                    _playerEmotion = normalizedEmotion;
                    break;
                case EmotionTarget.Npc:
                    _npcEmotion = normalizedEmotion;
                    break;
            }

            if (IsCurrentCharacterTarget(resolvedTarget))
            {
                SetMouthClosed();
            }
        }

        private string GetEmotionForCurrentCharacter()
        {
            if (_currentCharacter == null)
            {
                return DefaultEmotion;
            }

            return _currentCharacter.isPlayer ? _playerEmotion : _npcEmotion;
        }

        private EmotionTarget ResolveEmotionTarget(EmotionTarget target)
        {
            if (target != EmotionTarget.Current)
            {
                return target;
            }

            if (_currentCharacter != null)
            {
                return _currentCharacter.isPlayer ? EmotionTarget.Player : EmotionTarget.Npc;
            }

            if (_activePanel == playerPanel)
            {
                return EmotionTarget.Player;
            }

            if (_activePanel == npcPanel)
            {
                return EmotionTarget.Npc;
            }

            return EmotionTarget.Unknown;
        }

        private bool IsCurrentCharacterTarget(EmotionTarget target)
        {
            if (_currentCharacter == null)
            {
                return false;
            }

            return target == EmotionTarget.Player
                ? _currentCharacter.isPlayer
                : !_currentCharacter.isPlayer;
        }

        private void ResetEmotions()
        {
            _playerEmotion = DefaultEmotion;
            _npcEmotion = DefaultEmotion;
            _pendingEmotionForNextLine = null;
        }

        private static string NormalizeEmotion(string emotion)
        {
            return string.IsNullOrWhiteSpace(emotion) ? DefaultEmotion : emotion.Trim();
        }

        private static bool TryParseEmotionTarget(string target, out EmotionTarget emotionTarget)
        {
            switch (target?.Trim().ToLowerInvariant())
            {
                case "player":
                case "игрок":
                    emotionTarget = EmotionTarget.Player;
                    return true;
                case "npc":
                case "нпс":
                    emotionTarget = EmotionTarget.Npc;
                    return true;
                case "current":
                case "speaker":
                case "текущий":
                    emotionTarget = EmotionTarget.Current;
                    return true;
                default:
                    emotionTarget = EmotionTarget.Unknown;
                    return false;
            }
        }

        private enum EmotionTarget
        {
            Unknown,
            Current,
            Player,
            Npc,
        }
    }
}
