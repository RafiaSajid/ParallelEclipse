using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "RiddleData", menuName = "Riddles/New Riddle")]
public class RiddleData : ScriptableObject
{
    public string scrambled; // scrambled word
    public string answer;    // correct answer
    public string hint;      // hint for the riddle
}
