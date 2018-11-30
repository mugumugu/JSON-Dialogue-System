using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrettyText : MonoBehaviour {

    public float letterPause = 0.2f;
    public AudioClip[] typeSFX;
    public bool animationComplete = false;
    public string message;
    Text textComp;
    public AudioSource SoundManager;
    void Start(){
        SoundManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioSource>();
    }

    public void animateSelf()
    {
        StopCoroutine(TypeText());
        textComp = GetComponent<Text>();
        textComp.text = "";
        animationComplete = false;
        StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        foreach (char letter in message.ToCharArray())
        {
            textComp.text += letter;

            int index = Random.Range(0, typeSFX.Length);
            SoundManager.PlayOneShot(typeSFX[index]);
 
            yield return 0;
            yield return new WaitForSeconds(letterPause);
        }

        if (textComp.text.Length == message.Length)
        {
            animationComplete = true;
        }
    }
}
