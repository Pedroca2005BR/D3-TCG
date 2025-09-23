using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsController : MonoBehaviour
{
    [SerializeField] GameObject optionsMenu;

    public void ExitGame()
    {
        Debug.Log("Quiting game...");
        Application.Quit();
    }

    public void OpenOptionsMenu()
    {
        optionsMenu.SetActive(true);
    }

    public void ExitOptionsMenu()
    {
        optionsMenu.SetActive(false);
    }
}
