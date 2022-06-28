using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Nav : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform goal;
    private NavMeshAgent agent;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        //agent.destination = goal.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetDestination(Transform trans)
    {
        if (trans)
        {
            agent.destination = trans.position;
        }
    }

    public void Stop()
    {
        agent.destination = this.transform.position;
    }
}
