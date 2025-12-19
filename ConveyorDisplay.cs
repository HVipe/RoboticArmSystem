using UnityEngine;
using TMPro;
using System.Collections.Generic;
using ConveyorSystem;

public class ConveyorDisplay : MonoBehaviour
{
    public ConveyorBelt conveyorBelt;
    public TMP_Text destroyedText;
    public TMP_Text remainingText;
    public TMP_Text goalsText;

    private void Start()
    {
        SubscribeToEvent();
    }

    private void OnEnable()
    {
        SubscribeToEvent();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvent();
    }

    private void SubscribeToEvent()
    {
        if (conveyorBelt != null)
        {
            conveyorBelt.OnGoalsUpdated += UpdateGoalsDisplay;
        }
    }

    private void UnsubscribeFromEvent()
    {
        if (conveyorBelt != null)
        {
            conveyorBelt.OnGoalsUpdated -= UpdateGoalsDisplay;
        }
    }



    private void UpdateGoalsDisplay(List<ConveyorGoal> goals)
    {
        if (goalsText != null)
        {
            goalsText.text = "";
            foreach (var goal in goals)
            {
                goalsText.text += $"{goal.targetTag}: {goal.currentCount}/{goal.targetCount}\n";
            }
        }
    }
}