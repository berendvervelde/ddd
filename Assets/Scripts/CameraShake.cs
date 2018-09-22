///Daniel Moore (Firedan1176) - Firedan1176.webs.com/
///26 Dec 2015
///
///Shakes camera parent object

using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour {

    private float shakeAmount;//The amount to shake this frame.
    private float shakeDuration;//The duration this frame.

    private Vector3 originalPos;

    //Readonly values...
    float shakePercentage;//A percentage (0-1) representing the amount of shake to be applied when setting rotation.
    float startAmount;//The initial shake amount (to determine percentage), set when ShakeCamera is called.
    float startDuration;//The initial shake duration, set when ShakeCamera is called.

    bool isRunning = false; //Is the coroutine running right now?

    public void ShakeCamera(float amount, float duration) {

        shakeAmount = amount;//Add to the current amount.
        startAmount = shakeAmount;//Reset the start amount, to determine percentage.
        shakeDuration = duration;//Add to the current time.
        startDuration = shakeDuration;//Reset the start time.

        this.originalPos = this.transform.localPosition;

        if (!isRunning) StartCoroutine(Shake());//Only call the coroutine if it isn't currently running. Otherwise, just set the variables.
    }


    IEnumerator Shake() {
        isRunning = true;

        while (shakeDuration > 0.01f) {

            shakePercentage = shakeDuration / startDuration;//Used to set the amount of shake (% * startAmount).

            shakeAmount = startAmount * shakePercentage;//Set the amount of shake (% * startAmount).
			shakeDuration -= Time.deltaTime;
            //shakeDuration = Mathf.Lerp(shakeDuration, 0, Time.deltaTime);//Lerp the time, so it is less and tapers off towards the end.

            this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, originalPos + Random.insideUnitSphere * shakeAmount, Time.deltaTime * 3);

            yield return null;
        }
        this.transform.localPosition = originalPos;
        isRunning = false;
    }
}