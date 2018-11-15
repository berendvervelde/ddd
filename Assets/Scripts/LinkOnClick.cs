using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LinkOnClick : MonoBehaviour {

	public void Link(string uri){
		Application.OpenURL(uri);
	}
}
