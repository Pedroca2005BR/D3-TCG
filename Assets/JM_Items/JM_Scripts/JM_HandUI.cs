using UnityEngine;
using System.Collections.Generic;

public class JM_HandUI : MonoBehaviour
{
    public List<CardInstance> cardsInHand = new List<CardInstance>();

    [Header("Configurações")]

    public float cardSpace = 0.5f; 
    
    public void UpdateHandUI()
    {
        int totalCards = cardsInHand.Count;
        if (totalCards == 0) return;

        float centerIndex = (totalCards - 1) / 2f;
        
        for (int i = 0; i < totalCards; i++)
        {
            CardInstance card = cardsInHand[i];
            float xPosition = (i - centerIndex) * cardSpace;
            
            Vector3 targetLocalPosition = new Vector3(
                xPosition, 
                0f, 
                0f  
            );

            Quaternion targetLocalRotation = Quaternion.identity; 

            card.transform.SetLocalPositionAndRotation(targetLocalPosition, targetLocalRotation);
        }
    }
}