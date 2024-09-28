using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    private Vector2 touchStartPos;
    private Vector2 touchEndPos;
    private bool isSwiping = false;

    public Board boardScript;


    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }
    public int rotationIndex { get; private set; }

    public float stepDelay = 1f;
    public float lockDelay = .5f;

    private float stepTime;
    private float lockTime;


    private void Awake()
    {
        boardScript = GameObject.Find("Board").GetComponent<Board>();
    }
    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.board = board;
        this.position = position;
        this.data = data;
        this.rotationIndex = 0;
        this.stepTime = Time.time + this.stepDelay;
        this.lockTime = 0f;

        if (this.cells == null)
        {
            this.cells = new Vector3Int[data.cells.Length];
        }
        for (int i = 0; i < data.cells.Length; i++)
        {
            this.cells[i] = (Vector3Int)data.cells[i];
        }
    }

    public bool Move(Vector2Int movement)
    {
        Vector3Int newPosition = this.position;
        newPosition.x += movement.x;
        newPosition.y += movement.y;

        bool valid = this.board.isValidPosition(this, newPosition);

        if (valid)
        {
            this.position = newPosition;
            this.lockTime = 0f;
        }

        return valid;

    }
    public void HardDrop()
    {
        while (Move(Vector2Int.down))
        {
            continue;
        }
        Lock();
        AudioManager.Instance.PlaySFX("Drop");
    }

    public void Rotate(int direction)
    {
        int defaultRotation = this.rotationIndex;
        this.rotationIndex = Wrap(this.rotationIndex + direction, 0, 4);

        ApplyRotationMatrix(direction);

        if (!TestWallKicks(this.rotationIndex, direction))
        {
            this.rotationIndex = defaultRotation;
            ApplyRotationMatrix(-direction);
        }
    }
    private void ApplyRotationMatrix(int direction)
    {
        for (int i = 0; i < this.data.cells.Length; i++)
        {
            Vector3 cell = this.cells[i];

            int x, y;

            switch (this.data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    cell.x -= .5f;
                    cell.y -= .5f;
                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;

                default:
                    x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
            }
            this.cells[i] = new Vector3Int(x, y, 0);

        }
    }

    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int wallKIckIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < this.data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = this.data.wallKicks[wallKIckIndex, i];
            if (Move(translation))
            {
                return true;
            }
        }

        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = rotationIndex * 2;

        if (rotationDirection < 0)
        {
            wallKickIndex--;
        }
        return Wrap(wallKickIndex, 0, this.data.wallKicks.GetLength(0));
    }

    private int Wrap(int input, int min, int max)
    {
        if (input < min)
        {
            return max - (min - input) % (max - min);
        }
        else
        {
            return min + (input - min) % (max - min);
        }
    }

    private void Lock()
    {
        this.board.Set(this);

        this.board.ClearLines(); // Clearing lines
        this.board.SpawnPiece();
    }
    private void Step()
    {
        this.stepTime = Time.time + this.stepDelay;
        Move(Vector2Int.down);

        if (this.lockTime >= this.lockDelay)
        {
            Lock();
        }
    }

    private void HandleTap()
    {
        Debug.Log("Tap detected!");
        // Implement your tap action here
        Rotate(-1);
    }

    private void HandleSwipe(Vector2 direction)
    {
        direction.Normalize();

        // Check for left or right swipe
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x > 0)
            {
                Debug.Log("Right swipe detected!");
                // Implement your right swipe action here

                Move(Vector2Int.right);
            }
            else
            {
                Debug.Log("Left swipe detected!");
                // Implement your left swipe action here

                Move(Vector2Int.left);
            }
        }
        // Check for up or down swipe
        else
        {
            if (direction.y > 0)
            {
                Debug.Log("Up swipe detected!");
                // Implement your up swipe action here
                Rotate(1);
            }
            else
            {
                Debug.Log("Down swipe detected!");
                // Implement your down swipe action here
                HardDrop();
            }
        }
    }
    private void Update()
    {
        if (board)
        {
            this.board.Clear(this);
        }

        this.lockTime += Time.deltaTime;
        #region KEYBOARD Controls


        if (boardScript.isGaming)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Rotate(-1);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                Rotate(1);
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                Move(Vector2Int.left);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                Move(Vector2Int.right);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                Move(Vector2Int.down);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                HardDrop();
            }
        }
        
        #endregion
        #region TOUCH Controls
        if (boardScript.isGaming)
        {
            // Check for touch input on both mobile and PC
            if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            {
                // Store the starting position of the touch
                touchStartPos = Input.GetMouseButtonDown(0) ? (Vector2)Input.mousePosition : Input.GetTouch(0).position;
                isSwiping = true;
            }
            else if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
            {
                // Store the ending position of the touch
                touchEndPos = Input.GetMouseButtonUp(0) ? (Vector2)Input.mousePosition : Input.GetTouch(0).position;

                // Calculate the swipe direction and distance
                Vector2 swipeDirection = touchEndPos - touchStartPos;
                float swipeMagnitude = swipeDirection.magnitude;

                // Check for a tap (if the swipe distance is small)
                if (isSwiping && swipeMagnitude < 10f)
                {
                    // Handle the tap
                    HandleTap();
                }
                // Check for a swipe
                else if (isSwiping && swipeMagnitude >= 50f)
                {
                    // Determine the swipe direction and handle it
                    HandleSwipe(swipeDirection);
                }

                isSwiping = false;
            }
        }
        #endregion

        if (board)
        {
            if (board.isGaming)
            {
                if (Time.time >= this.stepTime)
                {
                    Step();
                }
            }
        }
        if (board)
        {

            this.board.Set(this);
        }

    }
}
