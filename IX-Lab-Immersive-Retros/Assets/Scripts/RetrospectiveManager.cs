
using UnityEngine;
using TMPro;

public class RetrospectiveManager : MonoBehaviour
{
    public enum Phase
    {
        Introduction,
        Brainstorm,
        Discussion,
        Voting,
        Results,
        Finished
    }

    [Header("Current Phase")]
    public Phase currentPhase = Phase.Introduction;

    [Header("Screen Text References")]
    public TMP_Text screen1Text;
    public TMP_Text screen2Text;
    public TMP_Text screen3Text;
    public TMP_Text screen4Text;

    private void Start()
    {
        UpdateScreens();
    }

    public void NextPhase()
    {
        if (currentPhase == Phase.Finished)
            return;

        currentPhase++;

        UpdateScreens();
    }

    public void PreviousPhase()
    {
        if (currentPhase == Phase.Introduction)
            return;

        currentPhase--;

        UpdateScreens();
    }

    private void UpdateScreens()
    {
        screen1Text.text = GetScreenText(1);
        screen2Text.text = GetScreenText(2);
        screen3Text.text = GetScreenText(3);
        screen4Text.text = GetScreenText(4);
    }

    private string GetScreenText(int screen)
    {
        switch (currentPhase)
        {
            case Phase.Introduction:
                return "Welkom bij de Retrospective!";

            case Phase.Brainstorm:
                return screen == 1
                    ? "Brainstorm\n\nVoeg ideeën toe."
                    : "De brainstorm is bezig.";

            case Phase.Discussion:
                return screen == 2
                    ? "Discussie\n\nBespreek de ideeën."
                    : "De discussie is bezig.";

            case Phase.Voting:
                return screen == 3
                    ? "Voting\n\nBreng je stem uit."
                    : "Er kan gestemd worden.";

            case Phase.Results:
                return screen == 4
                    ? "Resultaten\n\nBekijk de uitslag."
                    : "De stemming is afgelopen.";

            case Phase.Finished:
                return "De Retrospective is afgelopen!";

            default:
                return "";
        }
    }
}
