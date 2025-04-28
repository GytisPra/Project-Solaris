using UnityEngine;
using UnityEngine.SceneManagement;

public class SolarPad : MonoBehaviour
{
    public static SolarPad Instance { get; private set; }
    private static Camera solarPadCam;
    private static Canvas solarPadCanvas;
    private bool isTheoryOpen;
    private GameObject currentTheoryPage;

    public GameObject subjects;

    private void Start()
    {
        solarPadCanvas = GetComponentInChildren<Canvas>();
        solarPadCam = GetComponentInChildren<Camera>();

        solarPadCam.gameObject.SetActive(false);
        solarPadCanvas.gameObject.SetActive(false);
    }
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Open()
    {
        if (solarPadCam == null)
        {
            Debug.LogError("Cannot open solar pad. No camera found!");
            return;
        }

        solarPadCam.gameObject.SetActive(true);
        solarPadCanvas.gameObject.SetActive(true);

        solarPadCam.depth = 10;

        GameStateManager.Instance.SetState(GameState.Menu);
    }

    public void Close()
    {
        if (isTheoryOpen)
        {
            if (currentTheoryPage != null)
            {
                currentTheoryPage.SetActive(false);
            }
            subjects.SetActive(true);
            currentTheoryPage = null;
            isTheoryOpen = false;
            return;
        }

        solarPadCam.depth = -10; 

        solarPadCam.gameObject.SetActive(false);
        solarPadCanvas.gameObject.SetActive(false);

        GameStateManager.Instance.SetState(GameState.Gameplay);
    }

    public void OpenTheory(GameObject theoryPage)
    {
        subjects.SetActive(false);
        theoryPage.SetActive(true);
        currentTheoryPage = theoryPage;
        isTheoryOpen = true;
    }
}

