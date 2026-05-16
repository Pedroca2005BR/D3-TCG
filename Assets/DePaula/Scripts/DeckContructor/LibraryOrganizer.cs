using UnityEngine;

public class LibraryOrganizer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform fireLocation;
    [SerializeField] private Transform waterLocation;
    [SerializeField] private Transform earthLocation;
    [SerializeField] private Transform animalLocation;
    [SerializeField] private Transform spellLocation;

    public Transform GetRightTransform(CardData card)
    {
        switch (card.type)
        {
            case CardType.Fire:
                return fireLocation;
            case CardType.Water:
                return waterLocation;
            case CardType.Earth:
                return earthLocation;
            case CardType.Animal:
                return animalLocation;
            case CardType.Spell:
                return spellLocation;
            default:
                //AudioManager.instance.PlaySound("Explosion");
                return transform;
        }
    }

    public void ClearLibrary()
    {
        ClearLocation(fireLocation);
        ClearLocation(waterLocation);
        ClearLocation(earthLocation);
        ClearLocation(animalLocation);
        ClearLocation(spellLocation);
    }

    public void ToggleDisponibilityTargetCard(CardData card, bool available)
    {
        
        var location = GetRightTransform(card);

        //InBuildDebugger.instance.Log(location, card);

        for (int i = 0; i < location.childCount; i++)
        {
            var child = location.GetChild(i);
            var display = child.GetComponent<CardDisplay>();

            //if (display != null) AudioManager.instance.PlaySound("Explosion");
            //if (card != null) AudioManager.instance.PlaySound("Mimir");

            //InBuildDebugger.instance.Log(display.cardData);

            if (display != null && display.cardData.addressableKey == card.addressableKey)
            {
                display.ToggleDisponibility(available);
            }
        }
        //AudioManager.instance.PlaySound("Explosion");
    }

    public void ToggleDisponibilityAllCards(bool available)
    {
        ToggleDisponibilityForLocation(fireLocation, available);
        ToggleDisponibilityForLocation(waterLocation, available);
        ToggleDisponibilityForLocation(earthLocation, available);
        ToggleDisponibilityForLocation(animalLocation, available);
        ToggleDisponibilityForLocation(spellLocation, available);
    }

    private void ToggleDisponibilityForLocation(Transform location, bool available)
    {
        for (int i = 0; i < location.childCount; i++)
        {
            var child = location.GetChild(i);
            var display = child.GetComponent<CardDisplay>();
            if (display != null)
            {
                display.ToggleDisponibility(available);
            }
        }
    }

    private void ClearLocation(Transform location)
    {
        for (int i = 0; i < location.childCount; i++)
        {
            var child = location.GetChild(i);
            if (Application.isPlaying)
                Destroy(child.gameObject);
            else
                DestroyImmediate(child.gameObject);
        }
    }
}