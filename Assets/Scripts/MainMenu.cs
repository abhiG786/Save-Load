using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;



public class MainMenu : MonoBehaviour
{
    public Button playButton;  // Reference to the Play button
    public Button loadButton;  // Reference to the Load button
    public Button exitButton;  // Reference to the Exit button
    public GameObject playerPrefab; // Reference to the player prefab

    private string saveFilePath;

    void Start()
    {
        // Set the save file path to a location on the player's computer
        saveFilePath = Path.Combine(Application.persistentDataPath, "savefile.json");

        // Add listeners for the buttons
        if (playButton != null) playButton.onClick.AddListener(PlayGame);
        if (loadButton != null) loadButton.onClick.AddListener(LoadGame);
        if (exitButton != null) exitButton.onClick.AddListener(ExitGame);
    }

    // This method is called when the Play button is clicked
    public void PlayGame()
    {
        // Load Level1 or the initial game level
        SceneManager.LoadScene("Level1");
    }

    // This method is called when the Load button is clicked
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
                StartCoroutine(LoadPlayerPosition(data.playerPosition)); // Load player position after the scene is loaded
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
    private IEnumerator LoadPlayerPosition(Vector3 position)
    {
        yield return new WaitForSeconds(0.5f); // Ensure the scene is loaded
        GameObject player = GameObject.FindWithTag("Player"); // Find the player GameObject by tag
        if (player == null)
        {
            // If the player object is not found, instantiate it
            player = Instantiate(playerPrefab, position, Quaternion.identity);
            player.tag = "Player"; // Ensure it has the Player tag
            Debug.Log("Player instantiated at position: " + position);
        }
        else
        {
            player.transform.position = position; // Set the player's position
            Debug.Log("Player position loaded: " + position);
        }
    }

    // This method is called when the Exit button is clicked
    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game is exiting");  // For testing in the editor
    }

    // Method to save the current game state (level and player position)
    public void SaveGame()
    {
        string currentLevel = SceneManager.GetActiveScene().name; // Get the current scene name
        SaveData data = new SaveData
        {
            levelName = currentLevel // Save the level name
        };

        GameObject player = GameObject.FindWithTag("Player"); // Find your player GameObject by tag
        if (player != null)
        {
            data.playerPosition = player.transform.position; // Save player's position
        }
        else
        {
            Debug.Log("Player not found, cannot save position.");
            return;
        }

        // Convert game data to JSON and save it to a file
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(saveFilePath, json);

        Debug.Log("Game saved at level: " + currentLevel + " with position: " + data.playerPosition);
    }
}
