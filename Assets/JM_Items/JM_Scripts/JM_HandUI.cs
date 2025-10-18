using UnityEngine;
using System.Collections.Generic;

public class JM_HandUI : MonoBehaviour
{
    public List<CardInstance> cardsInHand = new List<CardInstance>();

    [Header("Configurações")]
    public float cardSpace = 0.5f;        
    public float maxAngle = 60f;        
    public float yOffset = 0.1f;
    public float zDepth = 0.02f;        
    
    public void UpdateHandUI()
    {
        int totalCards = cardsInHand.Count;
        if (totalCards == 0) return;

        float centerIndex = (totalCards - 1) / 2f;
        float angleTotal = maxAngle;
        float startAngle = -angleTotal / 2f; 

        for (int i = 0; i < totalCards; i++)
        {
            CardInstance card = cardsInHand[i];

            float xPosition = (i - centerIndex) * cardSpace;
            float currentAngle = startAngle + (i * (angleTotal / Mathf.Max(1, totalCards - 1)));
            
            float normalizedArc = Mathf.Abs(i - centerIndex) / centerIndex;
            
            float depthOffset = Mathf.Abs(i - centerIndex) * zDepth;
            
            Vector3 targetLocalPosition = new Vector3(
                xPosition, 
                Mathf.Pow(normalizedArc, 2) * yOffset,
                depthOffset 
            );

            Quaternion targetLocalRotation = Quaternion.Euler(0, currentAngle, 0);

            card.transform.SetLocalPositionAndRotation(targetLocalPosition, targetLocalRotation);
        }
    }
}