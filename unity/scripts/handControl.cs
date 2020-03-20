using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class handControl : MonoBehaviour
{
    GameObject index1;
    GameObject index2;
    GameObject index3;
    GameObject middle1;
    GameObject middle2;
    GameObject middle3;
    GameObject ring1;
    GameObject ring2;
    GameObject ring3;
    GameObject pinky1;
    GameObject pinky2;
    GameObject pinky3;

    serial serialController;


    float maxjoint1;
    float maxjoint2;
    float maxjoint3;
    float minjoint1;
    float minjoint2;
    float minjoint3;



    // Start is called before the first frame update
    void Start()
    {	
	
		//Grab game objects for each "bone" in the hand skeleton
        index1 = GameObject.Find("b_l_index1").gameObject;
        index2 = GameObject.Find("b_l_index2").gameObject;
        index3 = GameObject.Find("b_l_index3").gameObject;

        middle1 = GameObject.Find("b_l_middle1").gameObject;
        middle2 = GameObject.Find("b_l_middle2").gameObject;
        middle3 = GameObject.Find("b_l_middle3").gameObject;

        ring1 = GameObject.Find("b_l_ring1").gameObject;
        ring2 = GameObject.Find("b_l_ring2").gameObject;
        ring3 = GameObject.Find("b_l_ring3").gameObject;

        pinky1 = GameObject.Find("b_l_pinky1").gameObject;
        pinky2 = GameObject.Find("b_l_pinky2").gameObject;
        pinky3 = GameObject.Find("b_l_pinky3").gameObject;
		
        serialController = GameObject.Find("commThread").GetComponent<serial>();

        minjoint1 = -5.0f;
        minjoint2 = 0.0f;
        minjoint3 = 8.0f;
        maxjoint1 = -90.0f;
        maxjoint2 = -110.0f;
        maxjoint3 = -60.0f;
    }

    // Update is called once per frame
    void Update()
    {

      for(int i = 0; i < 4; i++){
        float finger = serialController.getFinger(i);
        BendFinger(i+1, finger/100.0f);
      }




    }

	// Bend finger # to a certain percentage between fully curled and fully straight.
    void BendFinger(int finger, float percent){
        Transform tf1;
        Transform tf2;
        Transform tf3;

        float desired1 = percent * (maxjoint1 - minjoint1) + minjoint1;
        float desired2 = percent * (maxjoint2 - minjoint2) + minjoint2;
        float desired3 = percent * (maxjoint3 - minjoint3) + minjoint3;

        switch(finger){
            case 1:
                tf1 = index1.GetComponent<Transform>();
                tf2 = index2.GetComponent<Transform>();
                tf3 = index3.GetComponent<Transform>();
                break;
            case 2:
                tf1 = middle1.GetComponent<Transform>();
                tf2 = middle2.GetComponent<Transform>();
                tf3 = middle3.GetComponent<Transform>();

                break;
            case 3:
                tf1 = ring1.GetComponent<Transform>();
                tf2 = ring2.GetComponent<Transform>();
                tf3 = ring3.GetComponent<Transform>();

                break;
            case 4:
                tf1 = pinky1.GetComponent<Transform>();
                tf2 = pinky2.GetComponent<Transform>();
                tf3 = pinky3.GetComponent<Transform>();

                break;
            default:
                return;

        }

        float angle1 = normalizeJointAngle(tf1.localEulerAngles.z);
        float angle2 = normalizeJointAngle(tf2.localEulerAngles.z);
        float angle3 = normalizeJointAngle(tf3.localEulerAngles.z);

        tf1.Rotate(0, 0, desired1-angle1);
        tf2.Rotate(0, 0, desired2-angle2);
        tf3.Rotate(0, 0, desired3-angle3);

        return;
    }

	//Unity passes angle without wrapping.  This functions allows for wrapping of angles
    float normalizeJointAngle(float angle){
        angle %= 360;
        if(angle > 180){
            return angle - 360;
        }
        return angle;
    }
}
