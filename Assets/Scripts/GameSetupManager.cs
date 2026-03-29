using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameSetupManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject setupPanel; // The container that will move (must have RectTransform)
    public GameObject gamePanel;  // The main game UI (to be enabled later)
    
    [Header("Setup Steps")]
    public GameObject welcomeStep;
    public GameObject startGameStep;
    public GameObject playerCountStep;
    public GameObject playerConfigStep;
    
    [Header("Sliding Settings")]
    public float slideDuration = 0.4f;
    private RectTransform setupRect;
    private Coroutine slideCoroutine;

    [Header("Config UI Elements")]
    // ... (rest of headers)
    public Text configTitle; 
    public InputField nameInput;
    public Transform characterGrid; 
    public Button startGameButton; 
    public Button nextButton;
    public Image selectedCharPreview;
    public Text feedbackText;

    [Header("Start Game UI Elements")]
    public Dropdown dayInput;
    public Dropdown monthInput;
    public Dropdown yearInput;
    public InputField codeInput;
    public Text startGameFeedbackText;

    [Header("Resources")]
    public CharacterDatabase charDB; 
    public Transform colorGrid; 

    // Internal State
    private int totalPlayers = 2;
    private int currentPlayerIndex = 0;
    private List<PlayerConfiguration> players = new List<PlayerConfiguration>();
    
    private int selectedModelIndex = -1;
    private int selectedColorIndex = -1;

    void Start()
    {
        // Initial State
        if (setupPanel) 
        {
            setupPanel.SetActive(true);
            setupRect = setupPanel.GetComponent<RectTransform>();
        }
        if (gamePanel) gamePanel.SetActive(false);
        
        // Ensure all steps are active for sliding
        if (welcomeStep) welcomeStep.SetActive(true);
        if (startGameStep) startGameStep.SetActive(true);
        if (playerCountStep) playerCountStep.SetActive(true);
        if (playerConfigStep) playerConfigStep.SetActive(true);

        // Try to load DB if not assigned (auto-find)
        if (charDB == null) charDB = Resources.Load<CharacterDatabase>("CharacterDatabase");

        ShowWelcomeStep();
    }

    private void MoveToStep(int stepIndex)
    {
        if (setupRect == null) return;

        // Assuming each step is exactly the width of the setupPanel or parent container
        // If the steps are child of setupPanel, we move setupPanel relative to its parent (Canvas)
        float stepWidth = setupRect.rect.width;
        if (stepWidth == 0) stepWidth = Screen.width; // Fallback if rect not initialized

        float targetX = -stepIndex * stepWidth;
        
        if (slideCoroutine != null) StopCoroutine(slideCoroutine);
        slideCoroutine = StartCoroutine(SlideTo(targetX));
    }

    private IEnumerator SlideTo(float targetX)
    {
        float elapsed = 0;
        float startX = setupRect.anchoredPosition.x;
        Vector2 pos = setupRect.anchoredPosition;

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / slideDuration;
            // SmoothStep for nicer feel
            t = t * t * (3f - 2f * t);
            
            pos.x = Mathf.Lerp(startX, targetX, t);
            setupRect.anchoredPosition = pos;
            yield return null;
        }

        pos.x = targetX;
        setupRect.anchoredPosition = pos;
        slideCoroutine = null;
    }

    private void ShowWelcomeStep()
    {
        MoveToStep(0);
    }

    public void OnWelcomeFinished()
    {
        ShowStartGameStep();
    }

    private void ShowStartGameStep()
    {
        MoveToStep(1);
        if (startGameFeedbackText) startGameFeedbackText.text = "";
    }

    // ... (rest of methods)
    public void OnLoggedGameSelected()
    {
        bool hasDay = dayInput != null && dayInput.value > 0;
        bool hasMonth = monthInput != null && monthInput.value > 0;
        bool hasYear = yearInput != null && yearInput.value > 0;
        bool hasCode = codeInput != null && !string.IsNullOrWhiteSpace(codeInput.text);

        if (!hasDay || !hasMonth || !hasYear || !hasCode)
        {
            if (startGameFeedbackText) startGameFeedbackText.text = "Selecciona tots els camps de data i omple el codi de joc.";
            return;
        }

        if (startGameFeedbackText) startGameFeedbackText.text = "";

        string day = dayInput.options[dayInput.value].text;
        string month = monthInput.options[monthInput.value].text;
        string year = yearInput.options[yearInput.value].text;
        string date = $"{year}-{month}-{day}";
        string code = codeInput.text;
        
        Debug.Log($"[Placeholder] Logged Game Started. Date: {date}, Code: {code}");
        
        ShowPlayerCountSelection();
    }

    public void OnGuestSessionSelected()
    {
        string date = System.DateTime.Now.ToString("yyyy-MM-dd");
        Debug.Log($"[Placeholder] Guest Session Started. Date: {date}");
        
        ShowPlayerCountSelection();
    }

    public void OnPlayerCountSelected(int count)
    {
        totalPlayers = Mathf.Clamp(count, 2, 4); // Enforce GDD limit (2-4 players)
        players.Clear();
        currentPlayerIndex = 0;
        
        ShowPlayerConfigStep();
    }

    private void ShowPlayerCountSelection()
    {
        MoveToStep(2);
    }

    private void ShowPlayerConfigStep()
    {
        MoveToStep(3);

        // Reset UI for current player
        configTitle.text = $"Configura Personatge {currentPlayerIndex + 1}";
        nameInput.text = "";
        
        selectedModelIndex = -1;
        selectedColorIndex = -1;
        
        if (selectedCharPreview) selectedCharPreview.sprite = null;
        if (selectedCharPreview) selectedCharPreview.color = Color.clear; 
        
        if (feedbackText) feedbackText.text = ""; 
        
        RefreshCharacterGrid();
        if (colorGrid) colorGrid.gameObject.SetActive(false); 
        
        bool isLast = (currentPlayerIndex == totalPlayers - 1);
        if (nextButton) nextButton.gameObject.SetActive(false); 
        if (startGameButton) startGameButton.gameObject.SetActive(false); 
    }

    private void RefreshCharacterGrid()
    {
        if (characterGrid == null || charDB == null) return;

        for(int i=0; i < characterGrid.childCount; i++)
        {
             Button b = characterGrid.GetChild(i).GetComponent<Button>();
             if (b) b.interactable = true;
        }
        
        foreach(var p in players)
        {
            int usedIndex = p.ModelIndex;
            if (usedIndex >= 0 && usedIndex < characterGrid.childCount)
            {
                 Button b = characterGrid.GetChild(usedIndex).GetComponent<Button>();
                 if (b) b.interactable = false;
            }
        }
    }

    public void OnCharacterSelected(int modelIndex)
    {
        if (charDB == null || modelIndex < 0 || modelIndex >= charDB.characters.Count) return;

        selectedModelIndex = modelIndex;
        selectedColorIndex = -1; 
        
        if (colorGrid)
        {
            colorGrid.gameObject.SetActive(true);
            RefreshColorGrid();
        }

        UpdatePreview();
        ValidateCurrentConfig();
    }

    public void OnColorSelected(int colorIndex)
    {
        selectedColorIndex = colorIndex;
        UpdatePreview();
        ValidateCurrentConfig(); 
    }

    private void RefreshColorGrid()
    {
        if (colorGrid == null) return;
        
        for(int i=0; i < colorGrid.childCount; i++)
        {
             Button b = colorGrid.GetChild(i).GetComponent<Button>();
             if (b) b.interactable = true;
        }

        foreach(var p in players)
        {
            int usedColor = p.ColorIndex;
            if (usedColor >= 0 && usedColor < colorGrid.childCount)
            {
                 Button b = colorGrid.GetChild(usedColor).GetComponent<Button>();
                 if (b) b.interactable = false;
            }
        }
    }

    private void UpdatePreview()
    {
        if (selectedCharPreview == null || charDB == null) return;
        
        if (selectedModelIndex >= 0)
        {
            var character = charDB.characters[selectedModelIndex];
            selectedCharPreview.color = Color.white;
            
            int spriteIndex = 0;
            if (selectedColorIndex >= 0) spriteIndex = selectedColorIndex + 1;
            
            if (character.sprites != null && spriteIndex < character.sprites.Length)
            {
                selectedCharPreview.sprite = character.sprites[spriteIndex];
            }
        }
    }

    public void OnNextPlayerButton()
    {
        if (ValidateCurrentConfig())
        {
            SaveCurrentConfig();
            currentPlayerIndex++;
            ShowPlayerConfigStep();
        }
    }

    public void OnStartGameButton()
    {
        if (ValidateCurrentConfig())
        {
            SaveCurrentConfig();
            FinishSetup();
        }
    }

    private bool ValidateCurrentConfig()
    {
        bool nameOk = !string.IsNullOrEmpty(nameInput.text);
        bool modelOk = selectedModelIndex >= 0;
        bool colorOk = selectedColorIndex >= 0;

        bool isValid = nameOk && modelOk && colorOk;
        
        bool isLast = (currentPlayerIndex == totalPlayers - 1);
        if (nextButton) nextButton.gameObject.SetActive(isValid && !isLast);
        if (startGameButton) startGameButton.gameObject.SetActive(isValid && isLast);

        if (!isValid)
        {
            if (!modelOk) if (feedbackText) feedbackText.text = "Selecciona un Model!";
            else if (!colorOk) if (feedbackText) feedbackText.text = "Selecciona un Color!";
            else if (!nameOk) if (feedbackText) feedbackText.text = "Escriu un Nom!";
        }
        else
        {
            if (feedbackText) feedbackText.text = "";
        }

        return isValid;
    }

    private void SaveCurrentConfig()
    {
        PlayerConfiguration pc = new PlayerConfiguration();
        pc.PlayerID = currentPlayerIndex;
        pc.PlayerName = nameInput.text;
        
        if (charDB != null && selectedModelIndex >= 0)
        {
             var c = charDB.characters[selectedModelIndex];
             int sIdx = selectedColorIndex + 1;
             if (sIdx < c.sprites.Length) pc.CharacterSprite = c.sprites[sIdx];
        }

        pc.ModelIndex = selectedModelIndex;
        pc.ColorIndex = selectedColorIndex;
        
        players.Add(pc);
    }

    private void FinishSetup()
    {
        if (setupPanel) setupPanel.SetActive(false);
        if (gamePanel) gamePanel.SetActive(true);

        GameControl.Instance.InitializeGame(players);
    }
}
