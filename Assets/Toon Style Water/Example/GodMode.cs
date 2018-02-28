using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GodMode : MonoBehaviour {

    public static GodMode inst;
    public float speed = 10;
    public float sensitivity = 5;

    private void OnEnable() {
        inst = this;
    }

    private void Start()  {
        inst = this;        
    }

    // Update is called once per frame
    float rotationY;

    void Update () {

        Vector3 add = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) add += transform.forward;
        if (Input.GetKey(KeyCode.A)) add -= transform.right;
        if (Input.GetKey(KeyCode.S)) add -= transform.forward;
        if (Input.GetKey(KeyCode.D)) add += transform.right;
        add.y = 0;
        if (Input.GetKey(KeyCode.LeftShift)) add += Vector3.down;
        if (Input.GetKey(KeyCode.Space)) add += Vector3.up;


        transform.position += add*speed * Time.deltaTime;


        if (Input.GetMouseButton(1) )   {
            float rotationX =transform.localEulerAngles.y;
            rotationY = transform.localEulerAngles.x;



            rotationX += Input.GetAxis("Mouse X") * sensitivity;
            rotationY -= Input.GetAxis("Mouse Y") * sensitivity;
          
            transform.rotation = Quaternion.Euler(new Vector3(rotationY, rotationX, 0));

        }




    }
}
