using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.Mathematics;

public class Starship : Agent
{
    private bool firstEpisode = true;
    private SuccessRate successRateScript;
    public float maxAcceleration;

    public float trainingSpeed;

    public Transform star;
    private List<Transform> masses = new List<Transform>();
    private Rigidbody rb;

    public Transform forwardThrust;
    public Transform backThrust;
    public Transform leftThrust;
    public Transform rightThrust;

    public float timeSpeed;

    private bool success = false;

    private bool firstFrame = true;

    public int shipID;

    public Transform target;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<SystemEconomy>(out SystemEconomy econ))
        {
            AddReward(-1000000 * trainingSpeed);
            successRateScript.Report(false);
            EndEpisode();
            Destroy(gameObject);
        }
        else if (other.tag == "Target" && other.transform == target)
        {
            AddReward(1000000 * trainingSpeed);
            successRateScript.Report(true);
            EndEpisode();
            Destroy(gameObject);
        }

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    public override void OnEpisodeBegin()
    {
        SetReward(0);
        if (firstEpisode)
        {
            firstEpisode = false;
        }
        else
        {
            successRateScript.Report(success);
            success = false;
        }

        //transform.localPosition = new Vector3(-23f, 0, -23f);
        rb.velocity = Vector3.zero;
        rb.rotation = Quaternion.Euler(Vector3.zero);
        rb.angularVelocity = Vector3.zero;



    }




    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(transform.localEulerAngles.y);
        sensor.AddObservation(target.localPosition);
        sensor.AddObservation(target.GetComponent<Rigidbody>().velocity);
        sensor.AddObservation(rb.velocity);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector3 thrust = new Vector3(actions.ContinuousActions[1], 0, 0);
        forwardThrust.localScale = new Vector3(math.clamp(actions.ContinuousActions[1], 0, 1), 1, 1);
        backThrust.localScale = new Vector3(math.clamp(actions.ContinuousActions[1], -1, 0), 1, 1);
        rb.AddForce(transform.rotation * thrust * maxAcceleration);
        rb.angularVelocity = new Vector3(0, actions.ContinuousActions[0], 0);
        leftThrust.localScale = new Vector3(0.15f, 1, math.clamp(actions.ContinuousActions[0], -1, 0));
        rightThrust.localScale = new Vector3(0.15f, 1, math.clamp(actions.ContinuousActions[0], 0, 1));

    }


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        successRateScript = GameObject.Find("SuccessRate").GetComponent<SuccessRate>();
    }

    // Start is called before the first frame update
    void Start()
    {


        Vector3 towardsStar = star.localPosition - transform.localPosition;
        Vector3 rotated = Quaternion.AngleAxis(90, Vector3.up) * towardsStar;
        rb.AddForce(rotated * 1);
        Time.timeScale = timeSpeed;


    }



    // Update is called once per frame
    void FixedUpdate()
    {
        if (firstFrame)
        {
            masses = star.GetComponentsInChildren<Transform>().ToList();
            masses.Append(star);
            firstFrame = false;
        }


        for (int i = 0; i < masses.Count(); i++)
        {
            Vector3 towardsMass = masses[i].localPosition - transform.localPosition;
            towardsMass = (float)(1 / Math.Pow(towardsMass.magnitude, 2)) * towardsMass.normalized * masses[i].GetComponent<Rigidbody>().mass * rb.mass;
            rb.AddForce(towardsMass * 0.001f);
        }

        //Debug.Log("Reward: " + (10 - (transform.localPosition - target.localPosition).magnitude) * trainingSpeed);
        AddReward((-1 * (transform.localPosition - target.localPosition).magnitude) * trainingSpeed);


    }
}
