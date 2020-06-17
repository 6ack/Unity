using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour {

    [Header("Pool objects")]
    public PoolObject Ammo;
    public PoolObject BrickShild;

    [Header("Objects")]
    public Transform Arrow;
    public Transform TouchLine;
    public Transform Ball;
    public Transform BrickKill;

    [Header("With/Hight")]
    public Vector2 Grid;
    public Vector2 SizeBrick { get; set; }
    public float TopYPositionBrickBox { get; set; }
    public float HightBrickBox { get; set; }
    public int Score { get; set; }

    public static GameManager Singlton { get; private set; }


    // ---- PRIVATE

    private int countBricKill;

    //begin terget position
    private Vector3 beginTargetPos;

    private Camera mainCamera;
    private GameManager.State state;
    private Vector3 directionTarget = Vector3.zero;
    private Vector3 positionStartBrick; // start position brick Kill
   

    private List<Brick>[] LinesGame;

    // activ number line
    private int numberLine = 0;

    // count ammo
    [SerializeField]
    private int countAmmo;

    private bool killBrick;

    private int сountBоll;

    private bool continueGame;

    private enum State
    {
        Idle,
        Targeting,
        Shooting,
        GameOver
    }


    

    private void Awake()
    {
        Singlton = this;
        Physics2D.IgnoreLayerCollision(8,8, true);
    }

    void Start ()
    {
        continueGame = true;

        this.state = GameManager.State.Idle;
        this.mainCamera = Camera.main;
        
        сountBоll = countAmmo;
        UpdateCountBall();

        // ------ diseble vizible TouchLine/arrow

        this.TouchLine.gameObject.SetActive(false);
        this.Arrow.gameObject.SetActive(false);

        //------------------------------------

        // brickKill size
        float xy = (mainCamera.orthographicSize * 2.0f * Screen.width / Screen.height) / Grid.x;
        SizeBrick = new Vector2(xy, xy);



        positionStartBrick = ScenManager.Singlton.CalcPosition(new Vector2(Screen.width / 2f, Screen.height - 125f * (Screen.width / 800f)));
        positionStartBrick.y -= SizeBrick.x / 2;

        // Position and scale brick kill
        SetPositionBrickKill(new Vector3(GetRendomPositionXBrick(), positionStartBrick.y, positionStartBrick.z));
        BrickKill.localScale = SizeBrick;

        // Grid hight;
        HightBrickBox = SizeBrick.x * Grid.y * -1;

        // Top position grid
        TopYPositionBrickBox = positionStartBrick.y - (SizeBrick.x / 2f + 0.16f);

        //Bottom/Top bounder position
        ScenManager.Singlton.BottomBounder.position = new Vector2(0, HightBrickBox + TopYPositionBrickBox);
        ScenManager.Singlton.TopBounder.position = ScenManager.Singlton.CalcPosition(new Vector2(Screen.width / 2f, Screen.height - 125f * (Screen.width / 800f)));

        //Ball position
        Ball.position = new Vector2(0, HightBrickBox + TopYPositionBrickBox + Ball.localScale.y/2f );

        //initialization line bricks 
        LinesGame = new List<Brick>[(int)Grid.y];
    }

    void Update ()
    {
        if (state == GameManager.State.GameOver)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0) && !IsPointerOverUIObject())
        {
            beginTargetPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -this.mainCamera.transform.position.z));

            if (state == GameManager.State.Idle)
            {
                StartTarget();
            }

        }
        if (state == GameManager.State.Targeting)
        {
            UpdateTargeting();
        }
    }

    private void GameOver()
    {
        state = GameManager.State.GameOver;
        SoundMenager.Singlton.PlaySound(SoundGame.Gameover);

        SaveDataGame();

        if (continueGame)
        {
            ScenManager.Singlton.GameOverPause();
            continueGame = false;
        }
        else
            ScenManager.Singlton.GameOver();
    }


    public void СontinueGame()
    {
        state = GameManager.State.Idle;
        ScenManager.Singlton.Window.gameObject.SetActive(false);
        RemoveLines();
    }

    public void SaveDataGame()
    {
        SaveData.UpdateBestScore(Score);
    }

