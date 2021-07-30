using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System;

// class to manage connection
public class ServerConnect
{
    public static bool s_isConnected = false;

    private static TcpClient s_client = null;

    public static void Init()
    {
        s_client = new TcpClient();
    }

    // Run in seperate thread so it is not blocking
    public static void AttemptConnect()
    {
        Debug.Log( "Attempting to connect..." );
        
        while( true )
        {
            try
            {
                // Just connecting to localhost for the moment for testing
                s_client.Connect( "192.168.1.112", 8888 );
            }
            catch (SocketException)
            {
                Debug.Log( "Connection no work...");

                Thread.Sleep( 1000 );

                continue;
            }

            Debug.Log( "Connected. Sending Hi" );

            break;
        }

        // Once connected send "I have connected"
        String l_str = "Hi";
        NetworkStream l_stream = s_client.GetStream();
        byte[] l_bytesToSend = System.Text.Encoding.ASCII.GetBytes( l_str );
        s_client.Client.Send( l_bytesToSend );

        Debug.Log( "Sent." );
    }
}

public class MainScript : MonoBehaviour
{
    ServerConnect m_serverConnect;

    // The thread that will take care of connection and sending packets
    Thread m_thread;

    // Start is called before the first frame update
    void Start()
    {
        ServerConnect.Init();

        // Start the connection process
        m_thread = new Thread( new ThreadStart( ServerConnect.AttemptConnect ) );
        m_thread.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
