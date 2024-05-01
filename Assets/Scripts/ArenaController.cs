using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaController : MonoBehaviour
{
    private Vector3 initialCameraPosition;
    private Vector3 initialArenaScale;
    private float initialArenaLightIntensity;

    [SerializeField]
    Material BoundaryMaterial;
    [SerializeField]
    Camera MainCamera;
    [SerializeField]
    Light ArenaLight;

    private void Start()
    {
        initialCameraPosition = MainCamera.transform.position;
        initialArenaScale = transform.localScale;
        initialArenaLightIntensity = ArenaLight.intensity;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
       MainCamera.transform.position = new Vector3(MainCamera.transform.position.x, MainCamera.transform.position.y - Time.deltaTime/2, MainCamera.transform.position.z - Time.deltaTime/8.4f);
       transform.localScale = new Vector3(transform.localScale.x - Time.deltaTime, transform.localScale.y, transform.localScale.z - Time.deltaTime);
        ArenaLight.intensity -= Time.deltaTime*1.5f;
    }

    public void ResetArena()
    {
        MainCamera.transform.position = initialCameraPosition;
        transform.localScale = initialArenaScale;
        ArenaLight.intensity = initialArenaLightIntensity;
    }
}