#region TARGET

    
    private void StartTarget()
    {
        state = GameManager.State.Targeting;
        Arrow.transform.position = Ball.position;

      //  TouchDot.transform.localPosition = beginTargetPos;
        TouchLine.gameObject.SetActive(true);
        TouchLine.transform.localScale = Vector3.zero;
    }

    private void UpdateTargeting()
    {
        Vector3 a = beginTargetPos;
        Vector3 vector = this.mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -this.mainCamera.transform.position.z));
        Vector3 vector2 = vector - this.beginTargetPos;

        float num = Vector3.Distance(a, vector);

        if (Input.GetMouseButton(0))
        {
            if (num > 1f && a.y > vector.y)
            {
                float num2 = Mathf.Atan2(a.y - vector.y, a.x - vector.x) * 57.29578f;

                num2 *= -1;
                num2 += 90;

                if (num2 < -80f)
                {
                    num2 = -80f;
                }
                else if(num2 > 80f) {
                    num2 = 80f;
                }

                Vector3 normal = Ball.position;

                directionTarget.y = normal.y + Mathf.Cos(num2/ 57.29578f) ;
                directionTarget.x = normal.x + Mathf.Sin(num2 / 57.29578f) ;

                Arrow.localRotation = Quaternion.Euler(0f, 0f, -num2);

                if (Arrow.gameObject.activeSelf == false)
                    Arrow.gameObject.SetActive(true);

            }

            TouchLine.localScale = new Vector3(vector2.magnitude, 0.04f, 1f);
            TouchLine.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(vector2.y, vector2.x) * 57.29578f);
            TouchLine.position = a + vector2.normalized * vector2.magnitude / 2f;

        }


        if (Input.GetMouseButtonUp(0) && num > 1f)
        {
            this.state = GameManager.State.Shooting;
            TouchLine.gameObject.SetActive(false);
            Arrow.gameObject.SetActive(false);

            this.state = GameManager.State.Shooting;
            shootingCountAmmo = 0;
            StartCoroutine(Shooting((directionTarget - Arrow.position).normalized));
        }
        else if(Input.GetMouseButtonUp(0))
        {
            state = GameManager.State.Idle;
            TouchLine.gameObject.SetActive(false);
            Arrow.gameObject.SetActive(false);
        }

    
    }

    #endregion

#region SHOOTING

    private int shootingCountAmmo;

    // Shooting on ammo
    private IEnumerator Shooting(Vector3 direction)
    {
          for (int i = 0; i < countAmmo; i++)
        {
            Ammo ammoObject = PoolManager.Singleton.GetObject(Ammo).GetComponent<Ammo>();

            ammoObject.MyTransform.position = Ball.position + direction * 0.4f;
            float d = Mathf.Lerp(14f, 16.5f, сountBоll / 100f);
            ammoObject.Push(direction * d);
            shootingCountAmmo++;
            сountBоll--;
            UpdateCountBall();
            yield return null;
        }
    }

    // Shooting off ammo
    public void EndShooting()
    {
         shootingCountAmmo--;

      //  Debug.Log(shootingCountAmmo);
        if (shootingCountAmmo == 0)
        {
            this.state = GameManager.State.Idle;
            Addline();

            SetPositionBrickKill(new Vector3(GetRendomPositionXBrick(), positionStartBrick.y, 0));
            SetPositionBall(new Vector3(GetRendomPositionXBrick(false), Ball.position.y, 0));

            if (killBrick)
            {
                BrickKill.gameObject.SetActive(true);
                countAmmo += 1;
                
                killBrick = false;
            }
            сountBоll = countAmmo;
            UpdateCountBall();
        }
    }

    #endregion

