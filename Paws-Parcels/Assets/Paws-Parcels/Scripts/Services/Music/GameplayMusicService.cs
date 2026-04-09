using System.Collections.Generic;
using Paws_Parcels.Scripts.Saves.States;
using UnityEngine;

namespace Services.Music
{
    public class GameplayMusicService : RandomPlaylistMusicService
    {
        private readonly EditorDataService _editorDataService;

        public GameplayMusicService(
            SettingsStateProvider settingsStateProvider,
            EditorDataService editorDataService) : base(settingsStateProvider)
        {
            _editorDataService = editorDataService;
        }

        protected override IReadOnlyList<AudioClip> Tracks => _editorDataService.EditorSettings?.GameplayMusicTracks;
        protected override string PlayerName => "[GAMEPLAY MUSIC]";
    }
}
