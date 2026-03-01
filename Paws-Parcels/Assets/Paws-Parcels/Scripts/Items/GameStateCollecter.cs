#if UNITY_EDITOR
using Paws_Parcels.Scripts.Player;
using Paws_Parcels.Scripts.Saves.States.GameState;
using UnityEditor;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

namespace Paws_Parcels.Scripts
{
    public static class GameStateCollecter
    {
        [MenuItem("Tools/Collect scene data")]
        private static void CollectSceneData()
        {
            if (Application.isPlaying)
            {
                Debug.LogWarning("Game is already running");
                return;
            }

            var so = Resources.Load<BaseGameSO>("BaseStates/BaseGameSO");
            
            //player
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                so.GameState.PlayerData.Position = player.transform.position;
                so.GameState.PlayerData.Rotation = player.transform.rotation;
            }
            else
            {
                Debug.LogWarning("No player found");
            }
            
            //car
            var car = GameObject.FindGameObjectWithTag("Car");
            if (car != null)
            {
                so.GameState.CarData.Position = car.transform.position;
                so.GameState.CarData.Rotation = car.transform.rotation;
            }
            else
            {
                Debug.LogWarning("No car found");
            }
            
            //items
            var items = GameObject.FindGameObjectsWithTag("Item");
            if (items != null && items.Length > 0)
            {
                so.GameState.ItemsInWorld.Clear();
                foreach (var itemGameObject in items)
                {
                    var item = itemGameObject.GetComponent<Item>();
                    so.GameState.ItemsInWorld.Add(new ItemData()
                    {
                        ItemTypeId = item.ItemTypeId,
                        DestinationId = item.DestinationId,
                        Position = itemGameObject.transform.position,
                        Rotation = itemGameObject.transform.rotation,
                        AngularVelocity = Vector3.zero,
                        LinearVelocity = Vector3.zero,
                    });
                }
#if UNITY_EDITOR
                EditorUtility.SetDirty(so);
                // Дополнительно: сохраняем изменения в файл
                AssetDatabase.SaveAssetIfDirty(so);
                AssetDatabase.Refresh();
#endif
            }
            else
            {
                Debug.LogWarning("No items found");
            }
            
            Debug.Log("Data collected");
            //no quests, need write hands
        }
    }
}
#endif