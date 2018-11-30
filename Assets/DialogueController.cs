using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Linq;

public class DialogueController : MonoBehaviour {

    private StatController sc;

    public Text main;
    public GameObject cont, choice1, choice2;
    int id,index = 0;

    string path;
    string jsonString;
    Dialogue s;

    public AudioClip tapClip;
    public AudioSource SoundManager;
    public bool beginning = true;
    public int patternIndex = 0;
    TextAsset introAsset, supplyAsset, energyAsset, crewAsset;
    Dialogue introDialogue, supplyDialogue, energyDialogue, crewDialogue;
    Content[] introContent, supplyContent, energyContent, crewContent;
    List<int> ci = new List<int>();
    List<int> cs = new List<int>();
    List<int> ce = new List<int>();
    List<int> cc = new List<int>();
    public int days = 0;

    public int scenarioID = 0,
                scenarioIndex = 0;

    public List<string> triggered = new List<string>();
    public List<Content> currentScenes = new List<Content>();

    private void Start()
    {
        SoundManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioSource>();
        sc = GetComponent<StatController>();

        // LOAD ALL JSON ASSETS
        introAsset = Resources.Load<TextAsset>("intro");
        supplyAsset = Resources.Load<TextAsset>("supply");
        energyAsset = Resources.Load<TextAsset>("energy");
        crewAsset = Resources.Load<TextAsset>("crew");

        // PARSE JSON FILES
        introDialogue = JsonUtility.FromJson<Dialogue>(introAsset.text);
        introContent = introDialogue.scenario;

        supplyDialogue = JsonUtility.FromJson<Dialogue>(supplyAsset.text);
        supplyContent = supplyDialogue.scenario;

        energyDialogue = JsonUtility.FromJson<Dialogue>(energyAsset.text);
        energyContent = energyDialogue.scenario;

        crewDialogue = JsonUtility.FromJson<Dialogue>(crewAsset.text);
        crewContent = crewDialogue.scenario;

        print(introContent);

        // CHECK ASSET DIALOGUE COUNT
        for (int a = 0; a != introContent.Length; a++)
        {
            int toCheck = introContent[a].id;
            if (ci.Contains(toCheck)==false)
                ci.Add(toCheck);
        }

        for (int a = 0; a != supplyContent.Length; a++)
        {
            int toCheck = supplyContent[a].id;
            if (cs.Contains(toCheck) == false)
                cs.Add(toCheck);
        }
        for (int a = 0; a != energyContent.Length; a++)
        {
            int toCheck = energyContent[a].id;
            if (ce.Contains(toCheck) == false)
                ce.Add(toCheck);
        }
        for (int a = 0; a != crewContent.Length; a++)
        {
            int toCheck = crewContent[a].id;
            if (cc.Contains(toCheck) == false)
                cc.Add(toCheck);
        }
        
        // CHECK IF NEW GAME
        generatePattern();
        
    }

    private void generatePattern()
    {
        scenarioID = 0;
        scenarioIndex = 0;
        currentScenes.Clear();

        if (patternIndex == 0)          // intro
        {
            patternIndex += 1;
            createScenario(0);
            Debug.Log("Intro Created");
        }else if (patternIndex == 1)    // supply
        {
            patternIndex += 1;
            createScenario(1);
            Debug.Log("Supply Scenario Created");
        }
        else if(patternIndex == 2)      // energy
        {
            patternIndex += 1;
            createScenario(2);
            Debug.Log("Energy Scenario Created");
        }
        else if(patternIndex == 3){     // crew
            patternIndex+= 1;
            createScenario(3);
            Debug.Log("Crew Scenario Created");
        }else if(patternIndex >= 4){     // crew
            endDay();
        }
        
    }

    public void endDay(){
        displaySentence("Sleep mode");
        resetButtons();
        cont.GetComponent<Button>().onClick.AddListener(delegate{
            generatePattern();
        });
        showButtons("proceed");
        patternIndex= 1;
        days+=1;

        if(sc.supply>=50)
            sc.supply -= Random.Range(10,30);

        if(sc.energy>=50)
            sc.energy -= Random.Range(10,30);
    }

    public void createScenario(int type)
    {
        int i = Random.Range(1, ci.Count);
        int s = Random.Range(1, cs.Count);
        int e = Random.Range(1, ce.Count);
        int c = Random.Range(1, cc.Count);
        int r = 0,
            t = 0;
        Content[] k = null;

        if (type == 0){
            t = introContent.Length;
            r = i;
            k = introContent;
        }else if (type == 1) {
            t = supplyContent.Length;
            r = s;
            k = supplyContent;
        }
        else if (type == 2) {
            t = energyContent.Length;
            r = e;
            k = energyContent;
        } else if (type == 3) {
            t = crewContent.Length;
            r = c;
            k = crewContent;
        }

        for (int a = 0; a != t; a++)
        {
            if (k[a].id == r)
                currentScenes.Add(k[a]);
        }

        renderScenario();

    }

    public void playTapSound(){
        SoundManager.PlayOneShot(tapClip);
    }

    public void displaySentence(string s){
        main.GetComponent<PrettyText>().message = s;
        main.GetComponent<PrettyText>().animateSelf();
    }

    public void resetButtons(){
        cont.GetComponent<Button>().onClick.RemoveAllListeners();
        choice1.GetComponent<Button>().onClick.RemoveAllListeners();
        choice2.GetComponent<Button>().onClick.RemoveAllListeners();
    }

