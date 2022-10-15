using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManger : MonoBehaviour
{
    public static SceneManger instance;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        
    }

    public void ChangeScene(int SceneID)
    {     
       StartCoroutine(Transition(SceneID));
    
    }

    IEnumerator Transition(int SceneID)
    {
        DontDestroyOnLoad(gameObject);
        yield return SceneManager.LoadSceneAsync(SceneID);
        MapManager.instance.AllNodes.RemoveRange(0, MapManager.instance.AllNodes.Count);
        MapManager.instance.Initiate();
        Debug.Log("NewSceneLoaded " + SceneManager.GetActiveScene().buildIndex);
        Destroy(gameObject);
    }
}
