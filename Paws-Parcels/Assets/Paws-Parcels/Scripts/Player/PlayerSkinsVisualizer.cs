using System;
using System.Collections.Generic;
using Paws_Parcels.Scripts;
using R3;
using UnityEngine;
using UnityEngine.Serialization;

namespace Paws_Parcels.Scripts.Player
{
    public class PlayerSkinsVisualizer : BaseVisualizer
    {
        [Serializable]
        private sealed class SkinVisualSetup
        {
            [SerializeField] [FormerlySerializedAs("_skinName")] private string _skinId;
            [SerializeField] private GameObject[] _objectsToSetActive = Array.Empty<GameObject>();

            public string SkinId => _skinId;
            public IReadOnlyList<GameObject> ObjectsToSetActive => _objectsToSetActive;
        }

        [SerializeField] private GameObject[] _objectsToShowWhenNoSkin = Array.Empty<GameObject>();
        [SerializeField] private List<SkinVisualSetup> _skinsVisuals = new();

        protected override void OnEnable()
        {
            base.OnEnable();

            ApplySkin(_gameProxy.AppliedSkinName.Value);
            _disposable.Add(_gameProxy.AppliedSkinName.Subscribe(ApplySkin));
        }

        private void ApplySkin(string appliedSkinId)
        {
            var activeObjects = new HashSet<GameObject>();

            if (string.IsNullOrWhiteSpace(appliedSkinId))
            {
                AddObjectsToSet(_objectsToShowWhenNoSkin, activeObjects);
            }
            else
            {
                foreach (var skinVisual in _skinsVisuals)
                {
                    if (skinVisual == null || skinVisual.SkinId != appliedSkinId)
                    {
                        continue;
                    }

                    AddObjectsToSet(skinVisual.ObjectsToSetActive, activeObjects);
                    break;
                }
            }

            foreach (var skinVisual in _skinsVisuals)
            {
                if (skinVisual == null)
                {
                    continue;
                }

                foreach (var visualObject in skinVisual.ObjectsToSetActive)
                {
                    if (visualObject != null)
                    {
                        visualObject.SetActive(activeObjects.Contains(visualObject));
                    }
                }
            }

            foreach (var visualObject in _objectsToShowWhenNoSkin)
            {
                if (visualObject != null)
                {
                    visualObject.SetActive(activeObjects.Contains(visualObject));
                }
            }
        }

        private static void AddObjectsToSet(IReadOnlyList<GameObject> sourceObjects, ISet<GameObject> targetSet)
        {
            if (sourceObjects == null)
            {
                return;
            }

            foreach (var visualObject in sourceObjects)
            {
                if (visualObject != null)
                {
                    targetSet.Add(visualObject);
                }
            }
        }
    }
}
