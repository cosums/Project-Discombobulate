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

    private Coroutine _flickerRoutine;

    public bool IsFlickering => _flickerRoutine != null;

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

    public void Flicker(float duration, Action onBlackout = null)
    {
        if (_flickerRoutine != null) StopCoroutine(_flickerRoutine);
        _flickerRoutine = StartCoroutine(FlickerRoutine(duration, onBlackout));
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
        _flickerRoutine = null;
    }
}
