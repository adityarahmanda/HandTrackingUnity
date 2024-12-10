using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class UDPDataReceiver : MonoBehaviour
{
    public HandData HandData => _handData;
    
    public int Port = 2368;
    
    private UdpClient _udpClient;
    private HandData _handData;

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
                _handData = Deserialize(receivedBytes);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error receiving UDP message: {ex.Message}");
            }
        }
    }
    
    private HandData Deserialize(byte[] bytes)
    {
        var dataString = Encoding.ASCII.GetString(bytes);
        var dataStringSplit = dataString.Split(',');

        if (string.IsNullOrEmpty(dataString)) return default;

        var handPos = new Vector2(Convert.ToInt32(dataStringSplit[0]), Convert.ToInt32(dataStringSplit[1]));
        Debug.Log("Received Pos: " + handPos);

        return new HandData
        {
            Position = handPos,
            IsValid = true
        };
    }
}

[Serializable]
public struct HandData
{
    public Vector2 Position;
    public bool IsValid;
}

