using System;
using System.Collections;
using UnityEngine;

public class LightingManager : MonoBehaviour
{
    public static LightingManager Instance { get; private set; }
    private static Light[] _sceneLights;

    public float minFlickerInterval = 0.03f;
    public float maxFlickerInterval = 0.15f;
    public float BlackoutHold = 0.6f;

    private Coroutine _routine;

    public bool IsFlickering => _routine != null;

    void Awake()
    {
        if (Instance != null) {Destroy(gameObject); return;}
        Instance = this;
    }

    void Start()
    {
        _sceneLights = FindObjectsByType<Light>(FindObjectsSortMode.None);
        Debug.Log($"[LightingManager]: Found {_sceneLights.Length} lights");
    }

    void Update()
    {
        
    }

    public void SetAllLights(bool on)
    {
        foreach (var light in _sceneLights)
        {
            if (light == null) continue;
            light.enabled = on;
        }
    }


    public void Blackout(float hold, Action onBlackout = null)
    {
        _routine = StartCoroutine(BlackoutRoutine(hold, onBlackout));
    }

    private IEnumerator BlackoutRoutine(float hold, Action onBlackout)
    {
        SetAllLights(false);
        onBlackout?.Invoke();
 
        yield return new WaitForSeconds(hold);
 
        SetAllLights(true);
        _routine = null;
    }

    public void Flicker(float duration, Action onBlackout = null)
    {
        if (_routine != null) StopCoroutine(_routine);
        _routine = StartCoroutine(FlickerRoutine(duration, onBlackout));
    }

    private IEnumerator FlickerRoutine(float duration, Action onBlackout)
    {
        float end = Time.time + duration;
        while (Time.time < end)
        {
            SetAllLights(UnityEngine.Random.value > 0.5f);
            yield return new WaitForSeconds(UnityEngine.Random.Range(minFlickerInterval, maxFlickerInterval));
        }

        SetAllLights(false);
        onBlackout?.Invoke();

        yield return new WaitForSeconds(BlackoutHold);

        SetAllLights(true);
        _routine = null;
    }
}
