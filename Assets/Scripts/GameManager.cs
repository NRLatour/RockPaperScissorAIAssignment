using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton 
    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }
    //Game states (see below)
    public GameState State;
    // This is to create events
    public static event Action<GameState> OnGameStateChanged;

    public bool HighestFuzzy_NotBlending = true;

    public BoundaryStyles Boundary = BoundaryStyles.WrapAround;
    public UnitFormations UnitFormation = UnitFormations.Triangle;

    [SerializeField]
    ArenaController arenaController;
    [SerializeField]
    public UnitSpawnPlacement unitSpawnPlacement;
    [SerializeField] GameObject[] Tombs;
    [SerializeField] GameObject[] Explosions;

    //number of units
    [SerializeField] private int rockUnits;
    [SerializeField] private int paperUnits;
    [SerializeField] private int scissorsUnits;

    public float RockAggressive = 0.0f;
    public float PaperAggressive = 0.0f;
    public float ScissorsAggressive = 0.0f;

    public int[] units = new int[3];
    GroupAI[] Groups = new GroupAI[3];
    int groupsIndex = 0;


    private void Start()
    {
        //get all the number of units
        updateUnitNumbers();

        GetGroups();
        //set the game state
        UpdateGameState(GameState.FuzzyLogic);
    }

    private void GetGroups()
    {
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("GroupAI"))
        {
            Groups[groupsIndex++] = g.transform.GetComponent<GroupAI>();
        }
    }

    public void UpdateGameState(GameState newState)
    {
        //we have 3 states in the game,
        State = newState;

        switch (newState)
        {
            //The game play
            case GameState.FuzzyLogic:
                HandleFuzzyLogic();
                break;
            //Decide the results and see if there is a winner. In this state, gameplay still goes on. 
            case GameState.Decide:
                HandleDecide();
                break;
            //The game has finished
            case GameState.Victory:
                HandleVictory();
                break;
            default:
                break;
        }

        OnGameStateChanged?.Invoke(newState);
    }


    private void HandleFuzzyLogic()
    {
        foreach (GroupAI g in Groups)
        {
            g.UpdateFuzzyLogic();
        }
    }




    private void HandleDecide()
    {
        updateUnitNumbers();
        //if one of the units reached to 0, then the game ends.
        if (units.Min() == 0)
        {
            UpdateGameState(GameState.Victory);
        }
        else
        {
            UpdateGameState(GameState.FuzzyLogic);
        }
        
    }

    private void updateUnitNumbers()
    {
               
        // Get the number of each unit in the game
        rockUnits = GameObject.FindGameObjectsWithTag("Rock").Length;
        paperUnits = GameObject.FindGameObjectsWithTag("Paper").Length;
        scissorsUnits = GameObject.FindGameObjectsWithTag("Scissors").Length;

        //And set them as variables
        units[0] = rockUnits;
        units[1] = paperUnits;
        units[2] = scissorsUnits;
       
    }

    private void HandleVictory()
    {
        //When the game ends, all the objects freeze. 
        Time.timeScale = 0;
        return;
    }

    public void NewFormation(UnitFormations newformation)
    {
        UnitFormation = newformation;
        unitSpawnPlacement.ChangeFormation(newformation);
        ResetGame();
    }

    public void ResetGame()
    {
        arenaController.ResetArena();
        HandleDecide();
        EmptyGraveyard();
    }

    private void EmptyGraveyard()
    {
        GameObject[] graves = GameObject.FindGameObjectsWithTag("Tombstone");
        foreach (GameObject tombstone in graves)
        {
            Destroy(tombstone);
        }
    }

    public void NewTombstone(int dead, Vector3 pos)
    {
        Instantiate(Explosions[dead], pos, Quaternion.identity);
        if (dead < Tombs.Length)
        {
            Instantiate(Tombs[dead], pos + 2 * Vector3.down, Quaternion.identity);
        }
    }
}


// These are Game States. You can change them as you see appropriate. 

public enum GameState
{
    FuzzyLogic,
    Decide,
    Victory
}
public enum BoundaryStyles
{
    WrapAround,
    Destruction
}


public enum UnitFormations 
{ 
    Triangle, 
    Circular, 
    Scattered, 
    Unbalanced, 
    Desperate 
}