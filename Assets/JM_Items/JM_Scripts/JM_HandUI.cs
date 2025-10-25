using UnityEngine;
using System.Collections.Generic;

public class JM_HandUI : MonoBehaviour
{
    public List<CardInstance> cardsInHand = new List<CardInstance>();

    public JM_TurnController turnController;

    [Header("Configurações")]

    public float cardSpace = 0.5f;
    public bool turnDown = false;
    public bool isPlayer1Hand = false;
    
    public void UpdateHandUI()
    {

        turnController.handManager.UpdateHand();
        
        if (gameObject.CompareTag("P1Hand")) isPlayer1Hand = true;

        

        List<CardInstance> cardsInHandMode = cardsInHand.FindAll(c => c.Mode == CardInstance.CardMode.InHand);
        int totalCards = cardsInHandMode.Count;
        if (totalCards == 0) return;

        float centerIndex = (totalCards - 1) / 2f;
        
        for (int i = 0; i < totalCards; i++)
        {
            CardInstance card = cardsInHandMode[i];
            float xPosition = (i - centerIndex) * cardSpace;
            
            Vector3 targetLocalPosition = new Vector3(
                xPosition, 
                0f, 
                0f  
            );

            Quaternion targetLocalRotation;

            if (!isPlayer1Hand) targetLocalRotation = Quaternion.Euler(0, 0, 180);
            else targetLocalRotation = Quaternion.identity;

            if (isPlayer1Hand && turnController.currentState == GameStates.p1Choosing)
            {
                card.frontSide.SetActive(true);
                card.backSide.SetActive(false);
            }
            else if (!isPlayer1Hand && turnController.currentState == GameStates.p2Choosing)
            {
                card.frontSide.SetActive(true);
                card.backSide.SetActive(false);
            }
            else
            {
                card.frontSide.SetActive(false);
                card.backSide.SetActive(true);
            }
            
            card.transform.SetLocalPositionAndRotation(targetLocalPosition, targetLocalRotation);
        }
    }
}