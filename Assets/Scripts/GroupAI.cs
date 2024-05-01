using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupAI : MonoBehaviour
{
    public enum AIof
    {
        Papers,
        Scissors,
        Rocks
    };

    public AIof thisAI;

    [SerializeField] private float enemyNumbers;
    [SerializeField] private float targetNumbers;
    [SerializeField] private float friendlyNumbers;

    // Slope of the Membership functions used in Fuzzyification methods
    private const float FUZZYINPUTINCREMENT = 0.2f;

    // Indexes of FuzzyInput array for Friendly, Enemy and Target FuzzyInput values in the Rule Evaluation Method
    private const int Friendly = 0;
    private const int Enemy = 1;
    private const int Target = 2;

    // Rule count in the Rule Evaluation
    private int NumberOfRules = 8;

    private const float MOVECALMLY = 2f;
    private const float AVERAGESPEED = 6f;
    private const float AGGRESSIVE = 10f;

    [SerializeField] private float aggressiveness;
    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManagerOnGameStateChanged;
    }

    public void UpdateFuzzyLogic()
    {
        GameManagerOnGameStateChanged(GameState.FuzzyLogic);
    }

    private void GameManagerOnGameStateChanged(GameState state)
    {

        if (state == GameState.FuzzyLogic)
        {
            if (thisAI == AIof.Scissors)
            {
                //if this code attached to the ScissorsAI, count number of game objects.
                friendlyNumbers = GameObject.FindGameObjectsWithTag("Scissors").Length;
                enemyNumbers = GameObject.FindGameObjectsWithTag("Rock").Length;
                targetNumbers = GameObject.FindGameObjectsWithTag("Paper").Length;

                //calculate aggresiveness
                aggressiveness = RunFuzzyLogic(friendlyNumbers, enemyNumbers, targetNumbers);
                GameManager.Instance.ScissorsAggressive = aggressiveness;

                //Assign aggressiveness to each unit in the fiction. 
                foreach (GameObject units in GameObject.FindGameObjectsWithTag("Scissors"))
                {
                    units.GetComponent<IndividualAI>().Aggresiveness = aggressiveness;
                }
            }
            if (thisAI == AIof.Rocks)
            {
                //if this code attached to the RocksAI, count number of game objects.
                friendlyNumbers = GameObject.FindGameObjectsWithTag("Rock").Length;
                enemyNumbers = GameObject.FindGameObjectsWithTag("Paper").Length;
                targetNumbers = GameObject.FindGameObjectsWithTag("Scissors").Length;

                //calculate aggresiveness
                aggressiveness = RunFuzzyLogic(friendlyNumbers, enemyNumbers, targetNumbers);
                GameManager.Instance.RockAggressive = aggressiveness;

                //Assign aggressiveness to each unit in the fiction. 
                foreach (GameObject units in GameObject.FindGameObjectsWithTag("Rock"))
                {
                    units.GetComponent<IndividualAI>().Aggresiveness = aggressiveness;
                }
            }
            if (thisAI == AIof.Papers)
            {
                //if this code attached to the PaperAI, count number of game objects.
                friendlyNumbers = GameObject.FindGameObjectsWithTag("Paper").Length;
                enemyNumbers = GameObject.FindGameObjectsWithTag("Scissors").Length;
                targetNumbers = GameObject.FindGameObjectsWithTag("Rock").Length;

                //calculate aggresiveness
                aggressiveness = RunFuzzyLogic(friendlyNumbers, enemyNumbers, targetNumbers);
                GameManager.Instance.PaperAggressive = aggressiveness;

                //Assign aggressiveness to each unit in the fiction. 

                foreach (GameObject units in GameObject.FindGameObjectsWithTag("Paper"))
                {
                    units.GetComponent<IndividualAI>().Aggresiveness = aggressiveness;
                }
            }

        }

    }

    // Start is called before the first frame update
    void Start()
    {
        //use if you need
    }

    // Update is called once per frame
    /*void Update()
    {
        //use if you need
    }*/

    private float RunFuzzyLogic(float numberOfFriendly, float numberOfEnemy, float numberOfTarget)
    {
        //First, fuzzyification. Here, we convert unit numbers values between 1 -0 and find membership values
        float[] degreesOfMemberships = Fuzzyification(numberOfFriendly, numberOfEnemy, numberOfTarget);
        //Then, using the degrreso of membership, we evaluate rules. You should get (2x2x2=8 trules) 
        float[] FuzzyOutput= RuleEvaluation(degreesOfMemberships);
        // finally, Defuzzification process, generates one output
        float crispOutput = Defuzzification(FuzzyOutput);

        return crispOutput;
    }


    // fill the codes below
    private float[] Fuzzyification(float numberOfFriendly, float numberOfEnemy, float numberOfTarget)
    {
        float[] membershipFunctionResults = { Fuzzyification_friendly(numberOfFriendly), Fuzzyification_enemy(numberOfEnemy), Fuzzyification_target(numberOfTarget) };
                 
        return  membershipFunctionResults;
    }

    /// <summary>
    /// Returns the y value of the High#FriendlyUnits ranging from [0,1]
    /// To get the Low#FriendlyUnits, the equation is: (High#FriendlyUnits + Low#FriendlyUnits = 1)
    /// </summary>
    /// <param name="theNumberOfUnits">The number of remaining active friendly units</param>
    /// <returns>Fuzzy Input for High#FriendlyUnits</returns>
    private float Fuzzyification_friendly(float theNumberOfUnits)
    {
        // High#FriendlyUnits value is 1f for 10units -> 7units, descending from 1f to 0f for 7units -> 2units and 0f for 2units -> 0units
        float fuzzyInputFriendly = 1 - (7 - theNumberOfUnits) * FUZZYINPUTINCREMENT;
        fuzzyInputFriendly = Min(fuzzyInputFriendly, 1f);
        fuzzyInputFriendly = Max(fuzzyInputFriendly, 0);

        return fuzzyInputFriendly;
    }

    /// <summary>
    /// Returns the y value of the High#EnemyUnits ranging from [0,1]
    /// To get the Low#EnemyUnits, the equation is: (High#EnemyUnits + Low#EnemyUnits = 1)
    /// </summary>
    /// <param name="theNumberOfUnits">The number of remaining active enemy units</param>
    /// <returns>Fuzzy Input for High#EnemyUnits</returns>
    private float Fuzzyification_enemy(float theNumberOfUnits)
    {
        // High#EnemyUnits value is 1f for 10units -> 6units, descending from 1f to 0f for 6units -> 1units and 0f for 1units -> 0units
        float fuzzyInputEnemy = 1 - (6 - theNumberOfUnits) * FUZZYINPUTINCREMENT;
        fuzzyInputEnemy = Min(1f, fuzzyInputEnemy);
        fuzzyInputEnemy = Max(0f, fuzzyInputEnemy);

        return fuzzyInputEnemy;
    }

    /// <summary>
    /// Returns the y value of the High#TargetUnits ranging from [0,1]
    /// To get the Low#TargetUnits, the equation is: (High#TargetUnits + Low#TargetUnits = 1)
    /// </summary>
    /// <param name="theNumberOfUnits">The number of remaining active target units</param>
    /// <returns>Fuzzy Input for High#TargetUnits</returns>
    private float Fuzzyification_target(float theNumberOfUnits)
    {
        // High#TargetUnits value is 1f for 10units -> 8units, descending from 1f to 0f for 8units -> 3units and 0f for 3units -> 0units
        float fuzzyInputTarget = 1 - (8- theNumberOfUnits) * FUZZYINPUTINCREMENT;
        fuzzyInputTarget = Min(fuzzyInputTarget, 1f);
        fuzzyInputTarget = Max(fuzzyInputTarget, 0f);

        return fuzzyInputTarget;
    }

    /// <summary>
    /// Use the Fuzzy inputs to determine the speed output of multiple rules.
    /// </summary>
    /// <param name="fuzzyInput">The Fuzzyification results for the Friendly, Enemy and Target unit counts</param>
    /// <returns>The array of aggressiveness/speed value for the rules</returns>
    private float[] RuleEvaluation (float[] fuzzyInput)
    {
        float HighFriendly = fuzzyInput[Friendly];
        float HighEnemy = fuzzyInput[Enemy];
        float HighTarget = fuzzyInput[Target];
        float LowFriendly = 1 - HighFriendly;
        float LowEnemy = 1 - HighEnemy;
        float LowTarget = 1 - HighTarget;
        float[] outputVariable = new float[NumberOfRules];

        for (int i = 0; i < NumberOfRules; i++)
        {
            switch (i)
            {
                case 0: // Relatively Equivalent ally forces to the enemies' forces
                    outputVariable[i] = Min(HighEnemy, HighFriendly);   // AVERAGE SPEED
                    break;
                case 1: // Dangerous high number of enemies compared to Allies
                    outputVariable[i] = Min(HighEnemy, LowFriendly);    // AGGRESSIVE SPEED
                    break;
                case 2: // Low danger or plenty of targets
                    outputVariable[i] = Max(LowEnemy, HighTarget);      // AVERAGE SPEED
                    break;
                case 3: // Low Danger or few targets
                    outputVariable[i] = Max(LowEnemy, LowTarget);       // MOVE CALMLY
                    break;
                case 4: // Not a high number of enemies and (plenty of targets or plenty of allies)
                    outputVariable[i] = Min((1-HighEnemy), (Max(HighTarget,HighFriendly)));     // AGGRESSIVE SPEED
                    break;
                case 5: // Not a high number of enemies and (few targets or plenty of allies) 
                    outputVariable[i] = Min((1 - HighEnemy), (Max(LowTarget, HighFriendly)));   // AVERAGE SPEED
                    break;
                case 6: // Few enemies or (few targets or few of allies)
                    outputVariable[i] = Max(LowEnemy, (Max(LowTarget, LowFriendly)));           // MOVE CALMLY
                    break;
                case 7: // Few enemies or (few targets or few of allies)
                    outputVariable[i] = Min(LowFriendly, (Min(HighEnemy, HighEnemy)));           // AGGRESSIVE SPEED
                    break;
                default:
                    break;
            }
        }

        return outputVariable;
    }

    /// <summary>
    /// Determine the speed/aggressiveness of the group based on the Rule Evaluations per speed category
    /// </summary>
    /// <param name="fuzzyResults">RuleEvaluation output of Fuzzy Inputs</param>
    /// <returns>Aggressiveness/Speed value for group units</returns>
    private float Defuzzification(float[] fuzzyResults)
    {
        float speedOfTheUnits=0.0f;

        if (GameManager.Instance.HighestFuzzy_NotBlending)
        {
            // Get the largest value from each category
            float calmSpeed = Mathf.Max(fuzzyResults[3], fuzzyResults[6]) * MOVECALMLY;
            float avgSpeed = Mathf.Max(fuzzyResults[0], fuzzyResults[2], fuzzyResults[5]) * AVERAGESPEED;
            float aggrSpeed = Mathf.Max(fuzzyResults[1], fuzzyResults[4], fuzzyResults[7]) * AGGRESSIVE;
            Debug.Log("Largest: \nCalm: " + calmSpeed.ToString("n2") + ", Average: " + avgSpeed.ToString("n2") + ", Aggressive: " + aggrSpeed.ToString("n2"));
            speedOfTheUnits = Mathf.Max(calmSpeed, avgSpeed, aggrSpeed);
        }
        else
        {
            // Largest Of Maximum Blends
            float calmSpeed = Mathf.Max(fuzzyResults[3], fuzzyResults[6]);
            float avgSpeed = Mathf.Max(fuzzyResults[0], fuzzyResults[2], fuzzyResults[5]);
            float aggrSpeed = Mathf.Max(fuzzyResults[1], fuzzyResults[4], fuzzyResults[7]);
            Debug.Log("Blending: \nCalm: " + calmSpeed.ToString("n2") + ", Average: " + avgSpeed.ToString("n2") + ", Aggressive: " + aggrSpeed.ToString("n2"));
            speedOfTheUnits = (calmSpeed * MOVECALMLY + avgSpeed * AVERAGESPEED + aggrSpeed * AGGRESSIVE) / (calmSpeed+avgSpeed+aggrSpeed);
        }

        return speedOfTheUnits;
    }


    /// Lazy Mathf.Max(a,b) call
    private float Max(float a, float b) { return MathF.Max(a, b); }

    /// Lazy Mathf.Min(a,b) call
    private float Min(float a, float b) { return MathF.Min(a, b); }
}
