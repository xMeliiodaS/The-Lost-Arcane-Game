using UnityEngine;

public class CharacterSelection : MonoBehaviour
{
    public GameObject[] characters;
    public static int selectedCharacter = 0;

    public void NextCharacter()
    {
        characters[selectedCharacter].SetActive(false); // The currently selected character will be set to not active
        if (selectedCharacter < characters.Length - 1)
        {
            selectedCharacter++; // Go to the next character. (% is to make the selection go in a cicle)
        }
        characters[selectedCharacter].SetActive(true); // Set the current character to active
    }

    public void PreviousCharacter()
    {
        characters[selectedCharacter].SetActive(false);
        if(selectedCharacter > 0)
        {
            selectedCharacter--;
        }
        characters[selectedCharacter].SetActive(true);
    }
}
