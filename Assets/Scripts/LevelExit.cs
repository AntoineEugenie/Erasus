using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    public string spawnName;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("TRIGGER" );
        if (GameManager.instance.isLoadingSave) return;
        if (sceneToLoad != null)
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                Debug.Log("PLAYEEEER");
                GameManager.instance.playerController.player.spawnName = spawnName;
                SceneManager.LoadScene(sceneToLoad);
            }

        }
    }
}
