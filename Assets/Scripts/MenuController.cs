using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections; // Include for IEnumerator



public class MenuController : MonoBehaviour
{
    public GameObject menuPanel;      // A panel containing the Resume, Save, and Exit buttons
    public Button menuButton;         // The Menu button
    public Button restartButton;      // The Restart button
    public Button saveButton;         // The Save button
    public Button exitButton;         // The Exit button
    public Button loadButton;         // The Load button
    public GameObject playerPrefab;   // Prefab for the player to instantiate if not found

    private string saveFilePath;      // Path to save the game
    private bool isGamePaused = false;

    void Start()
    {
        // Initially hide the menu panel
        menuPanel.SetActive(false);

        // Set the save file path
        saveFilePath = Path.Combine(Application.persistentDataPath, "savefile.json");

        // Add listeners for the buttons
        if (menuButton != null) menuButton.onClick.AddListener(ShowMenu);
        if (restartButton != null) restartButton.onClick.AddListener(RestartGame);
        if (saveButton != null) saveButton.onClick.AddListener(SaveGame);
        if (exitButton != null) exitButton.onClick.AddListener(ExitGame);
        if (loadButton != null) loadButton.onClick.AddListener(LoadGame); // Add listener for Load button
    }

    // Show the menu and pause the game
    void ShowMenu()
    {
        menuPanel.SetActive(true);
        PauseGame();
    }

    // Called when the Resume button is clicked
    public void ResumeGame()
    {
        menuPanel.SetActive(false);
        UnpauseGame();
    }

    // Called when the Save button is clicked
    public void SaveGame()
    {
        SaveData data = new SaveData();
        // Save the current scene name
        data.levelName = SceneManager.GetActiveScene().name;

        // Save player position immediately
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            data.playerPosition = player.transform.position; // Save player's position
            Debug.Log("Saving Player position: " + data.playerPosition); // Debug log for position
        }
        else
        {
            Debug.Log("Player not found, cannot save position.");
            return;
        }

        // Convert game data to JSON and save it to a file
        string json = JsonUtility.ToJson(data, true); // Pretty print JSON for readability
        File.WriteAllText(saveFilePath, json);

        Debug.Log("Game saved to: " + saveFilePath);
    }

    // Called when the Load button is clicked
    public void LoadGame()
    {
        // Check if the save file exists
        if (File.Exists(saveFilePath))
        {
            // Read the saved data from the file
            string json = File.ReadAllText(saveFilePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            // Load the saved level from the saved data
            if (!string.IsNullOrEmpty(data.levelName))
            {
                SceneManager.LoadScene(data.levelName); // Load the saved level
                StartCoroutine(LoadPlayerPositionAfterDelay(data.playerPosition)); // Load player position after the scene is loaded
            }
            else
            {
                Debug.Log("No saved level found.");
            }
        }
        else
        {
            Debug.Log("No save file found.");
        }
    }

    // Coroutine to wait until the level is fully loaded, then set the player position
    private IEnumerator LoadPlayerPositionAfterDelay(Vector3 position)
    {
        yield return new WaitForSeconds(0.5f); // Delay to allow scene to load

        // After the scene is loaded, place the player in the saved position
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = position; // Set the saved position
            Debug.Log("Player position loaded: " + position); // Debug log for loaded position
        }
        else
        {
            // Instantiate player if not found
            player = Instantiate(playerPrefab, position, Quaternion.identity);
            player.tag = "Player"; // Ensure it has the Player tag
            Debug.Log("Player instantiated at position: " + position);
        }
    }

    // Called when the Restart button is clicked
    public void RestartGame()
    {
        // Load the home scene
        SceneManager.LoadScene("Home"); // Replace "Home" with your actual home scene name
    }

    // Called when the Exit button is clicked
    public void ExitGame()
    {
        // Quit the game
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop the editor
#else
        Application.Quit(); // Quit the application
#endif
        Debug.Log("Game is exiting");
    }

    // Pause the game (stop time)
    void PauseGame()
    {
        Time.timeScale = 0; // Freezes the game
        isGamePaused = true;
    }

    // Unpause the game (resume time)
    void UnpauseGame()
    {
        Time.timeScale = 1; // Resumes the game
        isGamePaused = false;
    }
}
