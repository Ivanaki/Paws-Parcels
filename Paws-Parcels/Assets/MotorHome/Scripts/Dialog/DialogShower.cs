using System.Collections;
using R3;
using TMPro;
using UnityEngine;
using Zenject;

namespace MotorHome.Scripts
{
    public class DialogShower : MonoBehaviour, IDialogShower
    {
        [SerializeField] private TMP_Text _tmpText;
        [SerializeField] private GameObject _panel;
        
        public bool IsStarted { get; private set; }=  false;
        
        private Coroutine _currentCoroutine;
        private string[] _phrases;
        private int _lettersInSecondSpeed;
        
        private Subject<Unit> _ended = null;
        private int _currentPhrase = 0;

        [Inject]
        private void Construct(GameSettingsSO  gameSettings)
        {
            _lettersInSecondSpeed = gameSettings.LettersInSecondDialogSpeed;
        }

        private void Awake()
        {
            _panel.SetActive(false);
        }

        private void Open()
        {
            //EventManagerTest.CloseShowable.Invoke();
            _panel.SetActive(true);   
        }

        private void Close()
        {
            _panel.SetActive(false);
        }
        
        public Observable<Unit> Show(string[] phrases)
        {
            if (_ended != null)
            {
                _ended.OnNext(Unit.Default);
                _ended.OnCompleted();
                _ended.Dispose();
            }
            
            Open();
            _phrases = phrases;
            IsStarted = true;
            _ended = new Subject<Unit>();
            _ended.Subscribe(_ =>  Close());
            _currentPhrase = 0;
            
            if(_currentCoroutine != null)
                StopCoroutine(_currentCoroutine);
            _currentCoroutine = StartCoroutine(ShowPhrase());
            return _ended;
        }
        

        public void Click()
        {
            if (IsStarted)
            {
                if (_currentCoroutine != null)
                {
                    StopCoroutine(_currentCoroutine);
                    _currentCoroutine = null;
                    _tmpText.text = _phrases[_currentPhrase];
                }
                else
                {
                    if (CheckEnded())
                        return;

                    _currentPhrase++;
                    _currentCoroutine = StartCoroutine(ShowPhrase());
                }
            }
            else
            {
                //Debug.LogError("Dialog is empty");
            }
        }

        public void Clear()
        {
            Close();
            if(_currentCoroutine != null)
                StopCoroutine(_currentCoroutine);
            if (_ended != null)
            {
                if(!_ended.IsDisposed)
                    _ended.Dispose();
                _ended = null;
            }
        }

        private IEnumerator ShowPhrase()
        {
            _tmpText.text = string.Empty;
            foreach (var letter in _phrases[_currentPhrase].ToCharArray())
            {
                _tmpText.text += letter;
                yield return new WaitForSeconds(1f/_lettersInSecondSpeed);
            }
            
            _currentCoroutine = null;
        }

        private bool CheckEnded()
        {
            if (_currentPhrase >= _phrases.Length - 1)
            {
                _tmpText.text = string.Empty;
                IsStarted = false;
                _ended.OnNext(Unit.Default);
                return true;
            }
            return false;
        }
        
        /*public void StartWork(Subject<Unit> start)
        {
            start.Subscribe(_ => );
        }

        public void EndWork(Subject<Unit> end)
        {
            _end = end;
            //End.Subscribe(_ => IsActive = false);
        }*/
    }
}