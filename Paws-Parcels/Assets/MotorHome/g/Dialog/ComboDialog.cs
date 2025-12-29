using System;
using R3;
using UnityEngine;

namespace MotorHome.Scripts
{
    public class ComboDialog : IDisposable
    {
        public bool IsStarted { get; private set; }=  false;
        
        private readonly IDialogShower _npcDialogShower;
        private readonly IDialogShower _playerDialogShower;
        
        private IDialogShower _currentDialogShower = null;
        private IDisposable _disposable;
        
        public ComboDialog(IDialogShower npc, IDialogShower player)
        {
            _npcDialogShower = npc;
            _playerDialogShower = player;
        }

        public Observable<Unit> StartDialog(PhrasesOneManData[] phrasesOneManDatas)
        {
            Dispose();
            IsStarted = true;
            Subject<Unit> startSubject = new();
            _disposable = startSubject;
            Subject<Unit> endSubject;
            
            var lastSubject = startSubject;
            for (var i = 0; i < phrasesOneManDatas.Length; i++)
            {
                
                var joined = new Subject<Unit>();
                _disposable = Disposable.Combine(_disposable, joined);

                var ai = i;
                lastSubject.Subscribe(_ =>
                {
                    if (phrasesOneManDatas[ai].WhoTalking == PhrasesOneManData.WhoTalk.Npc)
                    {
                        _currentDialogShower = _npcDialogShower;
                        _npcDialogShower.Show(phrasesOneManDatas[ai].Phrases).Take(1)
                            .Subscribe(_ =>
                            {
                                joined.OnNext(Unit.Default);
                            });
                    }
                    else
                    {
                        _currentDialogShower = _playerDialogShower;
                        _playerDialogShower.Show(phrasesOneManDatas[ai].Phrases).Take(1)
                            .Subscribe(_ =>
                            {
                                joined.OnNext(Unit.Default);
                            });
                    }
                });
                lastSubject = joined;
            }
            
            endSubject = lastSubject;
            endSubject.Subscribe(_ =>
            {
                IsStarted = false;
                _currentDialogShower = null;
            });
            startSubject.OnNext(Unit.Default);
            
            return endSubject;
        }

        public void Click()
        {
            if(_currentDialogShower != null)
                _currentDialogShower.Click();
            else
                Debug.LogError("No dialog shower");
        }

        public void Dispose()
        {
            _disposable?.Dispose();
            _currentDialogShower = null;
            _npcDialogShower.Clear();
            _playerDialogShower.Clear();
        }
    }
}