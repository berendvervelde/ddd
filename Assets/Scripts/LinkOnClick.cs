using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LinkOnClick : MonoBehaviour {

	public void Link(string uri){
		//Application.OpenURL ("market://details?id=com.example.android");
		Application.OpenURL(uri);
	}
}
