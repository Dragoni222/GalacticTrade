using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuccessRate : MonoBehaviour
{
    // Start is called before the first frame update
    public float successRate = 0;
    public float attempts = 0;
    public float successes = 0;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Report(bool success)
    {
        attempts += 1;
        successes += success ? 1 : 0;
        successRate = successes / attempts;
    }
}
