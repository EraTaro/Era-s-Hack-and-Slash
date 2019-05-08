using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title2Main : MonoBehaviour
{
    private void Awake()
    {
        TouchEventHandler.Instance.onTap += OnTap;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void OnTap()
    {
        Debug.Log("OnTap()");
        SceneManager.LoadScene("Main");
    }
}
