﻿/*
*/
using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine.UI;

public class HapticArmController : MonoBehaviour
{
    // receiving Thread
    Thread receiveThread;
    // udpclient object
    TcpClient client;
    public string IP = "127.0.0.1"; // default local
    public int port = 4148;
    public Vector3 movementMultiplyer = new Vector3(0, 0, 0);

    string strReceiveUDP = "";
    Vector3 currentForce = new Vector3(0, 0, 0);
    Vector3 armPos = new Vector3(0, 0, 0);

    public void Start()
    {
        Application.runInBackground = true;
        init();
    }

    // init
    private void init()
    {
        // define port
        port = 4148;
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    // receive thread
    private void ReceiveData()
    {
        client = new TcpClient();
        client.Connect("127.0.0.1", port);
        if (client.Connected)
            print("Connected");
        else print("Not connected");
        NetworkStream stream = client.GetStream();
        int arrayPos = 0;
        byte[] data = new byte[4*sizeof(float)];
        while (true)
        {
            try
            {
                int read;
                while ((read = stream.Read(data, arrayPos, data.Length)) > 0)
                {
                    arrayPos += read;
                    if(read > 0)
                    {
                        arrayPos = 0;
                        float id = System.BitConverter.ToSingle(data, 0);
                        if(id == 0)
                        {
                            float x = System.BitConverter.ToSingle(data, sizeof(float));
                            float y = System.BitConverter.ToSingle(data, sizeof(float)*2);
                            float z = System.BitConverter.ToSingle(data, sizeof(float)*3);
                            armPos.x = x;
                            armPos.y = y;
                            armPos.z = z;
                        }
                        else
                        {
                            print("Button changed state : " + System.BitConverter.ToSingle(data, sizeof(float)) + " " + System.BitConverter.ToSingle(data, sizeof(float) * 2));
                        }
                    }
                }
                Thread.Sleep(50);
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }

    public void setForce(Vector3 force)
    {
        if(force != currentForce)
        {
            float[] floats = { force.x, force.y, force.z };
            if(client.Connected)
                client.GetStream().Write(floatsToBytes(floats), 0, 3 * sizeof(float));
            currentForce = force;
        }
    }

    public Vector3 GetArmPos()
    {
        return new Vector3(armPos.x * movementMultiplyer.x, armPos.y * movementMultiplyer.y, armPos.z * movementMultiplyer.z);
    }

    public string UDPGetPacket()
    {
        return strReceiveUDP;
    }

    void OnDisable()
    {
        if (receiveThread != null) receiveThread.Abort();
        client.Close();
    }

    byte[] floatsToBytes(float[] floats)
    {
        byte[] result = new byte[floats.Length * sizeof(float)];
        for(int i=0; i<floats.Length; i++)
        {
            byte[] bytes = BitConverter.GetBytes(floats[i]);
            for (int j = 0; j < bytes.Length; j++)
                result[i * sizeof(float) + j] = bytes[j];
        }
        return result;
    }
}

