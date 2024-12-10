using UnityEngine;
using System;
using System.Net.Sockets;

[RequireComponent(typeof(Lidar2D))]
public class LidarDataUDPSender : MonoBehaviour
{
    public int Port = 2368;
    private Lidar2D _lidar2D;

    private void Start()
    {
        _lidar2D = GetComponent<Lidar2D>();
    }

    private void FixedUpdate()
    {
        using (UdpClient udpClient = new UdpClient())
        {
            byte[] data = ConvertVector3ArrayToBytes(_lidar2D.pointClouds);
            udpClient.Send(data, data.Length, "127.0.0.1", Port);
        }
    }

    private byte[] ConvertVector3ArrayToBytes(Vector3[] vectorArray)
    {
        int totalBytes = vectorArray.Length * 12;
        byte[] bytes = new byte[totalBytes];

        for (int i = 0; i < vectorArray.Length; i++)
        {
            Vector3 vector = vectorArray[i];

            BitConverter.GetBytes(vector.x).CopyTo(bytes, i * 12);
            BitConverter.GetBytes(vector.y).CopyTo(bytes, i * 12 + 4);
            BitConverter.GetBytes(vector.z).CopyTo(bytes, i * 12 + 8);
        }

        return bytes;
    }
}

