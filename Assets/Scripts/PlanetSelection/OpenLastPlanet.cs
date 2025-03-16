using JetBrains.Annotations;
using UnityEngine;

public class OpenLastPlanet : MonoBehaviour
{
    // TODO: fix a bug where the line dissapears forever


    public GameObject planets;
    public PlanetSelectionUIManager planetSelectionUIManager;
    public CameraRotation cameraRotation;

    private string lastPlanet;
    
    void Start()
    {
        if (!PlayerPrefs.HasKey("LastPlanet")) {
            return;
        }

        lastPlanet = PlayerPrefs.GetString("LastPlanet");
        GameObject planet = planets.transform.Find(lastPlanet).gameObject;

        if (lastPlanet == "Saturn") {
           planet = planet.transform.Find("SaturnST").gameObject;
        }

        if(planet == null) {
            Debug.LogError("Last Planet does not exist!");
            cameraRotation.ResetCamera();
            return;
        }

        // if (planet.TryGetComponent<OrbitRing>(out var orbitRing)) {
        //     orbitRing.SetLineVisibility(false);
        // }

        if (cameraRotation != null) {
            cameraRotation.SetTargetObject(planet);
        } else {
            Debug.LogError("Camera rotation component not set in inspector!");
        }

        if (planetSelectionUIManager != null) {
            planetSelectionUIManager.SetMainMenuCanvasActive(false);
            planetSelectionUIManager.SetPlanetSelectionCanvasActive(false);
            planetSelectionUIManager.SetPlanetUICanvasActive(true);
        }
        else {
            Debug.LogError("Planet selection UI manager not set in inspector!");

        }

    }
}
