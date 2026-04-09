using System.Collections.Generic;
using Paws_Parcels.Scripts.Saves.States;
using UnityEngine;

namespace Services.Music
{
    public class MainMenuMusicService : RandomPlaylistMusicService
    {
        private readonly EditorDataService _editorDataService;

        public MainMenuMusicService(
            SettingsStateProvider settingsStateProvider,
            EditorDataService editorDataService) : base(settingsStateProvider)
        {
            _editorDataService = editorDataService;
        }

        protected override IReadOnlyList<AudioClip> Tracks => _editorDataService.EditorSettings?.MainMenuMusicTracks;
        protected override string PlayerName => "[MAIN MENU MUSIC]";
    }
}
