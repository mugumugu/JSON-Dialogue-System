using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class mouthSwitch : MonoBehaviour {

	public Texture[] mouth;
	Texture defaultMouth;
	RawImage m;
	public bool animate;

	// Use this for initialization
	void Start () {
		m = GetComponent<RawImage>();
		defaultMouth = m.texture;
	}
	
	private float time = 0.0f;
	public float interpolationPeriod = 0.1f;
	int index;
	void Update () {
		if(animate){
			time += Time.deltaTime;
		
			if (time >= interpolationPeriod) {
				time = 0.0f;

				if(index==mouth.Length)
					index=0;
		
				m.texture = mouth[index];

				index+=1;

				Debug.Log("executing");

			}
		}else{
			m.texture = defaultMouth;
		}
	}

}
