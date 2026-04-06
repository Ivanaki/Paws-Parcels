using System;
using System.Collections.Generic;
using System.Linq;
using Paws_Parcels.NPCDialogs;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using TMPro;

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
        }

        private void Update()
        {
            if (!_isTyping || _currentCharacter == null || _activePanel?.portrait == null) return;

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
            await Typewriter.RunTypewriter(text, token.HurryUpToken).SuppressCancellationThrow();
            _isTyping = false;

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
    }
}
