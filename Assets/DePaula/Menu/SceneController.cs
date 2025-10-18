using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] string gameSceneName;

    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}
