using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public Camera mainCamera;
    public GameController gameController;
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }
    public TetrominoData[] tetrominos;
    public Vector3Int spawnPosition;
    public Vector2Int boardSize = new Vector2Int(10, 20);

    public TMPro.TextMeshProUGUI scoreText;
    public TMPro.TextMeshProUGUI gameOverScoreText;
    public bool isGaming = false;

    private bool isLevelOne, isLevelTwo, isLevelthree;
    public Color speedColor1, speedColor2, speedColor3, speedColor4;
    public int score, linesClearedCounter, scoreSingleLine, scoreDoubleLine, scoreTripleLine, scoreTetris;

    public int LinesClearedForL2 = 5;
    public float L2Speed = .8f;
    public int LinesClearedForL3 = 10;
    public float L3Speed = .5f;
    public int LinesClearedForL4 = 15;
    public float L4Speed = .3f;

    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-this.boardSize.x / 2, -this.boardSize.y / 2);
            return new RectInt(position, this.boardSize);
        }
    }

    private void Awake()
    {
        this.tilemap = GetComponentInChildren<Tilemap>();
        this.activePiece = GetComponentInChildren<Piece>();
        for (int i = 0; i < this.tetrominos.Length; i++)
        {
            this.tetrominos[i].Initialize();
        }
    }
    void Start()
    {
        ResetGameSpeedAndColor();
    }

    public void SpawnPiece()
    {
        int random = Random.Range(0, this.tetrominos.Length);
        TetrominoData data = this.tetrominos[random];

        this.activePiece.Initialize(this, this.spawnPosition, data);

        if (isValidPosition(this.activePiece, this.spawnPosition))
        {
            Set(this.activePiece);
        }
        else
        {
            GameOver();
            gameController.GoToGameOverMenu();
            AudioManager.Instance.PlaySFX("GameOver");
            isGaming = false;
        }


    }

    public void GameOver()
    {
        this.tilemap.ClearAllTiles();
        isGaming = false;
        ResetGameSpeedAndColor();
        Time.timeScale = 0;
        Debug.Log("GAME OVER");
        AudioManager.Instance.PlayMusic("MusicHighScores");
    }

    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePosition, null);
        }
    }

    public bool isValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = this.Bounds;
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            if (!bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }
            if (this.tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }
        return true;
    }
    public void ClearLines()
    {
        RectInt bounds = this.Bounds;
        int row = bounds.yMin;
        int currentLinesClearedCounter = 0;

        while (row < bounds.yMax)
        {
            if (IsLineFull(row))
            {
                currentLinesClearedCounter++;
                LineClear(row);
            }
            else
            {
                row++;
            }
        }

        switch (currentLinesClearedCounter)
        {
            case 1:
                Debug.Log("Single Line");
                AudioManager.Instance.PlaySFX("SingleLine");
                score += scoreSingleLine;
                linesClearedCounter += 1;
                ScoreUpdate();

                break;
            case 2:
                Debug.Log("Double Line");
                AudioManager.Instance.PlaySFX("DoubleLine");
                score += scoreDoubleLine;
                linesClearedCounter += 2;
                ScoreUpdate();

                break;
            case 3:
                Debug.Log("Triple Line");
                AudioManager.Instance.PlaySFX("TripleLine");
                score += scoreTripleLine;
                linesClearedCounter += 3;
                ScoreUpdate();

                break;
            case 4:
                Debug.Log("Tetris");
                AudioManager.Instance.PlaySFX("Tetris");
                score += scoreTetris;
                linesClearedCounter += 4;
                ScoreUpdate();

                break;
            default:
                break;
        }

    }

    private void LineClear(int row)
    {
        RectInt bounds = this.Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            this.tilemap.SetTile(position, null);
        }

        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = this.tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                this.tilemap.SetTile(position, above);
            }
            row++;
        }

        AudioManager.Instance.PlaySFX("Line");

    }

    public void ScoreUpdate()
    {
        scoreText.text = score.ToString();
        Debug.Log(score);

        if (linesClearedCounter >= LinesClearedForL4 && isLevelthree && isGaming)
        {
            isLevelthree = false;
            Debug.Log("SPEED LEVEL MAX");
            AudioManager.Instance.PlayMusic("Music05");

            mainCamera.backgroundColor = speedColor4;


            GetComponent<Piece>().stepDelay = L4Speed;
        }
        if (linesClearedCounter >= LinesClearedForL3 && isLevelTwo && isGaming)
        {
            isLevelTwo = false;
            isLevelthree = true;
            Debug.Log("SPEED LEVEL 3");
            AudioManager.Instance.PlayMusic("Music04");

            mainCamera.backgroundColor = speedColor3;

            GetComponent<Piece>().stepDelay = L3Speed;

        }
        if (linesClearedCounter >= LinesClearedForL2 && isLevelOne && isGaming)
        {
            isLevelOne = false;
            isLevelTwo = true;
            Debug.Log("SPEED LEVEL 2");
            AudioManager.Instance.PlayMusic("Music03");

            mainCamera.backgroundColor = speedColor2;

            GetComponent<Piece>().stepDelay = L2Speed;
        }
    }

    public void ResetGameSpeedAndColor()
    {
        isLevelOne = true;
        isLevelTwo = false;
        isLevelthree = false;
        mainCamera.backgroundColor = speedColor1;
        GetComponent<Piece>().stepDelay = 1;
    }

    private bool IsLineFull(int row)
    {
        RectInt bounds = this.Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            if (!this.tilemap.HasTile(position))
            {
                return false;
            }
        }
        return true;
    }
}
