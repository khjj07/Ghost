using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TMPLetterAnimator : MonoBehaviour
{
    public TextMeshProUGUI Text;

    public char GetLetter(int index)
    {
        return Text.text[index];
    }
    
    public void Sizing(int index,int size)
    {
        
    }

}
