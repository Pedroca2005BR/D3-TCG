using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PreGameManager : MonoBehaviour
{
    [Header("Rules")]
    [SerializeField] JM_RulesObject rules;

    [Header("Slots")]
    [SerializeField] DeckSlot slotP1;
    [SerializeField] DeckSlot slotP2;

    //[Header("Gamemode")]
    //[SerializeField] Dropdown dropdown;

    [Header("Scenes")]
    [SerializeField] string multiplayerScene;
    [SerializeField] string IAScene;

    public void ChangeGamemode(int index)
    {
        switch (index)
        {
            case 0:
                slotP1.gameObject.SetActive(true);
                slotP2.gameObject.SetActive(true);
                break;
            case 1:
                slotP1.gameObject.SetActive(true);
                slotP2.gameObject.SetActive(false);
                break;

        }

        rules.gameMode = (GameMode)index;

    }

    public async void Play()
    {
        if (rules.gameMode == GameMode.IA)
        {
            if (slotP1.TryGetDTO(out string deck1))
            {
                rules.deck1 = await DeckAddressableLoader.LoadDeckAsync(deck1);
                rules.deck2 = null;
            }
            else
            {
                Debug.LogError("Escolhe o deck, parceiro!");
                return;
            }

            SceneManager.LoadScene(IAScene);
        }
        else if (rules.gameMode == GameMode.MultiplayerLocal)
        {
            if (slotP1.TryGetDTO(out string deck1) && slotP2.TryGetDTO(out string deck2))
            {
                rules.deck1 = await DeckAddressableLoader.LoadDeckAsync(deck1);
                rules.deck2 = await DeckAddressableLoader.LoadDeckAsync(deck2);
            }
            else
            {
                Debug.LogError("Escolhe os decks, parceiro!");
                return;
            }

            SceneManager.LoadScene(multiplayerScene);
        }
    }
}
