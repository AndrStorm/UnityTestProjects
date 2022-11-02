using UnityEngine;
using TMPro;

public class LevelCounter : MonoBehaviour
{
    public TextMeshPro myText;
     

    public void CountLevel(int level)
    {
        myText.text = $"Level: {level}";
        
    }

}
