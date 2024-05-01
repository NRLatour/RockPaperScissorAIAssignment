using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawnPlacement : MonoBehaviour
{

    [SerializeField]
    private GameObject Units;

    [SerializeField]
    private GameObject TriangleFormationPrefab;
    [SerializeField]
    private GameObject UnbalancedPrefab;
    [SerializeField]
    private GameObject DesperatePrefab;
    [SerializeField]
    private GameObject RockPrefab;
    [SerializeField]
    private GameObject PaperPrefab;
    [SerializeField]
    private GameObject ScissorsPrefab;

    private void Start()
    {
        if (Units.transform.childCount == 0)
        {
            Triangle();
        }
    }

    public void ChangeFormation(UnitFormations formation)
    {
        switch (formation)
        {
            case UnitFormations.Triangle:
                Triangle();
                break;
            case UnitFormations.Circular:
                Circular();
                break;
            case UnitFormations.Scattered:
                Circular(true);
                break;
            case UnitFormations.Unbalanced:
                Unbalanced();
                break;
            case UnitFormations.Desperate:
                Desperate();
                break;
            default:
                Triangle();
                break;
        }
    }

    private void RemoveUnits()
    {
        if (Units.transform.childCount < 1)
            return;
        
        Transform[] children = Units.transform.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child == Units.transform)
                continue;
            Destroy(child.gameObject);
        }
    }

    private void Triangle()
    {
        RemoveUnits();
        Instantiate(TriangleFormationPrefab, Units.transform.position, Quaternion.identity, Units.transform);
        GameManager.Instance.UnitFormation = UnitFormations.Triangle;
    }

    private void Circular(bool randomize = false)
    {
        RemoveUnits();
        float angle = 0f;
        float increment = 10f;
        float gap = 20f;
        int maxUnits = 10;
        int multipler = 1;
        int middleDistance = 50;
        float varyingDistance = 10f;
        GameObject[] Prefabs = { RockPrefab, PaperPrefab, ScissorsPrefab };
        int[] count = { 0, 0, 0 };
        int index = (int)(Random.value * 2.99f);


        for (int i = 0; i < Prefabs.Length; i++)
        {
            for (int j = 0; j < maxUnits; j++)
            {
                float radianAngle = angle * Mathf.Deg2Rad;
                float variation = (varyingDistance + Random.Range(-3, 3));
                float xOffSet = (middleDistance + (multipler * variation)) * Mathf.Cos(radianAngle);
                float zOffSet = (middleDistance + (multipler * variation)) * Mathf.Sin(radianAngle);
                Vector3 pos = transform.position + new Vector3(xOffSet, 0, zOffSet);
                if (randomize)
                {
                    int randomIndex = GetRandomPrefab(count);
                    count[randomIndex]++;
                    Instantiate(Prefabs[randomIndex], pos, Quaternion.identity, Units.transform);
                }
                else
                {
                    Instantiate(Prefabs[(index + i)%3], pos, Quaternion.identity, Units.transform);
                }

                multipler *= -1;
                angle += increment;
            }
            angle += gap;
        }

        GameManager.Instance.UnitFormation = UnitFormations.Circular;
    }

    private int GetRandomPrefab(int[] count)
    {
        int index = (int)(Random.value * 2.99f);
        do
        {
            index = (index + 1) % 3;
        } while (count[index] > 9);
        return index;
    }

    private void Unbalanced()
    {
        RemoveUnits();
        Instantiate(UnbalancedPrefab, Units.transform.position, Quaternion.identity, Units.transform);
        GameManager.Instance.UnitFormation = UnitFormations.Triangle;
    }

    private void Desperate()
    {
        RemoveUnits();
        Instantiate(DesperatePrefab, Units.transform.position, Quaternion.identity, Units.transform);
        GameManager.Instance.UnitFormation = UnitFormations.Triangle;
    }
}

