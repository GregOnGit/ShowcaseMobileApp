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
                s_client.Connect( "gregoryonbusiness.ddns.net", 8888 );
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
        String l_strSend = "Hi";
        NetworkStream l_stream = s_client.GetStream();
        byte[] l_bytesToSend = System.Text.Encoding.ASCII.GetBytes( l_strSend );
        s_client.Client.Send( l_bytesToSend );

        Debug.Log( "Sent." );

        // Listen and wait for commands
        while( true )
        {
            // Checks if new data is available to be read from the network stream
            if( s_client.Available > 0 )
            {
                // Create an array large enough to hold the recieved data
                byte[] l_bytesRead = new byte[s_client.Available];

                // Read the data from the network stream
                s_client.GetStream().Read( l_bytesRead, 0, s_client.Available );

                // Convert the byte array to a string we can understand
                String l_str = System.Text.Encoding.ASCII.GetString( l_bytesRead );

                // For now, just write whatever it is to console
                Debug.Log( l_str );

                // Act upon the commands
                foreach( char l_cmd in l_str.ToCharArray() )
                {
                    // The play command
                    if( l_cmd == 'p' )
                    {
                        MainScript.s_playSoundNow = true;
                    }
                    else
                    {
                        Debug.Log( "I did not understand that" );
                    }
                }
            }

            Thread.Sleep( 500 );
        }
    }
}

public class MainScript : MonoBehaviour
{
    ServerConnect m_serverConnect;

    // The thread that will take care of connection and sending packets
    Thread m_thread;

    // The sound that will be played
    [SerializeField]
    public AudioClip m_audioClip;

    // When to play the sound
    public static bool s_playSoundNow = false;

    // Audio source
    [SerializeField]
    public AudioSource m_audioSource;

    // Start is called before the first frame update
    void Start()
    {
        m_audioSource.PlayOneShot( m_audioClip, 1.0f );

        ServerConnect.Init();

        // Start the connection process
        m_thread = new Thread( new ThreadStart( ServerConnect.AttemptConnect ) );
        m_thread.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if( s_playSoundNow )
        {
            m_audioSource.PlayOneShot( m_audioClip, 1.0f );
            s_playSoundNow = false;
        }
    }
}
