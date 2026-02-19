using System.Collections;
using System.Runtime.InteropServices;
using R3;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace MotorHome.Scripts
{
    public class TestMB : MonoBehaviour
    {
        [SerializeField] private Transform _gameObject;
        
        [Inject]
        private InputActionAsset _inputActionAsset;
        [Inject]
        private SoloMonolog _soloMonolog;
        [Inject]
        private LanguageChanger _languageChanger;
        [Inject]
        private UIActiveCoreManager _uiActiveCoreManager;
        [Inject]
        private QuestsSystem _questsSystem;
        
        
        private CompassUI _compassUI;
        private GasSystem _gasSystem;
        private MoneySystem _moneySystem;
        [Inject]
        private MusicManager _musicManager;

        [Inject]
        private void Construct(CoreUI coreUI, GasSystem gasSystem, MoneySystem moneySystem)
        {
            _compassUI = coreUI.CompassUI;
            _gasSystem = gasSystem;
            _moneySystem = moneySystem;
        }
        private void Awake()
        {
            /*var x = GetComponent<Collider>();
            print("Awake");
            _inputActionAsset.FindActionMap("UI").Disable();
            _inputActionAsset.FindActionMap("CorePause").Enable();*/
        }

        /*private void Start()
        {
            //print("Start");
            //BehaviorSubject<bool> IsPlayerInCollider = new(false);
            //IsPlayerInCollider.Subscribe(_ => print("subscribe"));
        }*/

        private void OnEnable()
        {
            /*_uiActiveCoreManager.CurrentUI.Subscribe(value =>
            {
                if (value == UIActiveCoreManager.UIEnum.Game)
                {
                    _gameObject.SetActive(true);
                }

                if (value == UIActiveCoreManager.UIEnum.QuestsPanel)
                {
                    _gameObject.SetActive(false);
                }
            });*/
        }

        private void Start()
        {
            
        }

        
        
        private void Update()
        {
            //if (Input.GetKeyDown(KeyCode.K))
            //{
                /*_soloMonolog.StartMonolog(_languageChanger.GetDialog(DialogEnum.SoloPhrases)[0].Phrases).Subscribe(_ =>
                {
                    Debug.Log("Monolog ended");
                });*/
            /*}
            if (Input.GetKeyDown(KeyCode.N))
            {
                _questsSystem.NewQuest(QuestEnum.FirstQuest);
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                _questsSystem.NewQuest(QuestEnum.SecondQuest);
            }*/
        }
    }
}