#region LINES

    private int endLine = 0;

    // Create new Line brick
    private void Addline()
    {
        DownLines();

        // namber line max Grid.y
        if (numberLine == Grid.y)
            numberLine = 0;


        if (numberLine == Grid.y - 1)
            endLine = 0;

        // is game over
        if (LinesGame[endLine] != null && LinesGame[endLine].Count != 0)
        {
            GameOver();
        }

        List<Brick> LineBrick = new List<Brick>();

       // Brick BrickKill_Brick = BrickKill.GetComponent<Brick>();

        // rendom count
        int count = (int)Random.Range(2, (int)Random.Range(2, Grid.x-1));

        // get pool brick
        for (int i = 0; i < count; i++)
        {
            Brick brick = PoolManager.Singleton.GetObject(BrickShild).GetComponent<Brick>();

            // number and line brick
            brick.line = numberLine;
            brick.NumberBick = i;

            // Random Count 
            int countB = (int)Random.Range(countAmmo, countAmmo * 2.2f);
            brick.brickMaxCount = countB;
            brick.brickCount = countB;

            if(countB > 4)
            brick.RendererS.color = ColorBrickManager.Singlton.GetColorBrick(countB);

            brick.UpdateCount();

            // scale brick
            brick.MyTransform.localScale = SizeBrick*0.97f;

            LineBrick.Add(brick);
            brick.ActivateOject();
        }


        // -----  random position block
        List<Vector2> PositonsBricks = new List<Vector2>();

        foreach (Brick brick in LineBrick)
        {
            bool rendPos = false;
            Vector3 pos  = Vector3.zero;

            while (rendPos == false)
            {
                pos = GetRendomPositionLineBrick();
                var checPos = PositonsBricks.Find((posBrick) => posBrick.x == pos.x);

                if (checPos.x == 0 || checPos.y == 0)
                    rendPos = true;
            }

            PositonsBricks.Add(pos);
            brick.MyTransform.position = pos;
        }
        // --------

        LinesGame[numberLine] = LineBrick;
       
        numberLine++;
        endLine++;
    }


    // Remove Line brick
    private void RemoveLines()
    {
        Debug.Log(endLine);

        List<Brick> TempLine = new List<Brick>();
        List<Brick> Line;
        for (int i = 0; i < 2; i++)
        {
            if (endLine == Grid.y)
            {
                Line = LinesGame[endLine-1];
                endLine = 1;
            }
            else
                Line = LinesGame[endLine - i];

            foreach (Brick brick in Line)
               TempLine.Add(brick);
 
            foreach(Brick brick in TempLine)
                DellBrickLine(brick);

            TempLine.Clear();
        }
    }
    #endregion

#region CONTACTS


    public void OnContactBrickKill(GameObject brickKill)
    {
        BrickKill brickObj = brickKill.GetComponent<BrickKill>();

        SoundMenager.Singlton.PlaySound(SoundGame.Score);

        killBrick = true;
        brickObj.ActiveEffect();
        brickObj.gameObject.SetActive(false);

        UpdateScore(1);
    }

    public bool OnContactBrick(GameObject brick)
    {
        Brick brickObj = brick.GetComponent<Brick>();
        brickObj.brickCount -= 1;

        // kill up, Update levl, score, position
        if (brickObj.brickCount <= 0)
        {
            SoundMenager.Singlton.PlaySound(SoundGame.BrickDie);
            DellBrickLine(brickObj);
            return true;
        }
        else
        {
            SoundMenager.Singlton.PlaySound(SoundGame.ContactBrick);

            if (brickObj.brickCount > 4)
                brickObj.RendererS.color = ColorBrickManager.Singlton.GetColorBrick(brickObj.brickCount);

            brickObj.UpdateCount();
        }
        
        return false;
    }

    #endregion

#region HELPERS

    private TextMesh ballTextMesh;


    private void UpdateCountBall()
    {
        if (ballTextMesh == null)
            ballTextMesh = Ball.GetComponentInChildren<TextMesh>();

        ballTextMesh.text = "x" + сountBоll;
    }

    private void DellBrickLine(Brick brick)
    {
        brick.DisabledObject();
        List<Brick> ListLine = LinesGame[brick.line];
        ListLine.Remove(brick);
    }

    private void UpdateScore(int score)
    {
        Score += score;
        ScenManager.Singlton.SetScore(Score);
    }

    private void SetPositionBrickKill(Vector3 position)
    {
        BrickKill.position = position;
    }

    private void SetPositionBall(Vector3 position)
    {
        Ball.position = position;
    }

    private Vector2 GetRendomPositionLineBrick(bool edges = true)
    {
        float Y = TopYPositionBrickBox - (SizeBrick.x / 2f) ;
        return new Vector2(GetRendomPositionXBrick(edges), Y);
    }

    private void DownLines()
    {
        if (LinesGame.Length == 0)
            return;

        foreach (List<Brick> lineBrick in LinesGame)
        {
            if(lineBrick != null)

            foreach (Brick brick in lineBrick)
                brick.MoveToY(brick.MyTransform.position.y - SizeBrick.y);

        }
    }

    private float GetRendomPositionXBrick(bool edges = true)
    {
        float posRendomX;

        if (edges)
            posRendomX = SizeBrick.x * (int)Random.Range(1, Grid.x);
        else
            posRendomX = SizeBrick.x * (int)Random.Range(2, Grid.x - 1);

        float xy = mainCamera.orthographicSize * 2.0f * Screen.width / Screen.height;

        return (xy / 2 * -1) + posRendomX - (SizeBrick.x / 2);
    }

    public static bool IsPointerOverUIObject()
    {
        bool result = false;
        if (Input.touchCount > 0)
        {
            result = EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        }
        return result;
    }
    #endregion

}
