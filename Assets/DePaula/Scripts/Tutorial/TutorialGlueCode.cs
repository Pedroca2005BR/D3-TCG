using UnityEngine;

public class TutorialGlueCode : MonoBehaviour
{
    [SerializeField] bool firstTimePlayingGame = true;
    [SerializeField] TutorialController tutorialController;
    [SerializeField] ConfirmationComponent confirmationComponent;
    [SerializeField] SceneController sceneController;

    private void Start()
    {
        AudioManager.instance.StopSound("BackMusic");
        AudioManager.instance.PlaySound("BackMusic");

        if (PlayerPrefs.GetInt("HasPlayedBefore", 0) == 0)
        {
            firstTimePlayingGame = true;
        }
        else
        {
            firstTimePlayingGame = false;
        }
    }

    public void TryInitiateGame()
    {
        if (firstTimePlayingGame)
        {
            confirmationComponent.ShowConfirmation("Essa é sua primeira vez jogando o jogo. Deseja ver um tutorial?", PlayTutorial, InitiateNormalGame);
            firstTimePlayingGame = false;
        }
        else
        {
            tutorialController.EndTutorial();
            sceneController.PlayGame();
        }
    }

    private void InitiateNormalGame()
    {
        tutorialController.EndTutorial();
        sceneController.PlayGame();
    }

    public void PlayTutorial()
    {
        tutorialController.EnterTutorialMode();
        sceneController.PlayGame();
    }
}
