using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private Button[] characterButtons;
    [SerializeField] private Button readyButton;
    [SerializeField] private Color readyColor = Color.green;
    [SerializeField] private Color notReadyColor = Color.red;
    [SerializeField] private Color selectedCharacterColor = Color.yellow;
    private string selectedPrefabName;
    private bool isReady = false;
    private Button lastSelectedCharacterButton;

    private void Start()
    {
        if (!ValidateComponents())
        {
            Debug.LogError("PlayerUI: Some components are not assigned. Please check the Inspector.");
            return;
        }
        InitializeUI();
    }

    private bool ValidateComponents()
    {
        return nameInputField != null && characterButtons != null && characterButtons.Length > 0 && readyButton != null;
    }

    private void InitializeUI()
    {
        foreach (var button in characterButtons)
        {
            button.onClick.AddListener(() => SelectCharacter(button));
        }
        readyButton.onClick.AddListener(ToggleReady);
        UpdateReadyButtonColor();
        nameInputField.onValueChanged.AddListener(UpdateName);
        string savedName = PlayerPrefs.GetString("PlayerName", "");
        if (!string.IsNullOrEmpty(savedName))
        {
            nameInputField.text = savedName;
            UpdateName(savedName);
        }
    }

    private void SelectCharacter(Button selectedButton)
    {
        if (lastSelectedCharacterButton != null)
        {
            lastSelectedCharacterButton.GetComponent<Image>().color = Color.white;
        }

        selectedPrefabName = selectedButton.name;
        lastSelectedCharacterButton = selectedButton;
        selectedButton.GetComponent<Image>().color = selectedCharacterColor;

        Debug.Log($"Selected character: {selectedPrefabName}");
        UpdatePlayerInfo();
    }

    private void UpdateName(string newName)
    {
        PlayerPrefs.SetString("PlayerName", newName);
        UpdatePlayerInfo();
    }

    private void UpdatePlayerInfo()
    {
        if (!string.IsNullOrEmpty(nameInputField.text) && !string.IsNullOrEmpty(selectedPrefabName))
        {
            PlayerManager.Instance.SetLocalPlayerInfo(nameInputField.text, selectedPrefabName, isReady);
        }
        UpdateReadyButtonInteractability();
    }

    private void ToggleReady()
    {
        if (string.IsNullOrEmpty(nameInputField.text) || string.IsNullOrEmpty(selectedPrefabName))
        {
            Debug.LogWarning("Cannot toggle ready: Name or character not selected");
            return;
        }
        isReady = !isReady;
        UpdatePlayerInfo();
        UpdateReadyButtonColor();
        if (isReady)
        {
            PlayerManager.Instance.SetPlayerReady(WebRTCEngine.Instance.LocalPeerId);
        }
        else
        {
            PlayerManager.Instance.SetPlayerNotReady(WebRTCEngine.Instance.LocalPeerId);
        }
        Debug.Log($"Player {nameInputField.text} ready status toggled to: {isReady}");
    }

    private void UpdateReadyButtonColor()
    {
        Image buttonImage = readyButton.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = isReady ? readyColor : notReadyColor;
        }
        TextMeshProUGUI buttonText = readyButton.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.text = isReady ? "Ready" : "Not Ready";
        }
    }

    private void UpdateReadyButtonInteractability()
    {
        readyButton.interactable = !string.IsNullOrEmpty(nameInputField.text) && !string.IsNullOrEmpty(selectedPrefabName);
    }

    private void Update()
    {
        // Ensure the last selected character button stays visually selected
        if (lastSelectedCharacterButton != null &&
            EventSystem.current.currentSelectedGameObject != lastSelectedCharacterButton.gameObject)
        {
            lastSelectedCharacterButton.GetComponent<Image>().color = selectedCharacterColor;
        }
    }
}