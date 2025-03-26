/*
 * StarDome.cs
 * 
 * Original script by Sebastian Lague
 * Source: https://github.com/SebLague/Solar-System
 * 
 */

using UnityEngine;

public class StarDome : MonoBehaviour
{
    public MeshRenderer starPrefab;
    public Vector2 radiusMinMax;
    public int count = 1000;
    const float calibrationDst = 20000;
    public Vector2 brightnessMinMax;


    void Start()
    {
        float starDst = Camera.main.farClipPlane - radiusMinMax.y;
        float scale = starDst / calibrationDst;

        for (int i = 0; i < count; i++)
        {
            MeshRenderer star = Instantiate(starPrefab, Random.onUnitSphere * starDst, Quaternion.identity, transform);
            float t = SmallestRandomValue(6);
            star.transform.localScale = Mathf.Lerp(radiusMinMax.x, radiusMinMax.y, t) * scale * Vector3.one;
            star.material.color = Color.Lerp(Color.black, star.material.color, Mathf.Lerp(brightnessMinMax.x, brightnessMinMax.y, t));
        }
    }

    float SmallestRandomValue(int iterations)
    {
        float r = 1;
        for (int i = 0; i < iterations; i++)
        {
            r = Mathf.Min(r, Random.value);
        }
        return r;
    }

}