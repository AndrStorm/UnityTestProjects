using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UILevelCounter : MonoBehaviour
{

    public TextMeshProUGUI counter;
    public void CountLevel(int level)
    {
        counter.text = $"Level: {level}";
    }
    
}
