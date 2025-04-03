using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class SceneCreatorWindow : EditorWindow
{
    private string sceneName = "NewScene";
    private int selectedPlanetIndex = 0;
    private PlanetList planetList;
    private GameObject levelPrefab;
    private GameObject platform;
    private float timeOfDay = 12f;
    private float rotationOfPlanet = 0f;
    private string selectedPlanet;

    [MenuItem("Tools/Scene Creator")]
    public static void ShowWindow()
    {
        GetWindow<SceneCreatorWindow>("Scene Creator");
    }

    private void OnEnable()
    {
        planetList = Resources.Load<PlanetList>("PlanetList");
        levelPrefab = Resources.Load<GameObject>("Level");
        platform = Resources.Load<GameObject>("Platform");
    }

    private void OnGUI()
    {
        GUILayout.Label("Scene Settings", EditorStyles.boldLabel);
        sceneName = EditorGUILayout.TextField("Scene Name", sceneName);

        if (planetList != null && planetList.planets.Length > 0)
        {
            selectedPlanetIndex = EditorGUILayout.Popup("Select Planet", selectedPlanetIndex, planetList.planets);
        }
        else
        {
            EditorGUILayout.HelpBox("No planets found! Create a PlanetList asset in Resources.", MessageType.Warning);
        }

        timeOfDay = EditorGUILayout.Slider("Time of Day", timeOfDay, 0, 24);

        rotationOfPlanet = EditorGUILayout.Slider("Rotation of planet", rotationOfPlanet, -360, 360);

        if (GUILayout.Button("Create Scene"))
        {
            CreateScene();
        }
    }

    private void CreateScene()
    {
        if (planetList == null || planetList.planets.Length == 0)
        {
            Debug.LogError("No planet selected.");
            return;
        }

        selectedPlanet = planetList.planets[selectedPlanetIndex];

        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);

        RenderSettings.ambientLight = Color.Lerp(Color.black, Color.white, timeOfDay / 24f);

        if (levelPrefab)
        {
            GameObject levelInstance = PrefabUtility.InstantiatePrefab(levelPrefab) as GameObject;
            levelInstance.name = $"{selectedPlanet}_Level";
            Undo.RegisterCreatedObjectUndo(levelInstance, "Create Level");

            ReplacePlanet(levelInstance, selectedPlanet);
        }
        else
        {
            Debug.LogError("Level prefab not found! Ensure it is inside 'Assets/Resources/' and named 'Level'.");
        }

        Debug.Log($"Scene '{sceneName}' created on planet '{selectedPlanet}' with TimeOfDay: {timeOfDay}");
    }

    private void ReplacePlanet(GameObject levelInstance, string selectedPlanet)
    {
        string planetPath = $"Planets/{selectedPlanet}/{selectedPlanet}";

        GameObject planetModel = Resources.Load<GameObject>(planetPath);

        if (planetModel == null)
        {
            Debug.LogError($"Planet model '{selectedPlanet}' not found in 'Assets/Resources/Planets/{selectedPlanet}/'!");
            return;
        }

        Transform planetTransform = levelInstance.transform.Find("Planet");

        if (planetTransform != null)
        {
            DestroyImmediate(planetTransform.gameObject);
        }

        GameObject newPlanet = PrefabUtility.InstantiatePrefab(planetModel) as GameObject;

        newPlanet.transform.SetParent(levelInstance.transform, false);
        if (selectedPlanet != "Saturn")
        {
            newPlanet.transform.localScale = new Vector3(50f, 50f, 50f);
        }
        
        newPlanet.transform.localPosition = Vector3.zero;
        foreach (Transform child in newPlanet.transform)
        {
            child.localPosition = Vector3.zero;
        }

        newPlanet.transform.Rotate(Vector3.right, rotationOfPlanet);
        newPlanet.name = "Planet";

        // Place the platform on the surface of the planet
        PlacePlatformOnPlanetSurface(newPlanet, levelInstance);
    }

    private void PlacePlatformOnPlanetSurface(GameObject planet, GameObject levelInstance)
    {
        if (platform == null)
        {
            Debug.LogError("Platform prefab not found!");
            return;
        }

        // Assuming the planet is spherical and centered at (0, 0, 0)
        // We can place the platform on the surface at a fixed latitude/longitude.

        // For this example, we're placing the platform at a fixed point on the surface
        // (you could choose another method to calculate this dynamically based on rotation).
        float planetRadius = Utils.GetRadius(selectedPlanet) * planet.transform.localScale.x;  // Planet scale is 50, so we'll use this as the radius

        // Calculate the platform's position on the surface of the planet
        Vector3 platformPosition = planet.transform.position + new Vector3(0f, planetRadius, 0f);

        // Instantiate the platform and position it on the surface
        GameObject platformInstance = PrefabUtility.InstantiatePrefab(platform) as GameObject;
        platformInstance.transform.SetParent(levelInstance.transform, false);
        platformInstance.transform.position = platformPosition;
        var playerFall = platformInstance.GetComponentInChildren<PlayerFall>();
        playerFall.fallThreshold = 0.45f;

        // Optional: Rotate the platform if needed to align with the planet's surface
        //platformInstance.transform.LookAt(planet.transform);  // Makes the platform face the planet's center
        platformInstance.name = "Platform";
    }
}
