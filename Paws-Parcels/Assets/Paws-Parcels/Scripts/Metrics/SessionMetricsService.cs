using System;
using System.Collections.Generic;
using Game.Root;
using Paws_Parcels.Scripts.Saves;
using Saves;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace Paws_Parcels.Scripts.Metrics
{
    [Serializable]
    public class GameSessionMetricsRecord
    {
        public int SchemaVersion = 1;
        public string SessionId;
        public string SessionStatus;
        public string StartSceneName;
        public string StartedAtUtc;
        public string LastUpdatedAtUtc;
        public string EndedAtUtc;
        public double PlaytimeSeconds;
        public int CompletedQuestsCount;
        public List<string> CompletedQuestIds = new();
        public string BuildVersion;
        public string ProductName;
        public string Platform;
        public string EndReason;
    }

    public class SessionMetricsService : IInitializable, IDisposable
    {
        private const int SchemaVersion = 1;
        private const string MetricsFolder = "Metrics";
        private const string SessionsFolder = "Sessions";
        private const string SessionStatusInProgress = "in_progress";
        private const string SessionStatusCompleted = "completed";
        private const string EndReasonSceneExit = "scene_exit";
        private const string EndReasonApplicationQuit = "application_quit";
        private const string EndReasonServiceDispose = "service_dispose";

        private readonly JsonFileSerializer _jsonFileSerializer;
        private readonly Stopwatch _sessionStopwatch = new();

        private GameSessionMetricsRecord _currentSession;
        private bool _applicationIsQuitting;

        public SessionMetricsService(JsonFileSerializer jsonFileSerializer)
        {
            _jsonFileSerializer = jsonFileSerializer;
        }

        public void Initialize()
        {
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
            Application.quitting += OnApplicationQuitting;

            TryStartSessionForScene(SceneManager.GetActiveScene());
        }

        public string GetSessionsDirectoryPath()
        {
            return SaveStatic.BuildFoldersPath(new[] { MetricsFolder, SessionsFolder });
        }

        public void RegisterCompletedQuest(string questId)
        {
            if (_currentSession == null || string.IsNullOrWhiteSpace(questId))
            {
                return;
            }

            if (_currentSession.CompletedQuestIds.Contains(questId))
            {
                return;
            }

            _currentSession.CompletedQuestIds.Add(questId);
            SaveCurrentSession(isCompleted: false, endReason: null);
        }

        public void Dispose()
        {
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
            Application.quitting -= OnApplicationQuitting;

            if (_applicationIsQuitting == false)
            {
                FinishSession(EndReasonServiceDispose);
            }
        }

        private void OnActiveSceneChanged(Scene previousScene, Scene nextScene)
        {
            var wasGameplayScene = IsGameplayScene(previousScene.name);
            var isGameplayScene = IsGameplayScene(nextScene.name);

            if (!wasGameplayScene && isGameplayScene)
            {
                StartSession(nextScene.name);
                return;
            }

            if (wasGameplayScene && !isGameplayScene)
            {
                FinishSession(EndReasonSceneExit);
            }
        }

        private void OnApplicationQuitting()
        {
            _applicationIsQuitting = true;
            FinishSession(EndReasonApplicationQuit);
        }

        private void TryStartSessionForScene(Scene scene)
        {
            if (IsGameplayScene(scene.name))
            {
                StartSession(scene.name);
            }
        }

        private bool IsGameplayScene(string sceneName)
        {
            return string.Equals(sceneName, Scenes.CORE, StringComparison.Ordinal);
        }

        private void StartSession(string sceneName)
        {
            if (_currentSession != null)
            {
                return;
            }

            _currentSession = new GameSessionMetricsRecord
            {
                SchemaVersion = SchemaVersion,
                SessionId = Guid.NewGuid().ToString("N"),
                SessionStatus = SessionStatusInProgress,
                StartSceneName = sceneName,
                StartedAtUtc = DateTime.UtcNow.ToString("O"),
                LastUpdatedAtUtc = DateTime.UtcNow.ToString("O"),
                EndedAtUtc = null,
                PlaytimeSeconds = 0d,
                CompletedQuestsCount = 0,
                CompletedQuestIds = new List<string>(),
                BuildVersion = Application.version,
                ProductName = Application.productName,
                Platform = Application.platform.ToString(),
                EndReason = null
            };

            _sessionStopwatch.Reset();
            _sessionStopwatch.Start();

            SaveCurrentSession(isCompleted: false, endReason: null);
        }

        private void FinishSession(string endReason)
        {
            if (_currentSession == null)
            {
                return;
            }

            SaveCurrentSession(isCompleted: true, endReason: endReason);
            _sessionStopwatch.Reset();
            _currentSession = null;
        }

        private void SaveCurrentSession(bool isCompleted, string endReason)
        {
            if (_currentSession == null)
            {
                return;
            }

            var nowUtc = DateTime.UtcNow.ToString("O");
            _currentSession.LastUpdatedAtUtc = nowUtc;
            _currentSession.PlaytimeSeconds = Math.Round(_sessionStopwatch.Elapsed.TotalSeconds, 2);
            _currentSession.CompletedQuestsCount = _currentSession.CompletedQuestIds.Count;

            if (isCompleted)
            {
                _currentSession.SessionStatus = SessionStatusCompleted;
                _currentSession.EndedAtUtc = nowUtc;
                _currentSession.EndReason = endReason;
                _sessionStopwatch.Stop();
            }

            var saveResult = _jsonFileSerializer.Save(_currentSession, _currentSession.SessionId, MetricsFolder, SessionsFolder);
            if (saveResult.IsComplete == false)
            {
                Debug.LogError($"Failed to save session metrics to {GetSessionsDirectoryPath()}: {saveResult.Exception?.Message}");
            }
        }
    }
}
