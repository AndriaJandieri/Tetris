using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreEntryScript : MonoBehaviour
{
    public TMPro.TextMeshProUGUI playerName;
    public TMPro.TextMeshProUGUI score;

    private void Awake()
    {
        playerName = transform.GetChild(1).gameObject.GetComponent<TMPro.TextMeshProUGUI>();
        score = transform.GetChild(2).gameObject.GetComponent<TMPro.TextMeshProUGUI>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
