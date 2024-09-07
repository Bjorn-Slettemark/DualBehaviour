using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private Button[] characterButtons;
    [SerializeField] private Button setReadyButton;
    [SerializeField] private Image setReadyButtonImage;
    [SerializeField] private Color readyColor = Color.green;
    [SerializeField] private Color notReadyColor = Color.red;
    [SerializeField] private Color selectedButtonColor = Color.yellow; // New selection color
    private string selectedPrefabName;
    private bool isReady = false;
    private const string PlayerChannelName = "MultiplayerChannel";
    private Color[] originalButtonColors; // To store original button colors

    private void Start()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        originalButtonColors = new Color[characterButtons.Length];
        for (int i = 0; i < characterButtons.Length; i++)
        {
            string prefabName = characterButtons[i].name;
            int index = i; // Capture the index for the lambda
            characterButtons[i].onClick.AddListener(() => SelectCharacter(prefabName, index));
            originalButtonColors[i] = characterButtons[i].image.color; // Store original color
        }
        setReadyButton.onClick.AddListener(SetPlayerReady);
        UpdateReadyButtonColor();
        nameInputField.onValueChanged.AddListener(UpdateName);
    }

    private void SelectCharacter(string prefabName, int selectedIndex)
    {
        selectedPrefabName = prefabName;
        Debug.Log($"Selected character: {prefabName}");

        // Update button colors
        for (int i = 0; i < characterButtons.Length; i++)
        {
            if (i == selectedIndex)
            {
                characterButtons[i].image.color = selectedButtonColor;
            }
            else
            {
                characterButtons[i].image.color = originalButtonColors[i];
            }
        }

        UpdateReadyButtonInteractability();
    }

    private void UpdateName(string newName)
    {
        UpdateReadyButtonInteractability();
    }

    private void UpdateReadyButtonInteractability()
    {
        setReadyButton.interactable = !string.IsNullOrEmpty(nameInputField.text) && !string.IsNullOrEmpty(selectedPrefabName);
    }

    private void SetPlayerReady()
    {
        if (string.IsNullOrEmpty(nameInputField.text) || string.IsNullOrEmpty(selectedPrefabName))
        {
            Debug.LogWarning("Cannot set player ready: Name or character not selected");
            return;
        }
        string localPeerId = WebRTCEngine.Instance.LocalPeerId;
        // Send player info message
        string playerInfoMessage = $"PlayerInfo:{localPeerId}:{nameInputField.text}:{selectedPrefabName}";
        EventChannelManager.Instance.RaiseNetworkEvent(PlayerChannelName, playerInfoMessage);
        // Send player ready message
        string playerReadyMessage = $"PlayerReady:{localPeerId}";
        EventChannelManager.Instance.RaiseNetworkEvent(PlayerChannelName, playerReadyMessage);
        isReady = true;
        UpdateReadyButtonColor();
        setReadyButton.interactable = false;
        Debug.Log($"Player {nameInputField.text} is ready with character {selectedPrefabName}");

        // Disable all character buttons when ready
        foreach (Button button in characterButtons)
        {
            button.interactable = false;
        }
    }

    private void UpdateReadyButtonColor()
    {
        setReadyButtonImage.color = isReady ? readyColor : notReadyColor;
    }
}