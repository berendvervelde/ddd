using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deathlightsAnimate : MonoBehaviour {

	public GameObject lights1;
	public GameObject lights2;

	void Start () {
		StartCoroutine(RotateSpecialFx(this.lights1, true));
		StartCoroutine(RotateSpecialFx(this.lights2, false));
	}

	protected IEnumerator RotateSpecialFx(GameObject go, bool clockwise) {
        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
		float rotateSpeed = -.9f;
		if(clockwise){
			rotateSpeed = .9f;
		}

        sr.color = new Color(1f,1f,1f,0f);
        int rotate = 0;

        while (sr.color.a < 0.9f) {
            sr.color += new Color(0, 0, 0, 0.05f);

            go.transform.Rotate (Vector3.forward * rotateSpeed);
            yield return null;
        }

        while (rotate < 10){
            rotate ++ ;
            go.transform.Rotate (Vector3.forward * rotateSpeed);
            yield return null;
        }
    
        while (sr.color.a > 0f) {
            sr.color -= new Color(0, 0, 0, 0.05f);
            go.transform.Rotate (Vector3.forward * rotateSpeed);
            yield return null;
        }
        Destroy(go);

		if(this.gameObject != null){
			Destroy(this.gameObject);
		}
    }
}
