using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO.Ports;
using System.Threading.Tasks;


public class serial : MonoBehaviour
{

  private Thread clientReceiveThread;
  private ConcurrentQueue<float[]> _queue;
  SerialPort stream;
  public string portName; //must set port name within unity editor to run

  float[] lastCommValue;

  // Start is called before the first frame update
  void Start()
  {
      _queue = new ConcurrentQueue<float[]>();
      lastCommValue  = new float[]{0.0f, 0.0f, 0.0f, 0.0f};
      startCommThread();
  }

	// On ending the Unity game, ensure the serial port is closed properly and end the thread (otherwise you may need to reboot computer)
  void OnApplicationQuit(){
      Debug.Log("ENDING");
      clientReceiveThread.Abort("TEST");
      stream.Close();
  }

	//get last dequeued reading for specified finger 
  public float getFinger(int finger){
    return lastCommValue[finger];
  }
  
	//
  void startCommThread(){
    clientReceiveThread = new Thread(new ThreadStart(ListenForData));
    clientReceiveThread.IsBackground = true;
    Debug.Log("starting handshake");
    clientReceiveThread.Start();
  }

  // Update is called once per frame
  void Update()
  {
    float[] outArray;
    float[] commArray;
    bool test = _queue.TryDequeue(out outArray);
    if (test)
    {
      commArray = outArray;
      lastCommValue = outArray;
    }
    else
    {
        return;
    }
  }

  private void ListenForData()
  {
    stream = new SerialPort(portName, 115200);
    stream.ReadTimeout = 50;
    stream.WriteTimeout = 50;
	
	//Start serial stream with arduino
    stream.Open();
    stream.Write("g\n\r");

	//On starting serial stream, arduino will reboot.
    for(int i = 0; i < 500000000; i++){
		//Wait for reboot
    }
    Debug.Log("DELAY OVER");

	//Send a character to arduino to initiate streaming of sensor readings.
    stream.Write("g\n\r");

    while (true)
    {
      if(!stream.IsOpen){
        return;
      }
      string serverMessage = stream.ReadLine();
      string[] values = serverMessage.Split(',');

      if(values.Length == 4){
        float[] farray = new float[]{0.0f, 0.0f, 0.0f, 0.0f};
        for(int i = 0; i < 4; i++){
          float value = 0;
          float.TryParse(values[i], out value);
          farray[i] = value;
        }
		
		//Increased queue size allows for smoother motion (in case of slow communication) at the cost of lag
        if(_queue.Count > 1){
          float[] temp;
          _queue.TryDequeue(out temp);
        }
        _queue.Enqueue(farray);
      }

    }
  }
}
