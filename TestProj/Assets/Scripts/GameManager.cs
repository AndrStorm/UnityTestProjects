using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public PlayerMovement movement;
    private float restartDelay = 2.5f;
    private float defTimeScale = 2f;
    private float deathForce=900f;
    private float curTimeScale = 2f;

    public LevelCounter levelCounter;
    public UILevelCounter UIlevelCounter;

    public CameraMovement gameCam;

    private void Update()
    {
        
    }
    public void EndGame()
    {
        Time.timeScale = defTimeScale;
        movement.enabled = false;
        Invoke("RestartLevel", restartDelay);
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void CrashRestart()
    {
        //взрыв

        Debug.Log("You expode");
        EndGame();
    }

    public void FallRestart(Rigidbody rb)
    {
        Debug.Log("You fall");
        //*игрок зависает в воздухе(подпрыгивает)
        //Physics.gravity = new Vector3(0,0,0);
        //rb.isKinematic = false;
        //rb.useGravity = false;
        //gameCam.enabled = false;
        rb.AddForce(new Vector3(0, deathForce, 0));
        



        EndGame();
        Physics.gravity = new Vector3(0, -9.8f, 0);


    }

    public void NextLoop(ref int runSpeed)
    {
        movement.transform.position = new Vector3(0f, 0.5f, 0f);


        //*возрастает скорость игры, вместо скорости игрока
        //runSpeed += 2;
        curTimeScale += 0.4f;
        Time.timeScale = curTimeScale;
        


        //*счетчик уровня в текст боксе
        //levelCounter.CountLevel((runSpeed-4)/2);
        levelCounter.CountLevel((int)((curTimeScale - 1.6f)/0.4f));
        UIlevelCounter.CountLevel((int)((curTimeScale - 1.6f) / 0.4f));



    }
}
