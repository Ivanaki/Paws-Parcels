using System;
using System.Collections.Generic;
using System.Linq;
using MotorHome.Scripts.Saves;

using R3;
using UnityEngine;
using Zenject;

namespace MotorHome.Scripts
{
    public class CoreSaver : IDisposable
    {
        private readonly CoreUI _coreUI;
        private readonly GameStateProvider _gameStateProvider;
        private readonly CorePause _corePause;
        private readonly UIActiveCoreManager _activeCoreManager;

        public CoreSaver(CoreUI coreUI, GameStateProvider gameStateProvider, CorePause corePause,
            UIActiveCoreManager activeCoreManager)
        {
            _coreUI = coreUI;
            _gameStateProvider = gameStateProvider;
            _corePause = corePause;
            _activeCoreManager = activeCoreManager;
        }

        private Dictionary<int, CoreSaveData> _saveData = new();
        private int _currentSaveNumber = -1;
        private int _currentSelectedNumber;

        public void Start(LoadType loadType, int saveNumber)
        {
            _activeCoreManager.CurrentUI.Subscribe(value =>
            {
                if (value == UIEnum.Pause)
                {
                    _coreUI.CoreSaverUI.Hide();
                }
            });
            
            _coreUI.CoreSaverUI.Hide();
            _coreUI.LoadPressed.Subscribe(_ =>
            {
                UpdateSavesList();
                _coreUI.CoreSaverUI.Show();
                _activeCoreManager.CurrentUI.Value = UIEnum.LoadPanel;
            });
            
            _coreUI.CoreSaverUI.ExitButtonPressed.Subscribe(_ =>
            {
                _coreUI.CoreSaverUI.Hide();
                _activeCoreManager.CurrentUI.Value = UIEnum.Pause;
            });

            _coreUI.SavePressed.Subscribe(_ =>
            {
                Save("User save");
                _corePause.HidePause();
                _activeCoreManager.CurrentUI.Value = UIEnum.Game;
            });

            _coreUI.CoreSaverUI.NewSelected.Subscribe(ShowSelectedSave);
            
            _coreUI.CoreSaverUI.LoadButtonPressed.Subscribe(_ =>
            {
                _gameStateProvider.LoadCoreState(_currentSelectedNumber);
                _coreUI.CoreSaverUI.Hide();
                _corePause.HidePause();
                _activeCoreManager.CurrentUI.Value = UIEnum.Game;
            });

            _coreUI.CoreSaverUI.DeleteButtonPressed.Subscribe(_ => Delete(_currentSelectedNumber.ToString()));


            var data = _gameStateProvider.LoadCoreSaverData();
            _currentSaveNumber = data.SaveNumber;
            _saveData = data.SaveData;
            
            
            if (loadType == LoadType.New)
            {
                _gameStateProvider.LoadCoreState(-2);
            }
            if(loadType == LoadType.Continue)
            {
                _gameStateProvider.LoadCoreState(saveNumber);
            }
        }

        private int GetLastSaveNumber()
        {
            if(_saveData.Keys.Count >0)
                return _saveData.Keys.Last();
            return -1;
        }

        private void ShowSelectedSave(int number)
        {
            _currentSelectedNumber =  number;
            
            Texture2D texture2D;
            var callback = _gameStateProvider.LoadScreenshot(number.ToString());
            if (callback.IsLoaded)
            {
                texture2D =  callback.Texture;
            }
            else
            {
                texture2D = new Texture2D(1, 1);
            }
            
            var hasKey = _saveData.TryGetValue(number, out CoreSaveData saveData);
            if (hasKey)
            {
                _coreUI.CoreSaverUI.ShowSelectedSave(saveData, texture2D);
            }
            else
            {
                _coreUI.CoreSaverUI.ShowSelectedSave(new CoreSaveData(0,"-", DateTime.Now, "-"), 
                    new Texture2D(1,1));
            }
        }

        private void Delete(string saveNumber)
        {
            _gameStateProvider.DeleteCoreState(_currentSelectedNumber);
            _saveData.Remove(_currentSelectedNumber);
            
            _gameStateProvider.SaveCoreSaverData(new BaseCoreSaverData(_currentSaveNumber, _saveData));
            
            UpdateSavesList();
        }
        
        public void Save(string saveType)
        {
            _currentSaveNumber++;
            _gameStateProvider.SaveCoreState(_currentSaveNumber.ToString());
            _gameStateProvider.CaptureScreenshot(_currentSaveNumber.ToString());
            
            _saveData.Add(_currentSaveNumber, new CoreSaveData(_currentSaveNumber, "mission name", DateTime.Now, saveType));

            _gameStateProvider.SaveCoreSaverData(new BaseCoreSaverData(_currentSaveNumber, _saveData));
        }
        
        private void UpdateSavesList()
        {
            var data = _gameStateProvider.LoadCoreSaverData();
                
            _currentSaveNumber = data.SaveNumber;
            _saveData = data.SaveData;
                
            _coreUI.CoreSaverUI.UpdateLoading(_saveData);
            _currentSelectedNumber = GetLastSaveNumber();
            ShowSelectedSave(_currentSelectedNumber);
            
        }
        
        public void Dispose()
        {
            
        }
    }
}