using UnityEngine;

public class DeckConstructorGlueCode : MonoBehaviour
{
    [SerializeField] ConfirmationComponent confirmationComponent;
    [SerializeField] DeckRuntimeUI deckRuntimeUI;
    [SerializeField] SceneController sceneController;

    public void GoBack()
    {
        // If deck not saved, try to get confirmation
        if (!deckRuntimeUI.IsCurrentDeckSaved())
        {
            confirmationComponent.ShowConfirmation("Seu deck atual não foi salvo. Tem certeza?", () => sceneController.PlayGame());
        }
        else
        {
            sceneController.PlayGame();
        }
    }
}