    public void showButtons(string type){
        if(type=="proceed"){
            cont.SetActive(true);
            choice1.SetActive(false);
            choice2.SetActive(false);
        }else if(type=="choice"){
            cont.SetActive(false);
            choice1.SetActive(true);
            choice2.SetActive(true);
        }
    }

    public void bindButton(int type){
        if(currentScenes[scenarioID].result.Contains(':')){
            string x = currentScenes[scenarioID].result.Split(':')[type];
            if (x == "cont")
                proceedButton();
            else if (x == "stop")
                generatePattern();
            else if ((x=="supply")||(x== "crew")||(x== "energy"))
                showResult(type);
        }else{
            triggered.Add(currentScenes[scenarioID].response.Split(':')[type]);
            displaySentence(currentScenes[scenarioID].consq.ToString());
            
            resetButtons();
            cont.GetComponent<Button>().onClick.AddListener(delegate{
                proceedButton();
            });
            showButtons("proceed");
        }

        playTapSound();
    }

    public void showResult(int type){
        if(currentScenes[scenarioID].consq!=null){
            string state = generateStats(currentScenes[scenarioID].result);
            if(state=="add"){
                setStats(state,currentScenes[scenarioID].result.Split(':')[0]);
                displaySentence(currentScenes[scenarioID].consq.Split(':')[0].ToString());
            }else if(state=="remove"){
                setStats(state,currentScenes[scenarioID].result.Split(':')[1]);
                displaySentence(currentScenes[scenarioID].consq.Split(':')[1].ToString());
            }else if(state=="death"){
                setStats(state,currentScenes[scenarioID].result.Split(':')[1]);
                displaySentence("Death");
            }

            resetButtons();
            cont.GetComponent<Button>().onClick.AddListener(delegate{
                generatePattern();
            });
            showButtons("proceed");
        }else{
            Debug.Log("Consqeunce Message not set");
        }
    }

    public void proceedButton(){
        scenarioID += 1;
        if (scenarioID == currentScenes.Count)
            generatePattern();
        else
            renderScenario();

        playTapSound();
    }

    public void renderScenario()
    {
        resetButtons();
        
        if (scenarioIndex != currentScenes.Count)
        {
            displaySentence(currentScenes[scenarioID].sentence.ToString());

            if (currentScenes[scenarioID].response.Contains(":")==false){
                showButtons("proceed");
                cont.GetComponent<Button>().onClick.AddListener(delegate {
                    proceedButton();
                });
            }else{
                showButtons("choice");
                choice1.GetComponentInChildren<Text>().text = currentScenes[scenarioID].response.Split(':')[0];
                choice2.GetComponentInChildren<Text>().text = currentScenes[scenarioID].response.Split(':')[1];

                choice1.GetComponent<Button>().onClick.AddListener(delegate{
                    bindButton(0);
                });
                
                choice2.GetComponent<Button>().onClick.AddListener(delegate{
                    bindButton(1);
                });
            }
        }
    }

    private void Update()
    {
        // CHECKS IF TEXT ANIMATION IS COMPLETE
        if (main.GetComponent<PrettyText>().animationComplete)
        {
            cont.GetComponent<Button>().interactable = true;
            choice1.GetComponent<Button>().interactable = true;
            choice2.GetComponent<Button>().interactable = true;
        }
        else
        {
            cont.GetComponent<Button>().interactable = false;
            choice1.GetComponent<Button>().interactable = false;
            choice2.GetComponent<Button>().interactable = false;
        }
    }

    private string generateStats(string type){

        if(type=="death"){
            return "death";
        }else{
            if(Random.value < 0.50f)
                return "add";
            else
                return "remove";
        }
    }

    public void setStats(string q,string type){
        if(q=="add"){
            if(type=="crew"){
                sc.supply-=10;
                sc.crew+=1;
            }else if(type=="energy"){
                sc.supply-=10;
                sc.energy+=10;
            }else if(type=="supply"){
                sc.energy-=10;
                sc.supply+=10;
            }

            if(sc.supply>=100){sc.supply=100;}
            if(sc.energy>=100){sc.energy=100;}
        }else if(q=="remove"){
            if(type=="crew")
                sc.crew-=1;
            else if(type=="energy")
                sc.energy-=10;
            else if(type=="supply")
                sc.supply-=10;

            if(sc.supply<=0){sc.supply=0;}
            if(sc.energy<=0){sc.energy=0;}
        }else if(q=="death"){
            if(type=="crew")
                sc.crew=0;
            else if(type=="energy")
                sc.energy=0;
            else if(type=="supply")
                sc.supply=0;
        }
        
        sc.energy-=10;
    }

    public void checkStats()
    {
        if ((sc.crew <= 0) || (sc.energy <= 0) || (sc.supply <= 0))
        {
            main.GetComponent<PrettyText>().message = "Game Over";
            main.GetComponent<PrettyText>().animateSelf();

            cont.SetActive(true);
            choice1.SetActive(false);
            choice2.SetActive(false);
        }
    }

}

[System.Serializable]
public class Dialogue
{
    public Content[] scenario;
}

[System.Serializable]
public class Content
{
    public int id, index,character,mood;
    public string type,sentence, response, result,consq;
}
