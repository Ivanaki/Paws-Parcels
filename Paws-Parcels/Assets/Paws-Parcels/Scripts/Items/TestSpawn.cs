using System.Collections;
using UnityEngine;

namespace Paws_Parcels.Scripts
{
    public class TestSpawn : MonoBehaviour
    {
        [SerializeField] private TestCube _testCube;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(2);
            if(_testCube.gameObject.activeSelf) Debug.LogError(
                @$"Set prefab ""{_testCube.name}"" active = false");
            var t = Instantiate(_testCube);
            t.Construct();
            t.gameObject.SetActive(true);
        }
    }
}