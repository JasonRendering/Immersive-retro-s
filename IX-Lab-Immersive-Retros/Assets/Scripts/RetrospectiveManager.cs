
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

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

    [Header("Buttons")]
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject previousButton;
    [SerializeField] private GameObject returnButton;
    [SerializeField] private GameObject exitButton;

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

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("mainMenu");
    }

    public void ReturnToSettings()
    {
        SceneManager.LoadScene("retroSettings");
    }

    private void UpdateScreens()
    {
        if (startButton != null)
            startButton.SetActive(currentPhase == Phase.Introduction);
        
        if (returnButton != null)
            returnButton.SetActive(currentPhase == Phase.Introduction);

        if (exitButton != null)
            exitButton.SetActive(currentPhase == Phase.Finished);

        if (nextButton != null)
            nextButton.SetActive(currentPhase != Phase.Introduction && currentPhase != Phase.Finished);
        

        if (previousButton != null)
            previousButton.SetActive(currentPhase != Phase.Introduction);

        screen1Text.text = GetScreenText(1);
        screen2Text.text = GetScreenText(2);
        screen3Text.text = GetScreenText(3);
        screen4Text.text = GetScreenText(4);

        SelectActiveButton();
    }

    private void SelectActiveButton()
    {
        GameObject buttonToSelect = currentPhase == Phase.Introduction
            ? startButton
            : nextButton != null && nextButton.activeInHierarchy
                ? nextButton
                : previousButton;

        NavigationController.SelectButton(buttonToSelect);
    }

    private string GetScreenText(int screen)
    {
        switch (currentPhase)
        {
            case Phase.Introduction:
                return "Welkom bij de Retrospective!";

            case Phase.Brainstorm:
                return screen == 1
                    ? "Brainstorm\nVoeg ideeën toe."
                    : "De brainstorm is bezig.";

            case Phase.Discussion:
                return screen == 2
                    ? "Discussie\nBespreek de ideeën."
                    : "De discussie is bezig.";

            case Phase.Voting:
                return screen == 3
                    ? "Voting\nBreng je stem uit."
                    : "Er kan gestemd worden.";

            case Phase.Results:
                return screen == 4
                    ? "Resultaten\nBekijk de uitslag."
                    : "De stemming is afgelopen.";

            case Phase.Finished:
                return "De Retrospective is afgelopen!";

            default:
                return "";
        }
    }
}
