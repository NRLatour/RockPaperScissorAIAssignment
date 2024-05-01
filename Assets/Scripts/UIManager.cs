using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    [SerializeField] public TextMeshProUGUI _stateText;
    [SerializeField] public TMP_Dropdown DefuzzificationChoice;
    [SerializeField] public TMP_Dropdown ArenaBoundaryChoice;
    [SerializeField] public TMP_Dropdown FormationChoice;
    [SerializeField] public TMP_Dropdown TimeScaleChoice;
    [SerializeField] public TextMeshProUGUI[] CurrentStats;

    [SerializeField] private int rockUnits;
    [SerializeField] private int paperUnits;
    [SerializeField] private int scissorsUnits;

    public int[] units = new int[3];


    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManagerOnGameStateChanged;

    }

    private void Start()
    {
        DefuzzificationChoice.value = (GameManager.Instance.HighestFuzzy_NotBlending) ? 1:0;
        ArenaBoundaryChoice.value = ((int)GameManager.Instance.Boundary);

    }

    private void GameManagerOnGameStateChanged(GameState state)
    {
        CurrentStats[0].text = (GameManager.Instance.HighestFuzzy_NotBlending) ? "Highest" : "Blending";
        CurrentStats[1].text = (GameManager.Instance.Boundary == BoundaryStyles.Destruction) ? "Destruction" : "Wrap-Around";
        CurrentStats[2].text = GameManager.Instance.RockAggressive.ToString("N2") + ", " + GameManager.Instance.units[0] +" left";
        CurrentStats[3].text = GameManager.Instance.PaperAggressive.ToString("N2") + ", " + GameManager.Instance.units[1] + " left";
        CurrentStats[4].text = GameManager.Instance.ScissorsAggressive.ToString("N2") + ", " + GameManager.Instance.units[2] + " left";

        if (state == GameState.Victory)
        {
            if (GameManager.Instance.units[0] == 0)
            {
                _stateText.text = "ROCK LOST";
            }

            if (GameManager.Instance.units[1] == 0)
            {
                _stateText.text = "PAPER LOST";
            }

            if (GameManager.Instance.units[2] == 0)
            {
                _stateText.text = "SCISSORS LOST";
            }
        }
    }


    public void ArenaBoundaryChanged()
    {
        GameManager.Instance.Boundary = (BoundaryStyles)ArenaBoundaryChoice.value;
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
    }

    public void DefuzzificationMethodChanged()
    {
        GameManager.Instance.HighestFuzzy_NotBlending = (DefuzzificationChoice.value == 1);
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
    }

    public void UnitFormationChanged()
    {
        GameManager.Instance.NewFormation((UnitFormations)FormationChoice.value);
        Time.timeScale = (TimeScaleChoice.value + 1f);
        _stateText.text = "";
    }
    public void GameSpeedChange()
    {
        Time.timeScale = (TimeScaleChoice.value + 1f);
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
    }
}
