using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFade : MonoBehaviour
{
    bool running = false;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!running)
            {
                running = true;
                StartCoroutine(FadeCam());
            }
        }
    }

    IEnumerator FadeCam()
    {
        FadeCamera fc = GetComponent<FadeCamera>();

        fc.FadeOut(0.3f);

        yield return new WaitUntil(()=>!fc.isFading);

        yield return new WaitForFixedUpdate();

        GetComponent<Camera>().enabled = false;
        yield return null;
    }
}