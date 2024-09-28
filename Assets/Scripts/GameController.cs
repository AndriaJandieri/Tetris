using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    public LeaderBoardSystem leaderBoardSystem;
    public Board boardScript;
    public Piece pieceScript;
    public Button pauseButton;

    public Sprite musicPlayBtn;
    public Sprite musicMuteBtn;
    public Button muteButton;
    public GameObject scoreHolder;

    public GameObject pauseMenu;
    public GameObject mainMenu;
    public GameObject scoreMenu;
    public GameObject soundMenu;
    public GameObject gameOverMenu;
    public GameObject nameInputMenu;
    public GameObject exitQuestion;

    public GameObject ghostBlock;

    public NameInputController nameInputController;
    private bool isPaused = false;

    void Start()
    {
        pauseButton.interactable = false;
        scoreHolder.SetActive(false);
        DisableAllMenu();
        mainMenu.SetActive(true);
        leaderBoardSystem.ResetHighScoresTextStyle();
    }
    void Update()
    {
        //if (soundMenu.activeSelf)
        //{
        //    AudioManager.Instance.SetMusicVolume();
        //    AudioManager.Instance.SetSFXVolume();
        //    Debug.Log("IM LOOPING HERE");
        //}
    }
    public void OnPauseButtonClick()
    {
        isPaused = !isPaused;

        if (!isPaused)
        {

            DisableAllMenu();
            pauseButton.interactable = true;
            Debug.Log("PLAYING");
            Time.timeScale = 1f;
            StartCoroutine(StartDelay(.1f));  //HERE IS DELAY TO AVOID PIECE INSTAN DROP

        }
        else
        {

            DisableAllMenu();
            pauseMenu.SetActive(true);
            pauseButton.interactable = false;
            Debug.Log("PAUSED");
            boardScript.isGaming = false;
            Time.timeScale = 0f;
        }
    }
    public void OnMuteButtonClick()
    {

        AudioManager.Instance.ToggleMusic();
        AudioManager.Instance.ToggleSFX();

        if (muteButton.image.sprite == musicPlayBtn)
        {
            muteButton.image.sprite = musicMuteBtn;
        }
        else
        {
            muteButton.image.sprite = musicPlayBtn;
        }
    }

    public void OnSoundButtonClick()
    {
        DisableAllMenu();
        soundMenu.SetActive(true);
        Debug.Log("Sound Button Clicked");
    }

    public void OnOkClick()
    {
        DisableAllMenu();
        if (isPaused)
        {
            pauseMenu.SetActive(true);
            //AudioManager.Instance.SetMusicVolume();
            //AudioManager.Instance.SetSFXVolume();
        }
        else
        {
            mainMenu.SetActive(true);
        }
    }

    public void OnBackClick()
    {
        DisableAllMenu();
        scoreHolder.SetActive(false);
        mainMenu.SetActive(true);

        leaderBoardSystem.ResetHighScoresTextStyle();

        AudioManager.Instance.PlayScoreMusicIfNotPlaying();
    }

    public void OnScoreButtonClick()
    {
        DisableAllMenu();
        scoreMenu.SetActive(true);
        //AudioManager.Instance.PlayMusic("MusicHighScores");
        Debug.Log("Scores Button Clicked");
    }

    public void GoToMainMenu()
    {

        DisableAllMenu();
        scoreHolder.SetActive(false);
        mainMenu.SetActive(true);

        pauseButton.interactable = false;

        AudioManager.Instance.PlayMusic("Music01");
    }

    public void GoToGameOverMenu()
    {
        DisableAllMenu();
        pauseButton.interactable = false;


        if (boardScript.score >= leaderBoardSystem.localDataEntries[9].score)
        {
            nameInputMenu.SetActive(true);
        }
        else
        {
            gameOverMenu.SetActive(true);
            boardScript.gameOverScoreText.text = boardScript.score.ToString();
            scoreHolder.SetActive(false);
        }
    }

    public void OnInputNameMenuOKAYClick()
    {
        if (nameInputController.HasValidInput())
        {

            //This Scenario is adding some functionality directly in nameInputController.HasValidInput()'s TRUE condition.

            DisableAllMenu();
            leaderBoardSystem.AddEntry(nameInputController.inputField.text, boardScript.score); //ADDING NEW HIGH SCORE
            leaderBoardSystem.DisplayLocalScores();
            leaderBoardSystem.SaveLocalDataToJSON();
            //leaderBoardSystem.ResetTextColors();
            scoreMenu.SetActive(true);
        }
        else
        {
            //nameInputController.PlayTextRules();
            nameInputController.ShowTextRules();
            //This Scenario is defined directly in nameInputController.HasValidInput()'s FALSE condition.
        }
    }

    public void OnStartButtonClick()
    {
        if (GetComponent<DifficultyModes>().isHardMode)
        {
            ghostBlock.SetActive(false);
        }
        else
        {
            ghostBlock.SetActive(true);
        }
        scoreHolder.SetActive(true);
        boardScript.GameOver();
        StartCoroutine(StartDelay(.1f));   //HERE IS DELAY TO AVOID PIECE INSTAN DROP
        boardScript.score = 0;
        boardScript.ScoreUpdate();
        boardScript.SpawnPiece();
        Time.timeScale = 1f;
        pauseButton.interactable = true;
        ResetPoints();
        DisableAllMenu();
        AudioManager.Instance.PlayMusic("Music02");
        Debug.Log("Start Game Clicked");
    }

    public void DisableAllMenu()
    {
        pauseMenu.SetActive(false);
        mainMenu.SetActive(false);
        scoreMenu.SetActive(false);
        soundMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        exitQuestion.SetActive(false);
        nameInputMenu.SetActive(false);
    }

    public void ResetPoints()
    {
        boardScript.linesClearedCounter = 0;
        boardScript.score = 0;
        isPaused = false;
        pieceScript.stepDelay = 1f;
    }

    public void AnswerOnExitQuestion(bool answer)
    {
        if (answer)
        {
            GoToMainMenu();
        }
        else
        {
            DisableAllMenu();
            pauseMenu.SetActive(true);

        }
    }

    public void ExitQuestionPop()
    {
        DisableAllMenu();
        exitQuestion.SetActive(true);
    }
    public IEnumerator StartDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        boardScript.isGaming = true;
    }
}
