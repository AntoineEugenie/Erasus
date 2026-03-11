using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Levels to Load")]
    public string newGameLevel;
    private string _levelToLoad;

    [SerializeField] private GameObject noSavedGameDialog;
    
    
    public void NewGameDialogYes()
    {
        SceneManager.LoadScene(newGameLevel);
    }

    public void LoadGameDialogYes()
    {
        if (PlayerPrefs.HasKey("savedLevel"))
        {
            _levelToLoad = PlayerPrefs.GetString("savedLevel");
            SceneManager.LoadScene(_levelToLoad);
        }
        else
        {
            noSavedGameDialog.SetActive(true);
        }
    }
    
    public void ExitButton()
    {
        Application.Quit();
    }
}
