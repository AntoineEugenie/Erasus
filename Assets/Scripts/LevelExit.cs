using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    public SceneAsset sceneToLoad;
    public string spawnName;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (sceneToLoad != null)
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                GameManager.instance.playerController.player.spawnName = spawnName;
                SceneManager.LoadScene(sceneToLoad.name);
            }

        }
    }
}
