using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager
{
    public class SceneInitializer : MonoBehaviour
    {
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log("Scene charge : " + scene.name);
            if (GameManager.instance.tileManager != null)
            {

            }



        }
    }
}
