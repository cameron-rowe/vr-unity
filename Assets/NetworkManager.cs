using UnityEngine;
using System;

using NetMQ;
using NetMQ.Sockets;

public class NetworkManager : MonoBehaviour {

    private bool isMaster = false;

    private NetMQSocket socket;

	// Use this for initialization
	void Start () {
        AsyncIO.ForceDotNet.Force();

	    foreach(var arg in Environment.GetCommandLineArgs()) {
            if(arg == "-master") {
                isMaster = true;
                break;
            }
        }

        if(isMaster) {
            socket = new PublisherSocket();
        }

        else {
            socket = new SubscriberSocket();
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
