using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ObservableCollections;
using R3;
using TMPro;
using UI.MVVM;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.Screens.Quests
{
    public class ScreenQuestsBinder : WindowBinder<ScreenQuestsViewModel>
    {
        private enum JournalTabId
        {
            Quests,
            Hints,
            DiaryPages,
            Skins
        }

        private enum SkinActionType
        {
            Buy,
            Apply,
            Cancel
        }
        
        [Serializable]
        public class MyClass
        {
            public string DiaryPageName;
            public GameObject DiaryGameobjectToSetActive;
        }

        [Serializable]
        private sealed class SkinSetup
        {
            [SerializeField] private GameObject _skinObjectInGrid;
            [SerializeField] private GameObject _purchasedMarkObjectInGrid;
            [SerializeField] [FormerlySerializedAs("_skinName")] private string _skinId;
            [SerializeField] private string _skinNameRu;
            [SerializeField] private string _skinNameEn;
            [SerializeField] [FormerlySerializedAs("_skinDescription")] [TextArea] private string _skinDescriptionRu;
            [SerializeField] [TextArea] private string _skinDescriptionEn;
            [SerializeField] private int _purchaseCost;

            private Button _cachedButton;

            public GameObject SkinObjectInGrid => _skinObjectInGrid;
            public string SkinId => string.IsNullOrWhiteSpace(_skinId) == false
                ? _skinId
                : _skinObjectInGrid != null
                    ? _skinObjectInGrid.name
                    : string.Empty;
            public int PurchaseCost => Mathf.Max(0, _purchaseCost);

            public Button ResolveButton()
            {
                if (_cachedButton == null && _skinObjectInGrid != null)
                {
                    _cachedButton = _skinObjectInGrid.GetComponent<Button>() ??
                                    _skinObjectInGrid.GetComponentInChildren<Button>(true);
                }

                return _cachedButton;
            }

            public void SetPurchasedMarkVisible(bool visible)
            {
                if (_purchasedMarkObjectInGrid != null)
                {
                    _purchasedMarkObjectInGrid.SetActive(visible);
                }
            }

            public string GetDisplayName(bool isRussian)
            {
                if (isRussian)
                {
                    if (string.IsNullOrWhiteSpace(_skinNameRu) == false)
                    {
                        return _skinNameRu;
                    }

                    if (string.IsNullOrWhiteSpace(_skinNameEn) == false)
                    {
                        return _skinNameEn;
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(_skinNameEn) == false)
                    {
                        return _skinNameEn;
                    }

                    if (string.IsNullOrWhiteSpace(_skinNameRu) == false)
                    {
                        return _skinNameRu;
                    }
                }

                return SkinId;
            }

            public string GetDescription(bool isRussian)
            {
                if (isRussian)
                {
                    if (string.IsNullOrWhiteSpace(_skinDescriptionRu) == false)
                    {
                        return _skinDescriptionRu;
                    }

                    if (string.IsNullOrWhiteSpace(_skinDescriptionEn) == false)
                    {
                        return _skinDescriptionEn;
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(_skinDescriptionEn) == false)
                    {
                        return _skinDescriptionEn;
                    }

                    if (string.IsNullOrWhiteSpace(_skinDescriptionRu) == false)
                    {
                        return _skinDescriptionRu;
                    }
                }

                return string.Empty;
            }
        }

        [Serializable]
        private sealed class JournalTabSetup
        {
            [SerializeField] private Button _tabButton;
            [SerializeField] private Image _openedTabImage;
            [SerializeField] private GameObject _contentRoot;
            [SerializeField] private Button _previousPageButton;
            [SerializeField] private Button _nextPageButton;
            [SerializeField] private GameObject[] _pages = Array.Empty<GameObject>();

            private readonly List<GameObject> _resolvedPages = new();
            private int _currentPageIndex;
            private bool _hasExplicitPages;

            public Button TabButton => _tabButton;
            public Image OpenedTabImage => _openedTabImage;
            public GameObject ContentRoot => _contentRoot;
            public Button PreviousPageButton => _previousPageButton;
            public Button NextPageButton => _nextPageButton;

            public void SetTabButton(Button button)
            {
                _tabButton = button;
            }

            public void SetContentRoot(GameObject contentRoot)
            {
                _contentRoot = contentRoot;
            }

            public void SetOpenedTabImage(Image openedTabImage)
            {
                _openedTabImage = openedTabImage;
            }

            public void SetPreviousPageButton(Button button)
            {
                _previousPageButton = button;
            }

            public void SetNextPageButton(Button button)
            {
                _nextPageButton = button;
            }

            public void SetPages(IEnumerable<GameObject> pages)
            {
                _hasExplicitPages = true;
                _pages = pages?.Where(page => page != null).ToArray() ?? Array.Empty<GameObject>();
            }

            public IReadOnlyList<GameObject> ResolvePages()
            {
                _resolvedPages.Clear();

                if (_pages != null)
                {
                    foreach (var page in _pages)
                    {
                        if (page != null)
                        {
                            _resolvedPages.Add(page);
                        }
                    }
                }

                if (_resolvedPages.Count == 0 && _contentRoot != null && HasPagingControls() && _hasExplicitPages == false)
                {
                    foreach (Transform child in _contentRoot.transform)
                    {
                        if (child == null)
                        {
                            continue;
                        }

                        if (_previousPageButton != null && child == _previousPageButton.transform)
                        {
                            continue;
                        }

                        if (_nextPageButton != null && child == _nextPageButton.transform)
                        {
                            continue;
                        }

                        if (child.GetComponent<Button>() != null)
                        {
                            continue;
                        }

                        _resolvedPages.Add(child.gameObject);
                    }
                }

                if (_resolvedPages.Count == 0 && _contentRoot != null)
                {
                    _resolvedPages.Add(_contentRoot);
                }

                _currentPageIndex = Mathf.Clamp(_currentPageIndex, 0, Mathf.Max(0, _resolvedPages.Count - 1));

                return _resolvedPages;
            }

            public void SetContentVisible(bool visible, bool supportsPaging)
            {
                if (_contentRoot != null)
                {
                    _contentRoot.SetActive(visible);
                }

                if (visible == false)
                {
                    return;
                }

                if (supportsPaging)
                {
                    ShowCurrentPage();
                }
            }

            public void ShowCurrentPage()
            {
                ShowPage(_currentPageIndex);
            }

            public void ShowPage(int pageIndex)
            {
                if (_resolvedPages.Count == 0)
                {
                    return;
                }

                _currentPageIndex = Mathf.Clamp(pageIndex, 0, _resolvedPages.Count - 1);

                for (var i = 0; i < _resolvedPages.Count; i++)
                {
                    if (_resolvedPages[i] != null)
                    {
                        _resolvedPages[i].SetActive(i == _currentPageIndex);
                    }
                }
            }

            public bool CanMovePage(int delta)
            {
                if (_resolvedPages.Count <= 1)
                {
                    return false;
                }

                var nextPageIndex = Mathf.Clamp(_currentPageIndex + delta, 0, _resolvedPages.Count - 1);
                return nextPageIndex != _currentPageIndex;
            }

            public void MovePage(int delta)
            {
                if (_resolvedPages.Count == 0)
                {
                    return;
                }

                _currentPageIndex = Mathf.Clamp(_currentPageIndex + delta, 0, _resolvedPages.Count - 1);
            }

            public void UpdatePageButtons(bool isActiveTab, bool supportsPaging)
            {
                var canMoveBack = isActiveTab && supportsPaging && HasPagingControls() && _resolvedPages.Count > 1 && _currentPageIndex > 0;
                var canMoveForward = isActiveTab && supportsPaging && HasPagingControls() && _resolvedPages.Count > 1 &&
                                     _currentPageIndex < _resolvedPages.Count - 1;

                if (_previousPageButton != null)
                {
                    _previousPageButton.gameObject.SetActive(canMoveBack);
                    _previousPageButton.interactable = canMoveBack;
                }

                if (_nextPageButton != null)
                {
                    _nextPageButton.gameObject.SetActive(canMoveForward);
                    _nextPageButton.interactable = canMoveForward;
                }
            }

            public void SetOpenedTabVisible(bool visible)
            {
                if (_openedTabImage != null)
                {
                    _openedTabImage.gameObject.SetActive(visible);
                }
            }

            private bool HasPagingControls()
            {
                return _previousPageButton != null || _nextPageButton != null;
            }
        }

        [Serializable]
        private sealed class BookFlipbookAnimation
        {
            [SerializeField] private Image _overlayImage;
            [SerializeField] private Sprite[] _openingFrames = Array.Empty<Sprite>();
            [SerializeField] private Sprite[] _closingFrames = Array.Empty<Sprite>();
            [SerializeField] private float _secondsPerFrame = 0.04f;
            [SerializeField] private float _secondsPerFrameForSwitching = 0.02f;
            [SerializeField] private bool _hideOverlayWhenIdle = true;

            public Image OverlayImage => _overlayImage;
            public bool HideOverlayWhenIdle => _hideOverlayWhenIdle;
            public float SecondsPerFrame => Mathf.Max(0.01f, _secondsPerFrame);
            public float SecondsPerFrameForSwitching => Mathf.Max(0.01f, _secondsPerFrameForSwitching);

            public void SetOverlayImage(Image image)
            {
                _overlayImage = image;
            }

            public IReadOnlyList<Sprite> GetOpeningFrames()
            {
                var frames = FilterFrames(_openingFrames);
                return frames.Length > 0 ? frames : Reverse(FilterFrames(_closingFrames));
            }

            public IReadOnlyList<Sprite> GetClosingFrames()
            {
                var frames = FilterFrames(_closingFrames);
                return frames.Length > 0 ? frames : Reverse(FilterFrames(_openingFrames));
            }

            private static Sprite[] FilterFrames(Sprite[] frames)
            {
                return frames == null ? Array.Empty<Sprite>() : frames.Where(frame => frame != null).ToArray();
            }

            private static Sprite[] Reverse(IReadOnlyList<Sprite> frames)
            {
                if (frames == null || frames.Count == 0)
                {
                    return Array.Empty<Sprite>();
                }

                var reversedFrames = new Sprite[frames.Count];

                for (var i = 0; i < frames.Count; i++)
                {
                    reversedFrames[i] = frames[frames.Count - 1 - i];
                }

                return reversedFrames;
            }
        }

        private static readonly string[] RuntimeQuestObjects =
        {
            "QuestName",
            "MissionName",
            "MissionDescription",
            "limit",
            "reward",
            "LimitText",
            "Button",
            "Image"
        };

        [SerializeField] private Button _exitButton;
        [SerializeField] private Button _selectInDataButton;

        [SerializeField] private Transform _prefabsGrid;
        [SerializeField] private OneQuestBinder _oneQuestBinderPrefab;

        [SerializeField] private Color _nonSelectedColor;
        [SerializeField] private Color _selectedColor;
        [SerializeField] private Color _mouseEnterColor;

        [SerializeField] private TMP_Text _questTitleText;
        [SerializeField] private TMP_Text _missionTitleText;
        [SerializeField] private TMP_Text _missionDescriptionText;
        [SerializeField] private TMP_Text _missionLimitTextText;
        [SerializeField] private TMP_Text _questRewardMoneyText;

        [Header("Journal Tabs")]
        [SerializeField] private JournalTabSetup _questsTab = new();
        [SerializeField] private JournalTabSetup _hintsTab = new();
        [SerializeField] private JournalTabSetup _diaryPagesTab = new();
        [SerializeField] private JournalTabSetup _skinsTab = new();
        [SerializeField] private List<MyClass> _diaryPagesGameObjects = new();

        [Header("Skins")]
        [SerializeField] private List<SkinSetup> _skins = new();
        [SerializeField] private TMP_Text _skinNameText;
        [SerializeField] private TMP_Text _skinDescriptionText;
        [SerializeField] private TMP_Text _skinCostText;
        [SerializeField] [FormerlySerializedAs("_skinBuyButton")] private Button _skinActionButton;
        [SerializeField] [FormerlySerializedAs("_skinBuyButtonText")] private TMP_Text _skinActionButtonText;

        [Header("Book Animation")]
        [SerializeField] private BookFlipbookAnimation _bookAnimation = new();

        private readonly Dictionary<OneQuestViewModel, OneQuestBinder> _instantiatedOneQuestBindersMap = new();
        private readonly Dictionary<JournalTabId, JournalTabSetup> _journalTabs = new();
        private readonly List<SkinSetup> _resolvedSkins = new();

        private CompositeDisposable _disposables;
        private CanvasGroup _canvasGroup;
        private RectTransform _runtimeTabButtonsContainer;
        private Coroutine _transitionCoroutine;
        private JournalTabId _currentTabId = JournalTabId.Quests;
        private bool _isTransitioning;
        private bool _isClosing;
        private SkinSetup _selectedSkin;

        protected override void OnBind(ScreenQuestsViewModel viewModel)
        {
            for (var i = _prefabsGrid.childCount - 1; i >= 0; i--)
            {
                Destroy(_prefabsGrid.GetChild(i).gameObject);
            }

            _disposables = new CompositeDisposable();

            foreach (var oneQuestViewModel in viewModel.OneQuestViewModels)
            {
                AddOneQuestBinder(oneQuestViewModel);
            }

            _disposables.Add(viewModel.OneQuestViewModels.ObserveAdd().Subscribe(e =>
            {
                var placeIndex = viewModel.OneQuestViewModels.Count - e.Index - 1;
                InsertOneQuestBinder(e.Value, placeIndex);
            }));

            _disposables.Add(viewModel.OneQuestViewModels.ObserveRemove().Subscribe(e =>
            {
                DestroyOneQuestBinder(e.Value);
            }));

            _disposables.Add(viewModel.OneQuestViewModels.ObserveClear().Subscribe(_ =>
            {
                DestroyAllQuestBinders();
            }));

            _disposables.Add(_exitButton.OnClickAsObservable().Subscribe(_ => viewModel.RequestBackToGame()));
            _disposables.Add(_selectInDataButton.OnClickAsObservable().Subscribe(_ => viewModel.RequestSelectInData()));

            _disposables.Add(viewModel.CurrentQuestTitleText.Subscribe(text => _questTitleText.text = text));
            _disposables.Add(viewModel.CurrentMissionTitleText.Subscribe(text => _missionTitleText.text = text));
            _disposables.Add(viewModel.CurrentMissionDescriptionText.Subscribe(text => _missionDescriptionText.text = text));
            _disposables.Add(viewModel.CurrentMissionLimitText.Subscribe(text => _missionLimitTextText.text = text));
            _disposables.Add(viewModel.CurrentQuestRewardMoneyText.Subscribe(text => _questRewardMoneyText.text = text));
            _disposables.Add(viewModel.DiaryPages.ObserveAdd().Subscribe(_ => UpdateDiaryPagesVisibility(viewModel)));
            _disposables.Add(viewModel.DiaryPages.ObserveRemove().Subscribe(_ => UpdateDiaryPagesVisibility(viewModel)));
            _disposables.Add(viewModel.DiaryPages.ObserveClear().Subscribe(_ => UpdateDiaryPagesVisibility(viewModel)));
            _disposables.Add(viewModel.PurchasedSkins.ObserveAdd().Subscribe(_ => RefreshSkinsUi(viewModel)));
            _disposables.Add(viewModel.PurchasedSkins.ObserveRemove().Subscribe(_ => RefreshSkinsUi(viewModel)));
            _disposables.Add(viewModel.PurchasedSkins.ObserveClear().Subscribe(_ => RefreshSkinsUi(viewModel)));
            _disposables.Add(viewModel.AppliedSkinName.Subscribe(_ => RefreshSkinsUi(viewModel)));
            _disposables.Add(viewModel.Money.Subscribe(_ => RefreshSkinsUi(viewModel)));
            _disposables.Add(viewModel.LocalizationService.CurrentLocale.Subscribe(_ => RefreshSkinsUi(viewModel)));

            InitializeSkinsUi(viewModel);
            UpdateDiaryPagesVisibility(viewModel);
            InitializeJournalUi();
        }

        public override void Close()
        {
            if (_isClosing)
            {
                return;
            }

            if (gameObject.activeInHierarchy == false)
            {
                base.Close();
                return;
            }

            if (_transitionCoroutine != null)
            {
                StopCoroutine(_transitionCoroutine);
                _transitionCoroutine = null;
            }

            StartCoroutine(CloseRoutine());
        }

        private void OnDestroy()
        {
            _disposables?.Dispose();
        }

        private void InitializeJournalUi()
        {
            EnsureCanvasGroup();
            EnsureJournalTabsMap();
            EnsureQuestTabContentRoot();
            EnsureDefaultContentRoots();
            EnsureTabButtons();
            EnsureAnimationOverlay();
            ResolveAllTabPages();
            BindJournalButtons();
            HideAllTabContent();
            UpdateTabButtonsVisuals();
            SetInputEnabled(false);

            _transitionCoroutine = StartCoroutine(InitialOpenRoutine());
        }

        private void EnsureCanvasGroup()
        {
            _canvasGroup = GetComponent<CanvasGroup>();

            if (_canvasGroup == null)
            {
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        private void EnsureJournalTabsMap()
        {
            _questsTab ??= new JournalTabSetup();
            _hintsTab ??= new JournalTabSetup();
            _diaryPagesTab ??= new JournalTabSetup();
            _skinsTab ??= new JournalTabSetup();

            _journalTabs.Clear();
            _journalTabs[JournalTabId.Quests] = _questsTab;
            _journalTabs[JournalTabId.Hints] = _hintsTab;
            _journalTabs[JournalTabId.DiaryPages] = _diaryPagesTab;
            _journalTabs[JournalTabId.Skins] = _skinsTab;
        }

        private void EnsureQuestTabContentRoot()
        {
            if (_questsTab.ContentRoot != null)
            {
                return;
            }

            var contentRoot = CreateFullStretchObject("QuestsTabContent");
            _questsTab.SetContentRoot(contentRoot);

            var scrollViewTransform = FindAncestorOrSelf(_prefabsGrid, "Scroll View");
            MoveIfNeeded(scrollViewTransform, contentRoot.transform);

            foreach (var objectName in RuntimeQuestObjects)
            {
                var directChild = FindDirectChild(objectName);
                MoveIfNeeded(directChild, contentRoot.transform);
            }
        }

        private void EnsureDefaultContentRoots()
        {
            EnsureContentRoot(_hintsTab, "HintsTabContent");
            EnsureContentRoot(_diaryPagesTab, "DiaryPagesTabContent");
            EnsureContentRoot(_skinsTab, "SkinsTabContent");
        }

        private void EnsureContentRoot(JournalTabSetup tabSetup, string objectName)
        {
            if (tabSetup.ContentRoot == null)
            {
                tabSetup.SetContentRoot(CreateFullStretchObject(objectName));
            }
        }

        private void EnsureTabButtons()
        {
            EnsureTabButton(_questsTab, "JournalTabQuests", "Quests");
            EnsureTabButton(_hintsTab, "JournalTabHints", "Hints");
            EnsureTabButton(_diaryPagesTab, "JournalTabDiaryPages", "Pages");
            EnsureTabButton(_skinsTab, "JournalTabSkins", "Skins");
        }

        private void EnsureTabButton(JournalTabSetup tabSetup, string objectName, string label)
        {
            if (tabSetup.TabButton != null)
            {
                return;
            }

            _runtimeTabButtonsContainer ??= CreateTabsContainer();

            var button = CreateRuntimeButton(_runtimeTabButtonsContainer, objectName, label, new Vector2(230f, 60f));
            tabSetup.SetTabButton(button);
        }

        private void EnsureAnimationOverlay()
        {
            if (_bookAnimation.OverlayImage != null)
            {
                return;
            }

            var overlayObject = CreateFullStretchObject("JournalAnimationOverlay");
            overlayObject.transform.SetAsLastSibling();

            var image = overlayObject.AddComponent<Image>();
            image.raycastTarget = false;
            image.preserveAspect = true;
            overlayObject.SetActive(false);

            _bookAnimation.SetOverlayImage(image);
        }

        private void InitializeSkinsUi(ScreenQuestsViewModel viewModel)
        {
            ResolveSkins();
            BindSkinButtons(viewModel);
            EnsureSelectedSkin();
            RefreshSkinsUi(viewModel);
        }

        private void ResolveSkins()
        {
            _resolvedSkins.Clear();

            foreach (var skin in _skins)
            {
                if (skin == null || skin.SkinObjectInGrid == null)
                {
                    continue;
                }

                _resolvedSkins.Add(skin);
            }

            if (_selectedSkin != null && _resolvedSkins.Contains(_selectedSkin) == false)
            {
                _selectedSkin = null;
            }
        }

        private void BindSkinButtons(ScreenQuestsViewModel viewModel)
        {
            foreach (var skin in _resolvedSkins)
            {
                var button = skin.ResolveButton();

                if (button == null)
                {
                    continue;
                }

                var capturedSkin = skin;
                _disposables.Add(button.OnClickAsObservable().Subscribe(_ => SelectSkin(capturedSkin, viewModel)));
            }

            if (_skinActionButton != null)
            {
                _disposables.Add(_skinActionButton.OnClickAsObservable().Subscribe(_ => RequestSelectedSkinAction(viewModel)));
            }
        }

        private void EnsureSelectedSkin()
        {
            if (_selectedSkin != null && _resolvedSkins.Contains(_selectedSkin))
            {
                return;
            }

            _selectedSkin = _resolvedSkins.FirstOrDefault();
        }

        private void SelectSkin(SkinSetup skin, ScreenQuestsViewModel viewModel)
        {
            if (skin == null)
            {
                return;
            }

            _selectedSkin = skin;
            RefreshSkinsUi(viewModel);
        }

        private void RequestSelectedSkinAction(ScreenQuestsViewModel viewModel)
        {
            if (_selectedSkin == null)
            {
                return;
            }

            if (viewModel.IsSkinPurchased(_selectedSkin.SkinId) == false)
            {
                viewModel.RequestBuySkin(_selectedSkin.SkinId, _selectedSkin.PurchaseCost);
            }
            else if (viewModel.IsSkinApplied(_selectedSkin.SkinId))
            {
                viewModel.RequestCancelSkin();
            }
            else
            {
                viewModel.RequestApplySkin(_selectedSkin.SkinId);
            }

            RefreshSkinsUi(viewModel);
        }

        private void RefreshSkinsUi(ScreenQuestsViewModel viewModel)
        {
            EnsureSelectedSkin();

            foreach (var skin in _resolvedSkins)
            {
                var button = skin.ResolveButton();
                var isSkinPurchasedInGrid = viewModel.IsSkinPurchased(skin.SkinId);

                if (button == null)
                {
                    skin.SetPurchasedMarkVisible(isSkinPurchasedInGrid);
                    continue;
                }

                SetButtonGraphicColor(button, skin == _selectedSkin ? _selectedColor : _nonSelectedColor);
                skin.SetPurchasedMarkVisible(isSkinPurchasedInGrid);
            }

            if (_selectedSkin == null)
            {
                SetSkinDetails("-", "-", "-");

                if (_skinActionButton != null)
                {
                    _skinActionButton.gameObject.SetActive(false);
                }

                return;
            }

            var isRussian = IsRussianLanguage(viewModel);
            var isPurchased = viewModel.IsSkinPurchased(_selectedSkin.SkinId);
            var isApplied = viewModel.IsSkinApplied(_selectedSkin.SkinId);

            SetSkinDetails(_selectedSkin.GetDisplayName(isRussian), _selectedSkin.GetDescription(isRussian),
                $"{_selectedSkin.PurchaseCost}/{viewModel.Money.CurrentValue}");

            if (_skinActionButtonText != null)
            {
                _skinActionButtonText.text = isPurchased == false
                    ? GetLocalizedButtonText(isRussian, SkinActionType.Buy)
                    : isApplied
                    ? GetLocalizedButtonText(isRussian, SkinActionType.Cancel)
                    : GetLocalizedButtonText(isRussian, SkinActionType.Apply);
            }

            if (_skinActionButton != null)
            {
                _skinActionButton.gameObject.SetActive(true);
                _skinActionButton.interactable = isPurchased || viewModel.CanBuySkin(_selectedSkin.SkinId, _selectedSkin.PurchaseCost);
            }
        }

        private static bool IsRussianLanguage(ScreenQuestsViewModel viewModel)
        {
            var locale = viewModel.LocalizationService.ActiveLocale;
            var languageCode = locale?.Identifier.CultureInfo?.TwoLetterISOLanguageName;
            return string.Equals(languageCode, "ru", StringComparison.OrdinalIgnoreCase);
        }

        private static string GetLocalizedButtonText(bool isRussian, SkinActionType actionType)
        {
            if (isRussian)
            {
                return actionType switch
                {
                    SkinActionType.Buy => "\u041A\u0443\u043F\u0438\u0442\u044C",
                    SkinActionType.Apply => "\u041F\u0440\u0438\u043C\u0435\u043D\u0438\u0442\u044C",
                    SkinActionType.Cancel => "\u041E\u0442\u043C\u0435\u043D\u0438\u0442\u044C",
                    _ => string.Empty
                };
            }

            return actionType switch
            {
                SkinActionType.Buy => "Buy",
                SkinActionType.Apply => "Apply",
                SkinActionType.Cancel => "Cancel",
                _ => string.Empty
            };
        }

        private void SetSkinDetails(string skinName, string skinDescription, string skinCost)
        {
            if (_skinNameText != null)
            {
                _skinNameText.text = string.IsNullOrWhiteSpace(skinName) ? "-" : skinName;
            }

            if (_skinDescriptionText != null)
            {
                _skinDescriptionText.text = string.IsNullOrWhiteSpace(skinDescription) ? "-" : skinDescription;
            }

            if (_skinCostText != null)
            {
                _skinCostText.text = string.IsNullOrWhiteSpace(skinCost) ? "-" : skinCost;
            }
        }

        private void UpdateDiaryPagesVisibility(ScreenQuestsViewModel viewModel)
        {
            var availableDiaryPageNames = new HashSet<string>(viewModel.DiaryPages);
            var availableDiaryPageObjects = new List<GameObject>();

            foreach (var diaryPageData in _diaryPagesGameObjects)
            {
                if (diaryPageData == null || diaryPageData.DiaryGameobjectToSetActive == null)
                {
                    continue;
                }

                var isAvailable = string.IsNullOrEmpty(diaryPageData.DiaryPageName) == false &&
                                  availableDiaryPageNames.Contains(diaryPageData.DiaryPageName);

                diaryPageData.DiaryGameobjectToSetActive.SetActive(isAvailable);

                if (isAvailable)
                {
                    availableDiaryPageObjects.Add(diaryPageData.DiaryGameobjectToSetActive);
                }
            }

            _diaryPagesTab.SetPages(availableDiaryPageObjects);

            if (_journalTabs.Count == 0)
            {
                return;
            }

            _diaryPagesTab.ResolvePages();

            if (_currentTabId == JournalTabId.DiaryPages)
            {
                _diaryPagesTab.SetContentVisible(true, SupportsPaging(JournalTabId.DiaryPages));
                _diaryPagesTab.UpdatePageButtons(_isClosing == false && _isTransitioning == false,
                    SupportsPaging(JournalTabId.DiaryPages));
            }
        }

        private void ResolveAllTabPages()
        {
            foreach (var journalTab in _journalTabs.Values)
            {
                journalTab.ResolvePages();
            }
        }

        private void BindJournalButtons()
        {
            BindTabButton(JournalTabId.Quests, _questsTab);
            BindTabButton(JournalTabId.Hints, _hintsTab);
            BindTabButton(JournalTabId.DiaryPages, _diaryPagesTab);
            BindTabButton(JournalTabId.Skins, _skinsTab);

            BindPageButtons(JournalTabId.Hints, _hintsTab);
            BindPageButtons(JournalTabId.DiaryPages, _diaryPagesTab);
            BindPageButtons(JournalTabId.Skins, _skinsTab);
        }

        private void BindTabButton(JournalTabId tabId, JournalTabSetup tabSetup)
        {
            if (tabSetup.TabButton == null)
            {
                return;
            }

            _disposables.Add(tabSetup.TabButton.OnClickAsObservable().Subscribe(_ =>
            {
                RequestOpenTab(tabId);
            }));
        }

        private void BindPageButtons(JournalTabId tabId, JournalTabSetup tabSetup)
        {
            if (tabSetup.PreviousPageButton != null)
            {
                _disposables.Add(tabSetup.PreviousPageButton.OnClickAsObservable().Subscribe(_ =>
                {
                    RequestMovePage(tabId, -1);
                }));
            }

            if (tabSetup.NextPageButton != null)
            {
                _disposables.Add(tabSetup.NextPageButton.OnClickAsObservable().Subscribe(_ =>
                {
                    RequestMovePage(tabId, 1);
                }));
            }
        }

        private void RequestOpenTab(JournalTabId tabId)
        {
            if (_isClosing || _isTransitioning || tabId == _currentTabId)
            {
                return;
            }

            _transitionCoroutine = StartCoroutine(RunContentSwap(() =>
            {
                _currentTabId = tabId;
            }));
        }

        private void RequestMovePage(JournalTabId tabId, int delta)
        {
            if (_isClosing || _isTransitioning || tabId != _currentTabId)
            {
                return;
            }

            var tabSetup = _journalTabs[tabId];
            tabSetup.ResolvePages();

            if (tabSetup.CanMovePage(delta) == false)
            {
                return;
            }

            _transitionCoroutine = StartCoroutine(RunContentSwap(() =>
            {
                tabSetup.MovePage(delta);
            }));
        }

        private IEnumerator InitialOpenRoutine()
        {
            _isTransitioning = true;
            UpdateTabButtonsVisuals();

            yield return PlayOpeningAnimation(false);

            ShowCurrentTabContent();
            SetInputEnabled(true);
            _isTransitioning = false;
            _transitionCoroutine = null;
            UpdateTabButtonsVisuals();
        }

        private IEnumerator RunContentSwap(Action applyHiddenChanges)
        {
            _isTransitioning = true;
            SetInputEnabled(false);
            HideAllTabContent();
            UpdateTabButtonsVisuals();

            yield return PlayClosingAnimation(true);

            applyHiddenChanges?.Invoke();
            ResolveAllTabPages();
            UpdateTabButtonsVisuals();

            yield return PlayOpeningAnimation(true);

            ShowCurrentTabContent();
            SetInputEnabled(true);
            _isTransitioning = false;
            _transitionCoroutine = null;
            UpdateTabButtonsVisuals();
        }

        private IEnumerator CloseRoutine()
        {
            _isClosing = true;
            SetInputEnabled(false);
            HideAllTabContent();
            UpdateTabButtonsVisuals();

            yield return PlayClosingAnimation(false);

            base.Close();
        }

        private IEnumerator PlayOpeningAnimation(bool useSwitchingSpeed)
        {
            var openingFrames = _bookAnimation.GetOpeningFrames();

            if (_bookAnimation.OverlayImage == null || openingFrames.Count == 0)
            {
                yield break;
            }

            var secondsPerFrame = useSwitchingSpeed
                ? _bookAnimation.SecondsPerFrameForSwitching
                : _bookAnimation.SecondsPerFrame;

            _bookAnimation.OverlayImage.gameObject.SetActive(true);

            foreach (var frame in openingFrames)
            {
                _bookAnimation.OverlayImage.sprite = frame;
                yield return new WaitForSecondsRealtime(secondsPerFrame);
            }

            if (_bookAnimation.HideOverlayWhenIdle)
            {
                _bookAnimation.OverlayImage.gameObject.SetActive(false);
            }
        }

        private IEnumerator PlayClosingAnimation(bool useSwitchingSpeed)
        {
            var closingFrames = _bookAnimation.GetClosingFrames();

            if (_bookAnimation.OverlayImage == null || closingFrames.Count == 0)
            {
                yield break;
            }

            var secondsPerFrame = useSwitchingSpeed
                ? _bookAnimation.SecondsPerFrameForSwitching
                : _bookAnimation.SecondsPerFrame;

            _bookAnimation.OverlayImage.gameObject.SetActive(true);

            foreach (var frame in closingFrames)
            {
                _bookAnimation.OverlayImage.sprite = frame;
                yield return new WaitForSecondsRealtime(secondsPerFrame);
            }
        }

        private void HideAllTabContent()
        {
            foreach (var journalTab in _journalTabs)
            {
                journalTab.Value.ResolvePages();
                journalTab.Value.SetContentVisible(false, SupportsPaging(journalTab.Key));
                journalTab.Value.UpdatePageButtons(false, SupportsPaging(journalTab.Key));
            }
        }

        private void ShowCurrentTabContent()
        {
            if (_journalTabs.TryGetValue(_currentTabId, out var tabSetup) == false)
            {
                return;
            }

            tabSetup.ResolvePages();
            tabSetup.SetContentVisible(true, SupportsPaging(_currentTabId));
            tabSetup.UpdatePageButtons(true, SupportsPaging(_currentTabId));
        }

        private void UpdateTabButtonsVisuals()
        {
            UpdateExitButtonVisibility();
            var showTabs = _isTransitioning == false && _isClosing == false;

            foreach (var journalTab in _journalTabs)
            {
                var button = journalTab.Value.TabButton;
                var isSelectedTab = journalTab.Key == _currentTabId;

                journalTab.Value.SetOpenedTabVisible(showTabs && isSelectedTab);

                if (button == null)
                {
                    continue;
                }

                button.gameObject.SetActive(showTabs);

                if (showTabs)
                {
                    button.interactable = true;

                    if (journalTab.Value.OpenedTabImage == null)
                    {
                        SetButtonGraphicColor(button, isSelectedTab ? _selectedColor : _nonSelectedColor);
                    }
                }
            }

            foreach (var journalTab in _journalTabs)
            {
                journalTab.Value.UpdatePageButtons(journalTab.Key == _currentTabId && _isClosing == false && _isTransitioning == false,
                    SupportsPaging(journalTab.Key));
            }
        }

        private void UpdateExitButtonVisibility()
        {
            if (_exitButton != null)
            {
                _exitButton.gameObject.SetActive(_isTransitioning == false && _isClosing == false);
            }
        }

        private void SetInputEnabled(bool enabled)
        {
            if (_canvasGroup == null)
            {
                return;
            }

            _canvasGroup.interactable = enabled;
            _canvasGroup.blocksRaycasts = enabled;
        }

        private bool SupportsPaging(JournalTabId tabId)
        {
            return tabId != JournalTabId.Quests;
        }

        private Button CreateRuntimeButton(Transform parent, string objectName, string label, Vector2 size)
        {
            var template = _selectInDataButton;

            if (template != null)
            {
                var buttonObject = Instantiate(template.gameObject, parent);
                buttonObject.name = objectName;

                if (buttonObject.TryGetComponent<RectTransform>(out var rectTransform))
                {
                    rectTransform.sizeDelta = size;
                    rectTransform.localScale = Vector3.one;
                }

                var button = buttonObject.GetComponent<Button>();
                var labelText = buttonObject.GetComponentInChildren<TMP_Text>(true);

                if (labelText != null)
                {
                    labelText.text = label;
                }

                return button;
            }

            return CreateFallbackButton(parent, objectName, label, size);
        }

        private Button CreateFallbackButton(Transform parent, string objectName, string label, Vector2 size)
        {
            var buttonObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            var buttonTransform = (RectTransform)buttonObject.transform;
            buttonTransform.SetParent(parent, false);
            buttonTransform.sizeDelta = size;

            var image = buttonObject.GetComponent<Image>();
            image.color = _nonSelectedColor;

            var button = buttonObject.GetComponent<Button>();
            button.targetGraphic = image;

            var textObject = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            var textTransform = (RectTransform)textObject.transform;
            textTransform.SetParent(buttonTransform, false);
            textTransform.anchorMin = Vector2.zero;
            textTransform.anchorMax = Vector2.one;
            textTransform.offsetMin = Vector2.zero;
            textTransform.offsetMax = Vector2.zero;

            var labelText = textObject.GetComponent<TextMeshProUGUI>();
            labelText.alignment = TextAlignmentOptions.Center;
            labelText.text = label;
            labelText.color = Color.black;
            labelText.fontSize = 30f;

            if (_questTitleText != null)
            {
                labelText.font = _questTitleText.font;
                labelText.fontSharedMaterial = _questTitleText.fontSharedMaterial;
            }

            return button;
        }

        private RectTransform CreateTabsContainer()
        {
            var containerObject = new GameObject("JournalTabButtons", typeof(RectTransform), typeof(HorizontalLayoutGroup));
            var containerTransform = (RectTransform)containerObject.transform;
            containerTransform.SetParent(transform, false);
            containerTransform.anchorMin = new Vector2(0.5f, 1f);
            containerTransform.anchorMax = new Vector2(0.5f, 1f);
            containerTransform.pivot = new Vector2(0.5f, 1f);
            containerTransform.anchoredPosition = new Vector2(0f, -28f);
            containerTransform.sizeDelta = new Vector2(1040f, 70f);

            var horizontalLayoutGroup = containerObject.GetComponent<HorizontalLayoutGroup>();
            horizontalLayoutGroup.spacing = 18f;
            horizontalLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
            horizontalLayoutGroup.childForceExpandWidth = false;
            horizontalLayoutGroup.childForceExpandHeight = false;
            horizontalLayoutGroup.childControlWidth = false;
            horizontalLayoutGroup.childControlHeight = false;

            return containerTransform;
        }

        private GameObject CreateFullStretchObject(string objectName)
        {
            var createdObject = new GameObject(objectName, typeof(RectTransform));
            var rectTransform = (RectTransform)createdObject.transform;
            rectTransform.SetParent(transform, false);
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.localScale = Vector3.one;
            return createdObject;
        }

        private Transform FindDirectChild(string objectName)
        {
            foreach (Transform child in transform)
            {
                if (child != null && child.name == objectName)
                {
                    return child;
                }
            }

            return null;
        }

        private static Transform FindAncestorOrSelf(Transform current, string objectName)
        {
            while (current != null)
            {
                if (current.name == objectName)
                {
                    return current;
                }

                current = current.parent;
            }

            return null;
        }

        private void MoveIfNeeded(Transform movedTransform, Transform newParent)
        {
            if (movedTransform == null || newParent == null || movedTransform == newParent)
            {
                return;
            }

            movedTransform.SetParent(newParent, false);
        }

        private void SetButtonGraphicColor(Button button, Color color)
        {
            if (button == null)
            {
                return;
            }

            var graphic = button.targetGraphic != null ? button.targetGraphic : button.GetComponent<Graphic>();

            if (graphic != null)
            {
                graphic.color = color;
            }
        }

        private void AddOneQuestBinder(OneQuestViewModel oneQuestViewModel)
        {
            var oneQuestBinder = Instantiate(_oneQuestBinderPrefab, _prefabsGrid);

            oneQuestBinder.Initialize(_selectedColor, _nonSelectedColor, _mouseEnterColor);
            oneQuestBinder.Bind(oneQuestViewModel);

            _instantiatedOneQuestBindersMap[oneQuestViewModel] = oneQuestBinder;
        }

        private void InsertOneQuestBinder(OneQuestViewModel oneQuestViewModel, int index)
        {
            var oneQuestBinder = Instantiate(_oneQuestBinderPrefab, _prefabsGrid);
            oneQuestBinder.transform.SetSiblingIndex(index);

            oneQuestBinder.Initialize(_selectedColor, _nonSelectedColor, _mouseEnterColor);
            oneQuestBinder.Bind(oneQuestViewModel);

            _instantiatedOneQuestBindersMap[oneQuestViewModel] = oneQuestBinder;
        }

        private void DestroyOneQuestBinder(OneQuestViewModel oneQuestViewModel)
        {
            if (_instantiatedOneQuestBindersMap.TryGetValue(oneQuestViewModel, out var oneQuestBinder))
            {
                Destroy(oneQuestBinder.gameObject);
                _instantiatedOneQuestBindersMap.Remove(oneQuestViewModel);
            }
        }

        private void DestroyAllQuestBinders()
        {
            foreach (Transform child in _prefabsGrid)
            {
                Destroy(child.gameObject);
            }

            _instantiatedOneQuestBindersMap.Clear();
        }
    }
}
