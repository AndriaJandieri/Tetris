using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyModes : MonoBehaviour
{
    public Button normalModeBtn;
    public Button hardModeBtn;

    public bool isHardMode = false;
    // Start is called before the first frame update
    void Start()
    {
        NormalMode();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void NormalMode()
    {
        normalModeBtn.GetComponent<Image>().color = new Color(255, 255, 255, .15f);
        hardModeBtn.GetComponent<Image>().color = new Color(255, 255, 255, 0);
        isHardMode = false;
    }
    public void HardMode()
    {
        normalModeBtn.GetComponent<Image>().color = new Color(255, 255, 255, 0);
        hardModeBtn.GetComponent<Image>().color = new Color(255, 255, 255, .15f);
        isHardMode = true;
    }
}
