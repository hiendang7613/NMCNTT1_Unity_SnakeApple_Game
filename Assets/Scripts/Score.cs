using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Score : MonoBehaviour
{
    public GameObject scoretext;

    private void Start()
    {
        scoretext.GetComponent<Text>().text = Snake.score.ToString();
    }

}
