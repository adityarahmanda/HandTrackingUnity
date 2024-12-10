using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;

public class LidarDataUDPReceiver : MonoBehaviour
{
    public int Port = 2368;
    public Vector3[] CloudPoints;
    
    private UdpClient _udpClient;

    private void Start()
    {
        try
        {
            _udpClient = new UdpClient(Port);
            Debug.Log($"Listening for messages on port {Port}...");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to initialize UDP receiver: {ex.Message}");
        }
    }
    
    private void OnApplicationQuit()
    {
        _udpClient?.Close();
    }
    
    private void FixedUpdate()
    {
        if (_udpClient != null && _udpClient.Available > 0)
        {
            try
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, Port);
                byte[] receivedBytes = _udpClient.Receive(ref endPoint);
                CloudPoints = Deserialize(receivedBytes);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error receiving UDP message: {ex.Message}");
            }
        }
    }
    
    private Vector3[] Deserialize(byte[] bytes)
    {
        // Calculate the number of Vector3 objects in the byte array (each Vector3 is 12 bytes)
        int numVectors = bytes.Length / 12;
        Vector3[] vectorArray = new Vector3[numVectors];

        // Loop through the byte array and reconstruct each Vector3
        for (int i = 0; i < numVectors; i++)
        {
            // Extract 12 bytes for each Vector3
            float x = BitConverter.ToSingle(bytes, i * 12);
            float y = BitConverter.ToSingle(bytes, i * 12 + 4);
            float z = BitConverter.ToSingle(bytes, i * 12 + 8);

            // Assign the Vector3 to the array
            vectorArray[i] = new Vector3(x, y, z);
        }

        return vectorArray;
    }
}

