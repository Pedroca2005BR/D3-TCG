using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] string sceneName;

    public void PlayGame()
    {
        SceneManager.LoadScene(sceneName);
    }
}
