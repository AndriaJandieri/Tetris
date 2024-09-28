using UnityEngine;
using TMPro;

public class NameInputController : MonoBehaviour
{
    public TMP_InputField inputField;
    public int minNameLength = 2;
    public GameObject txtRule1;
    public GameObject txtRule2;

    public Animator animator1;
    public Animator animator2;

    private void Start()
    {
        animator1 = txtRule1.GetComponent<Animator>();
        animator2 = txtRule2.GetComponent<Animator>();
        HideTextRules();

        inputField.onValueChanged.AddListener(OnInputValueChanged);
        inputField.onValueChanged.AddListener(OnNonEnglishCharactersType);
    }

    private void Update()
    {
    }

    private void OnInputValueChanged(string input)
    {
        string processedInput = input.Replace(" ", "");

        inputField.text = processedInput;
    }

    private void OnNonEnglishCharactersType(string input)
    {
        string filteredInput = System.Text.RegularExpressions.Regex.Replace(input, "[^a-zA-Z]", "");
        inputField.text = filteredInput;
    }

    public void ShowTextRules()
    {
        Time.timeScale = 1;

        animator1.SetTrigger("PlayTextAnim");
        animator2.SetTrigger("PlayTextAnim");
    }

    public void HideTextRules()
    {
        Time.timeScale = 1;
        animator1.SetTrigger("HideText");
        animator2.SetTrigger("HideText");

        Debug.Log("HideTextRules Called");
    }

    public bool HasValidInput()
    {
        // Check if the field is empty or has less than minNameLength characters
        if (inputField != null && !string.IsNullOrEmpty(inputField.text) && inputField.text.Length >= minNameLength)
        {
            HideTextRules();
            Debug.Log("Input is Valid");
            //field is not empty and has at least minNameLength characters
            return true;
        }
        else
        {
            ShowTextRules();
            Debug.Log("Input is NOT Valid");
            ////field is empty or has less than minNameLength characters
            return false;
        }
    }
}
