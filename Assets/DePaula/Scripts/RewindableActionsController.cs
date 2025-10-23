using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RewindableActionsController : MonoBehaviour 
{
    #region Singleton
    public static RewindableActionsController Instance { get; private set; }
    // Singleton pattern
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    List<GameAction> actions = new List<GameAction>();

    public async Task CardPlayed(GameAction gameAction)
    {
        //Debug.LogWarning("Time to execute immediatly!");

        actions.Add(gameAction);
        await gameAction.Execute();
    }

    public void RewindAction()
    {
        Debug.Log("Rewind!");
    }
}
