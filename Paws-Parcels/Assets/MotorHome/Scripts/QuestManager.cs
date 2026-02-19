using System;
using System.Collections;
using System.Runtime.ExceptionServices;
using Game.Root;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace MotorHome.Scripts
{
    public class QuestManager : MonoBehaviour
    {
        
        [Inject] private QuestsSystem _questsSystem;
        [Inject] private GasSystem _gasSystem;
        [Inject] private MusicManager _musicManager;
        [Inject] private IStopMove _stopMove;
        [Inject] private CursorManager _cursorManager;
        [Inject] private SoloMonolog _soloMonolog;
        [Inject] private UIActiveCoreManager _activeCoreManager;
        [Inject] private MessageGameUI _messageGameUI;
        private CompassUI _compassUI;
        //[SerializeField] private EventManagerTest _eventManagerTest;
        
        
       
        
        [Inject] 
        private CoreUI _coreUI;
        private IEnumerator Start()
        {
            yield return new WaitForSeconds(1);
            NewQuest(1);
            _soloMonolog.StartMonolog(new [] {"У меня закончился бензин.", "Пойду заправлюсь у колонки."}).Subscribe(_ =>
            {
                _activeCoreManager.CurrentUI.Value = UIEnum.QuestsPanel;
                //_eventManagerTest.ShowFerstMArk();
            });
            
            _coreUI.NewQuest.SetActive(false);
        }
        

        public void NewQuest(int number)
        {
            _questsSystem.CompleteQuest((QuestEnum)number);
            _questsSystem.NewQuest((QuestEnum)number);
            StartCoroutine(NewQ());
        }

        private IEnumerator NewQ()
        {
            _coreUI.NewQuest.SetActive(true);
            yield return new WaitForSeconds(4f);
            _coreUI.NewQuest.SetActive(false);
        }

        public void CompleteQuest(int number)
        {
            _questsSystem.CompleteQuest((QuestEnum)number);
        }

        public void ActivateCompass()
        {
            //_compassUI.ActivateCompassUI();
        }

        public void FullGas()
        {
            _gasSystem.SetGas(0.9f);
        }

        public void SetRainy()
        {
            _musicManager.SetRainy();
        }
        
        public void SetSunny()
        {
            _musicManager.SetSunny();
        }

        public void StopMove()
        {
            _stopMove.StopMove();
        }

        public void StartMove()
        {
            _stopMove.StartMove();
        }

        public void SetCompass(Transform tr)
        {
            //_compassUI.SetTarget(tr);
        }

        public void SetCursorStateLocked(bool newState)
        {
            _cursorManager.SetCursorStateLocked(newState);
        }

        public void ShowMessage(string message)
        {
            _messageGameUI.Send(message, 100000f);
        }

        public void HideMessageWithText(string message)
        {
            _messageGameUI.HideMessageWithText(message);
        }
    }
}