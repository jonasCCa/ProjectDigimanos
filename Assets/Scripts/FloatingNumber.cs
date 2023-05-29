using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingNumber : MonoBehaviour
{
    [SerializeField] TMP_Text floatingText;
    [SerializeField] float animationLength;
    [SerializeField] float animationHeight;
    private float startingY;

    private void Start()
    {
        startingY = transform.position.y;
        AnimateStart();
    }

    void AnimateStart()
    {
        StartCoroutine(Animate());
    }

    public FloatingNumber UpdateText(string newText)
    {
        if (newText != null)
            floatingText.text = newText;

        return this;
    }

    public FloatingNumber UpdateTextGradient(TMP_ColorGradient newGradient)
    {
        if (newGradient != null)
            floatingText.colorGradientPreset = newGradient;

        return this;
    }

    IEnumerator Animate()
    {
        float t = 0;
        Vector3 originalPos = transform.position;
        Vector3 targePos = transform.position + new Vector3(0, animationHeight, 0);

        // Translate up
        while (transform.position.y < startingY + animationHeight)
        {
            transform.position = Vector3.Lerp(originalPos, targePos, t / ((animationLength / 3)*2));

            t += Time.deltaTime;

            yield return null;
        }

        t = 0;
        Color originalColor = floatingText.color;
        Color targetColor = new Color(floatingText.color.r, floatingText.color.g, floatingText.color.b, 0);
        // Fade out
        while (floatingText.color.a > 0)
        {
            floatingText.color = Color.Lerp(originalColor, targetColor, t / (animationLength / 3));

            t += Time.deltaTime;

            yield return null;
        }

        Destroy(transform.parent.gameObject);
    }
}
