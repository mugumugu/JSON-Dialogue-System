using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class inheritParent : MonoBehaviour {

	Button x;
	void Start(){
		x = GetComponentInParent<Button>();
	}
	// Update is called once per frame
	void Update () {
		if(!x.interactable){
			Color c = x.colors.disabledColor;
			GetComponent<Text>().color = c;
		}else{
			Color c = x.colors.normalColor;
			GetComponent<Text>().color = c;
		}
	}
}
