using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CountManager : MonoBehaviour
{
    [Header("Count Display")]
    public TMP_Text countText; // UI text element to display the count
    public string targetTag = "Item"; // Tag to track
    public string countFormat = "Count: {0}"; // Format string for the count display

    [Header("Debug")]
    public bool showDebugInfo = true; // Whether to print debug information

    // Shared dictionary of counts across all CountManager instances
    private static Dictionary<string, int> tagCounts = new Dictionary<string, int>();

    void Start()
    {
        // Initialize target tag entry if needed
        if (!tagCounts.ContainsKey(targetTag))
        {
            tagCounts[targetTag] = 0;
        }

        // Sync the UI with the current count
        UpdateCountDisplay();

        if (showDebugInfo)
        {
            Debug.Log($"CountManager: Initialization complete for tag: {targetTag}");
        }
    }

    void Update()
    {
        // Update the count UI every frame
        UpdateCountDisplay();
    }

    // Update the UI to reflect the latest count
    private void UpdateCountDisplay()
    {
        if (countText != null)
        {
            int count = tagCounts.ContainsKey(targetTag) ? tagCounts[targetTag] : 0;
            countText.text = string.Format(countFormat, count);
        }
    }

    // Increment the count for the given tag
    public static void IncrementTagCount(string tag)
    {
        if (tagCounts.ContainsKey(tag))
        {
            tagCounts[tag]++;
        }
        else
        {
            tagCounts[tag] = 1;
        }

        Debug.Log($"CountManager: Tag '{tag}' incremented to {tagCounts[tag]}");
    }

    // Get the count for the given tag
    public static int GetTagCount(string tag)
    {
        return tagCounts.ContainsKey(tag) ? tagCounts[tag] : 0;
    }

    // Reset the count for the given tag
    public static void ResetTagCount(string tag)
    {
        if (tagCounts.ContainsKey(tag))
        {
            tagCounts[tag] = 0;
        }
    }

    // Reset all tracked counts
    public static void ResetAllCounts()
    {
        tagCounts.Clear();
    }
}
