using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static int PlayerScore1 = 0;
    public static int PlayerScore2 = 0;
    // Start is called before the first frame update

    GameObject theBall;

    public TextMesh pscore1;
    public TextMesh pscore2;
    public TextMesh gameMessage;

    void Start()
    {
        theBall = GameObject.FindGameObjectWithTag("Ball");
    }

    //when the ball hits the side walls.
    public static void Score(string wallID)
    {
        Debug.Log("I am here");
        if (wallID == "rightWall")
        {
            PlayerScore1++;
        }
        else
        {
            PlayerScore2++;
        }
    }

    public void resetScore() {
        PlayerScore1 = 0;
        PlayerScore2 = 0;
        theBall.SendMessage("RestartGame", 0.5f, SendMessageOptions.RequireReceiver);
    }


    // Update is called once per frame
    void Update()
    {
        pscore1.text = PlayerScore1.ToString();
        pscore2.text = PlayerScore2.ToString();

        if (PlayerScore1 == 10)
        {
            gameMessage.text = "PLAYER ONE WINS";
            theBall.SendMessage("ResetBall", null, SendMessageOptions.RequireReceiver);
        }
        else if (PlayerScore2 == 10)
        {
            gameMessage.text = "PLAYER TWO WINS";
            theBall.SendMessage("ResetBall", null, SendMessageOptions.RequireReceiver);
        }
    }
}
