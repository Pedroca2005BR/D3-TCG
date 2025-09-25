using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardInstance : MonoBehaviour
{
    public enum Mode
    {
        InHand = 0,
        InPlay = 1
    }

    [Header("Card Info")]
    public CardData cardData;

    [Header("Display Info")]
    [SerializeField] TextMeshProUGUI nameComponent;
    [SerializeField] TextMeshProUGUI descriptionComponent;
    [SerializeField] TextMeshProUGUI healthComponent;
    [SerializeField] TextMeshProUGUI attackComponent;
    [SerializeField] Image cardArtComponent;
    [SerializeField] Image backgroundComponent;

    [Header("Fixed Info for all Prefabs")]
    [SerializeField] Image backSideComponent;
    [SerializeField] Sprite backSideArt;

    HealthSystem healthSystem;
    Mode mode;


    public void SetupCardInstance(CardData data)
    {
        // Coloca a informacao da carta na instancia
        cardData = data;

        // Prepara componentes nao visuais
        healthSystem.Setup(cardData.health);
        mode = Mode.InHand;

        // Prepara textos
        nameComponent.text = cardData.cardName;
        descriptionComponent.text = cardData.cardDescription;
        ChangeHealthComponent();
        attackComponent.text = cardData.attack.ToString();

        // Prepara artes
        cardArtComponent.sprite = cardData.cardArt;
        backgroundComponent.sprite = cardData.backgroundArt;
        backSideComponent.sprite = backSideArt;
    }

    #region HealthMethods

    public void TakeDamage(int amount)
    {
        healthSystem.TakeDamage(amount);
        ChangeHealthComponent();
    }

    public void Heal(int amount)
    {
        healthSystem.Heal(amount);
        ChangeHealthComponent();
    }

    private void ChangeHealthComponent()
    {
        // Altera o valor do componente
        healthComponent.text = healthSystem.CurrentHealth.ToString();

        if (healthSystem.IsDamaged())
        {
            healthComponent.color = Color.red;
        }
        else if (healthSystem.WasBuffed)
        {
            healthComponent.color = Color.green;
        }
        else
        {
            healthComponent.color = Color.white;
        }
    }
    #endregion

    public void Flip()
    {
        //backSideComponent.gameObject.SetActive(!backSideComponent.gameObject.activeInHierarchy);
        backSideComponent.enabled = !backSideComponent.enabled;
    }

    public int GetAttackPower()
    {
        return cardData.attack;
    }
}
