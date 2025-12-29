using System;
using System.Collections.Generic;
using System.Linq;
using MotorHome.Scripts.Saves;
using R3;
using UnityEngine;
using Zenject;

namespace MotorHome.Scripts
{
    public class MainMenuSaveLoader : IInitializable
    {
        public Observable<int> LoadThisSave => _loadThisSave;
        
        private readonly Subject<int> _loadThisSave = new();
        private readonly GameStateProvider _gameStateProvider;
        private readonly MainMenuUI _mainMenuUI;
        private readonly CoreSaverUI _coreSaverUI;
        
        private Dictionary<int, CoreSaveData> _saveData = new();
        private int _currentSelectedNumber = -1;
        private int _currentSaveNumber;
        
        public MainMenuSaveLoader(GameStateProvider gameStateProvider, MainMenuUI mainMenuUI)
        {
            _gameStateProvider = gameStateProvider;
            _mainMenuUI = mainMenuUI;
            _coreSaverUI = _mainMenuUI.CoreSaverUI;
        }
        
        public void Initialize()
        {
            _mainMenuUI.LoadGamePressed.Subscribe(_ =>
            {
                UpdateSavesList();
                _coreSaverUI.Show();
            });
            
            _coreSaverUI.NewSelected.Subscribe(ShowSelectedSave);
            
            _coreSaverUI.LoadButtonPressed.Subscribe(_ =>
            {
                _loadThisSave.OnNext(_currentSelectedNumber);
            });

            _coreSaverUI.DeleteButtonPressed.Subscribe(_ => Delete(_currentSelectedNumber));
        }

        private void UpdateSavesList()
        {
            var data = _gameStateProvider.LoadCoreSaverData();
            
            _saveData = data.SaveData;
            _currentSaveNumber = data.SaveNumber;
                
            _coreSaverUI.UpdateLoading(_saveData);
            
            ShowSelectedSave(GetLastSaveNumber());
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
            texture2D = callback.IsLoaded ? callback.Texture : new Texture2D(1, 1);
            
            var hasKey = _saveData.TryGetValue(number, out CoreSaveData saveData);
            if (hasKey)
            {
                _coreSaverUI.ShowSelectedSave(saveData, texture2D);
            }
            else
            {
                _coreSaverUI.ShowSelectedSave(new CoreSaveData(0,"-", DateTime.Now, "-"), 
                    new Texture2D(1,1));
            }
        }
        
        private void Delete(int saveNumber)
        {
            _gameStateProvider.DeleteCoreState(_currentSelectedNumber);
            _saveData.Remove(_currentSelectedNumber);
            
            _gameStateProvider.SaveCoreSaverData(new BaseCoreSaverData(_currentSaveNumber, _saveData));
            
            UpdateSavesList();
        }
    }
}