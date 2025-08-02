using UnityEngine;
using System.Collections;

public class MaterialChangerObject : ObservableObject
{
    [Header("Material Settings")]
    [SerializeField] private Renderer objectRenderer;
    [SerializeField] private Material observedMaterial;
    [SerializeField] private Material unobservedMaterial;
    [SerializeField] private float transitionTime = 1f;
    
    [Header("Color Settings")]
    [SerializeField] private bool useColorChange = false;
    [SerializeField] private Color observedColor = Color.white;
    [SerializeField] private Color unobservedColor = Color.red;
    
    private Material originalMaterial;
    private Coroutine materialTransition;
    
    protected override void OnStart()
    {
        if (objectRenderer == null)
            objectRenderer = GetComponent<Renderer>();
            
        if (objectRenderer != null)
            originalMaterial = objectRenderer.material;
    }
    
    protected override void OnBecameObserved()
    {
        if (materialTransition != null)
            StopCoroutine(materialTransition);
            
        if (useColorChange)
            materialTransition = StartCoroutine(TransitionToColor(observedColor));
        else if (observedMaterial != null)
            objectRenderer.material = observedMaterial;
    }
    
    protected override void OnLeftObservation()
    {
        if (materialTransition != null)
            StopCoroutine(materialTransition);
            
        if (useColorChange)
            materialTransition = StartCoroutine(TransitionToColor(unobservedColor));
        else if (unobservedMaterial != null)
            objectRenderer.material = unobservedMaterial;
    }
    
    private IEnumerator TransitionToColor(Color targetColor)
    {
        Color startColor = objectRenderer.material.color;
        float elapsedTime = 0f;
        
        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionTime;
            objectRenderer.material.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }
        
        objectRenderer.material.color = targetColor;
    }
}