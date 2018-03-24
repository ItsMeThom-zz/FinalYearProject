using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scaler : MonoBehaviour {


    float scale = Mathf.Pow(2f, 1.0f / 12f);
    AudioSource sound;
    List<int> vals = new List<int>();

    void Start()
    {

        sound = GetComponent<AudioSource>();
        GenerateRandomScale();
        StartCoroutine(PlayScale());
    }

    public void GenerateRandomScale()
    {
        vals = new List<int> { 1, 6, 4, 5 };
        //Debug.Log("GEN");
        //vals = new List<int>();
        //int inital = UnityEngine.Random.Range(0, 8);
        //for(int i = 0; i < 16; i++)
        //{
        //    vals.Add(UnityEngine.Random.Range(0, 8));
        //}
    }

    IEnumerator PlayScale()
    {
        foreach(int i in vals)
        {
            Debug.Log(i);
            sound.pitch = Mathf.Pow(scale, i-1);
            sound.Play();
            yield return new WaitForSeconds(3.0f);
        }
        //for (int i = 0; i < 12; i++)
        //{

            
        //}
    }
}

