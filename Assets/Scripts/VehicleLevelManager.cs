using UnityEngine;
using UnityEngine.SceneManagement;  // Required for scene loading

public class VehicleLevelManager : MonoBehaviour
{
    // Method called when the vehicle enters the finish line trigger
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that enters the trigger is the vehicle
        if (other.CompareTag("Player"))
        {
            // Level complete, load the next level
            LoadNextLevel();
        }
    }

    // This method loads the next level
    void LoadNextLevel()
    {
        // Get the current scene index
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Load the next scene in the build order
        SceneManager.LoadScene(currentSceneIndex + 1);
    }
}
