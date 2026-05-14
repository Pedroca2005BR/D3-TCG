using UnityEngine;

public class GiveUpScript : MonoBehaviour
{
    [SerializeField] private ConfirmationComponent confirmationComponent;

    public void OnGiveUpButtonPressed()
    {
        // If its not any player's turn, it means that the game is in the middle of a transition, so we should not allow giving up at this moment.
        if (GameManager.Instance.turnController.currentState != GameStates.p1Choosing && GameManager.Instance.turnController.currentState != GameStates.p2Choosing)
        {
            return;
        }

        confirmationComponent.ShowConfirmation("Tem certeza que deseja desistir?", () => GiveUp());
    }

    private void GiveUp()
    {
        // If the player 1 has already played, it means that the player 2 is giving up, so player 1 wins. Otherwise, player 2 is giving up, so player 2 wins.
        bool player1Played = GameManager.Instance.turnController.player1Played;
        int givingUpPlayer = player1Played ? 2 : 1;
        GameManager.Instance.turnController.PlayerGaveUp(givingUpPlayer);
    }
}
