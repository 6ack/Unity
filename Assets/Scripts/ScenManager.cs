using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScenManager : MonoBehaviour {

    [Header("BOUNDER")]
    public Transform RightBounder;
    public Transform LeftBounder;
    public Transform BottomBounder;
    public Transform TopBounder;


    [Header("SCORE")]
    public Text Score;
    public Text BestScore;


    [Header("UI GAMEOVER")]
    public GameObject Window;
    public Text ScoreEnd;
    public Text BestScoreEnd;

    Plane plane;

    public static ScenManager Singlton { get; private set; }

    private void Awake()
    {
        if (Singlton == null)
            Singlton = this;

        plane = new Plane(transform.forward, transform.position);
    }

    void Start()
    {

        if (Window != null)
            Window.gameObject.SetActive(false);


        //-------- bounders positions 

        if (LeftBounder != null)
        {
            LeftBounder.position = CalcPosition(new Vector2(0f, Screen.height / 2f));
            LeftBounder.localScale = new Vector3(0.1f, Camera.main.orthographicSize * 2.0f, 1f);
        }

        if (RightBounder != null)
        {
            RightBounder.position = CalcPosition(new Vector2(Screen.width, Screen.height / 2f));
            RightBounder.localScale = new Vector3(0.1f, Camera.main.orthographicSize * 2.0f, 1f);
        }

        float widthScale = Camera.main.orthographicSize * 2.0f * Screen.width / Screen.height;

        if (BottomBounder != null)
        {
            BottomBounder.localScale = new Vector3(widthScale, 0.02f, 1f);
            TopBounder.localScale = new Vector3(widthScale, 0.02f, 1f);
        }

        if (TopBounder != null)
        {
            TopBounder.localScale = new Vector3(widthScale, 0.02f, 1f);
        }

        //--------

        BestScore.text = SaveData.GetBestScore().ToString();

    }

    public void SetScore(int score)
    {
        Score.text = score.ToString();
    }

    // calculation position in Screen
    public Vector3 CalcPosition(Vector2 screenPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        float dist = 0f;
        Vector3 pos = Vector3.zero;

        if (plane.Raycast(ray, out dist))
            pos = ray.GetPoint(dist);

        return pos;
    }

    public void GameOverPause()
    {
        Window.gameObject.SetActive(true);
        ScoreEnd.text = GameManager.Singlton.Score.ToString();
        BestScoreEnd.text = SaveData.GetBestScore().ToString();
    }

    public void GameOver()
    {
        Window.gameObject.SetActive(true);
        ScoreEnd.text = GameManager.Singlton.Score.ToString();
        BestScoreEnd.text = SaveData.GetBestScore().ToString();
    }

#region BUTTONS

    public void Button_СontinueGame()
    {
        GameManager.Singlton.СontinueGame();
        SoundMenager.Singlton.PlaySound(SoundGame.Buttom);
    }

    public void Button_StartGame()
    {
        SoundMenager.Singlton.PlaySound(SoundGame.Buttom);
        SceneManager.LoadScene("Game");
    }

#endregion
}
