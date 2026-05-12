using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        Debug.Log("Sc�ne charg�e : " + scene.name);
        if (GameManager.instance.tileManager != null)
        {

        }
    }
}