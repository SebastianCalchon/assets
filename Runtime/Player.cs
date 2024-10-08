using System;
using UnityEngine;

public class Player
{
    private string Id;
    private string Name;

    private Rotation rotation;
    private Position position;

    private LeftHandPosition leftHandPosition;
    private LeftHandRotation leftHandRotation;
    private RightHandPosition rightHandPosition;
    private RightHandRotation rightHandRotation;

    // Nuevos campos para el cuerpo y la cabeza
    private GameObject bodyObject;
    private GameObject headObject;

    private GameObject leftHandObject;
    private GameObject rightHandObject;

    public float positionSmoothTime = 0.1f; // Tiempo de suavizado para la posición
    public float rotationSmoothTime = 0.1f; // Tiempo de suavizado para la rotación

    public string GetId()
    {
        return Id;
    }

    public void SetId(string id)
    {
        Id = id;
    }

    public GameObject GetBodyObject()
    {
        return bodyObject;
    }

    public GameObject GetHeadObject()
    {
        return headObject;
    }

    // Asignar objetos de cuerpo y cabeza
    public void SetBodyObject(GameObject body)
    {
        bodyObject = body;
    }

    public void SetHeadObject(GameObject head)
    {
        headObject = head;
    }

    public Vector3 GetPosition()
    {
        return bodyObject.transform.position;
    }

    public void SetPosition(Vector3 position)
    {

        Vector3 bodyPosition = position;
        bodyPosition.y -= 0.5f;
        bodyObject.transform.position = Vector3.Lerp(bodyObject.transform.position, bodyPosition, positionSmoothTime);


        Vector3 headPosition = position;
        headPosition.y -= 0.1f;
        headObject.transform.position = Vector3.Lerp(headObject.transform.position, headPosition, positionSmoothTime);
    }

    public void SetRotation(Vector3 bodyRotation, Vector3 headRotation)
    {

        Vector3 bodyRot = new Vector3(-90, bodyRotation.y, 0);
        bodyObject.transform.rotation = Quaternion.Slerp(bodyObject.transform.rotation, Quaternion.Euler(bodyRot), rotationSmoothTime);



        Vector3 headRot = new Vector3(headRotation.x - 90, headRotation.y, 0);
        headObject.transform.rotation = Quaternion.Slerp(headObject.transform.rotation, Quaternion.Euler(headRot), rotationSmoothTime);
    }


    public void SetLeftHandPosition(Vector3 Lefthandposition)
    {
        leftHandObject.transform.position = Vector3.Lerp(leftHandObject.transform.position, Lefthandposition, positionSmoothTime);
    }

    public void SetLeftHandObject(GameObject handleft)
    {
        leftHandObject = handleft;
    }

    public void SetLeftHandRotation(Vector3 Lefthandrotation)
    {
        leftHandObject.transform.rotation = Quaternion.Slerp(leftHandObject.transform.rotation, Quaternion.Euler(Lefthandrotation), rotationSmoothTime);
    }

    public void SetRightHandPosition(Vector3 Righthandposition)
    {
        rightHandObject.transform.position = Vector3.Lerp(rightHandObject.transform.position, Righthandposition, positionSmoothTime);
    }

    public void SetRightHandObject(GameObject handright)
    {
        rightHandObject = handright;
    }

    public void SetRightHandRotation(Vector3 Righthandrotation)
    {
        rightHandObject.transform.rotation = Quaternion.Slerp(rightHandObject.transform.rotation, Quaternion.Euler(Righthandrotation), rotationSmoothTime);
    }
    public GameObject GetLeftHandObject()
    {
        if (leftHandObject == null)
        {
            Debug.LogError("La mano izquierda no está inicializada.");
        }
        return leftHandObject;
    }


    public GameObject GetRightHandObject()
    {
        if (rightHandObject == null)
        {
            Debug.LogError("La mano derecha no está inicializada.");
        }
        return rightHandObject;
    }
}
