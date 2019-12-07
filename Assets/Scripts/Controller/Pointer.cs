using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pointer : MonoBehaviour
{
    public float m_Distance = 100.0f;
    public LineRenderer m_LineRenderer = null;
    public LayerMask m_EverythingMask = 0;
    public LayerMask m_InteractableMask = 0;
    public UnityAction<Vector3, GameObject> OnPointerUpdate = null;

    private Transform m_CurrentOrigin = null;
    private GameObject m_CurrentObject = null;

    public GameObject Player;
    public GameObject[] line;
    Vector3 lineEndPosition;
    public bool isPressed;

    /// <summary>
    /// 
    /// </summary>
    public OVRScreenFade fade;

    //For the button Two/Back
    public bool controlMode;

    //GamepadMovement
    private Vector3 inputVector;
    private Vector3 movementVector;
    private float movementSpeed = 15.0f;
    public Camera c;

    private void Awake()
    {
        PlayerEvents.OnControllerSource += UpdateOrigin;
        PlayerEvents.OnTouchpadDown += ProcessTouchpadDown;
        PlayerEvents.OnTouchpadUp += ProcessTouchpadUp;
        PlayerEvents.OnTouchpadGet += ProcessTouchpadGet;
        PlayerEvents.OnIndexTriggerDown += ProcessIndexTriggerDown;
        PlayerEvents.OnTwoGet += ProcessTwoGet;
    }

    public void Start()
    {
        SetLineColor();
        Player = GameObject.FindWithTag("Player");

        fade = GameObject.Find("CenterEyeAnchor").gameObject.GetComponent<OVRScreenFade>();
        line = GameObject.FindGameObjectsWithTag("Line");
        controlMode = false;
        isPressed = false;
    }

    private void OnDestroy()
    {
        PlayerEvents.OnControllerSource -= UpdateOrigin;
        PlayerEvents.OnTouchpadDown -= ProcessTouchpadDown;

        PlayerEvents.OnTouchpadUp -= ProcessTouchpadUp;
        //
        PlayerEvents.OnIndexTriggerDown -= ProcessIndexTriggerDown;
        PlayerEvents.OnTwoGet -= ProcessTwoGet;
    }

    private void Update()
    {
        if(controlMode == false)
        {            
            Vector3 hitPoint = UpdateLine();

            m_CurrentObject = UpdatePointerStatus();

            OnPointerUpdate?.Invoke(hitPoint, m_CurrentObject);
        }
        else
        {           
            gamepadMovement();
            //gamepadMovementAccel();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private Vector3 UpdateLine()
    {
        //Create ray
        RaycastHit hit = CreateRaycast(m_EverythingMask);

        //Default end
        //Vector3 endPosition = m_CurrentOrigin.position + (m_CurrentOrigin.forward * m_Distance);
        if (isPressed == false)
        {
            lineEndPosition = m_CurrentOrigin.position + (m_CurrentOrigin.forward * m_Distance);
        }

        //Check hit
        if (hit.collider != null)
        {
            //endPosition = hit.point;
            lineEndPosition = hit.point;
        }

        //Set position
        m_LineRenderer.SetPosition(0, m_CurrentOrigin.position);
        //m_LineRenderer.SetPosition(1, endPosition);
        m_LineRenderer.SetPosition(1, lineEndPosition);

        //return endPosition;
        return lineEndPosition;
    }

    private void UpdateOrigin(OVRInput.Controller controller, GameObject controllerObject)
    {
        //Set origin of pointer
        m_CurrentOrigin = controllerObject.transform;

        //Is the laser visible?
        if (controller == OVRInput.Controller.Touchpad)
        {
            m_LineRenderer.enabled = false;
        }
        else
        {
            m_LineRenderer.enabled = true;
        }
    }

    private GameObject UpdatePointerStatus()
    {
        //Create ray
        RaycastHit hit = CreateRaycast(m_InteractableMask);

        //Check hit
        if (hit.collider)
        {
            return hit.collider.gameObject;
        }

        //return
        return null;
    }

    private RaycastHit CreateRaycast(int layer)
    {
        RaycastHit hit;
        Ray ray = new Ray(m_CurrentOrigin.position, m_CurrentOrigin.forward);
        //
        Physics.Raycast(ray, out hit, m_Distance * 10, layer);
        //

        return hit;
    }

    private void SetLineColor()
    {
        if (!m_LineRenderer)
        {
            return;
        }

        Color endColor = Color.white;
        endColor.a = 0.0f;

        m_LineRenderer.endColor = endColor;
    }

    private void ProcessTouchpadDown()
    {
        /*
        //If I don't point any object just teleport 10 Units forward
        if (!m_CurrentObject)
        {
            //Fade in every time you click
            fade.fadeOnStart = true;

            Vector3 endPosition = m_CurrentOrigin.position + (m_CurrentOrigin.forward * m_Distance);
            Player.transform.position = Vector3.MoveTowards(m_CurrentOrigin.position, endPosition, m_Distance);

            return;
        }

        //Interactable interactable = m_CurrentObject.GetComponent<Interactable>();
        //interactable.Pressed();

        //Fade in every time you click
        fade.fadeOnStart = true;

        //If I click on one object, then teleport to this object
        Vector3 endPosition2 = m_CurrentObject.transform.position - (m_CurrentOrigin.forward * 3);
        Player.transform.position = Vector3.MoveTowards(m_CurrentOrigin.position, endPosition2, m_Distance * 10);
        */


        isPressed = true;

        //lineEndPosition = m_CurrentOrigin.forward * m_Distance;
    }

    private void ProcessTouchpadGet()
    {
        lineEndPosition += m_CurrentOrigin.forward * 4;
    }

    private void ProcessTouchpadUp()
    {
        isPressed = false;

        //Fade in every time you click
        fade.fadeOnStart = true;

        //If I don't point any object just teleport 10 Units forward
        if (!m_CurrentObject)
        {
            //Vector3 endPosition = m_CurrentOrigin.position + (m_CurrentOrigin.forward * m_Distance);
            Player.transform.position = Vector3.MoveTowards(m_CurrentOrigin.position, lineEndPosition, Mathf.Infinity);

            return;
        }

        //Interactable interactable = m_CurrentObject.GetComponent<Interactable>();
        //interactable.Pressed();

        //If I click on one object, then teleport to this object
        Vector3 endPosition2 = m_CurrentObject.transform.position - (m_CurrentOrigin.forward * 3);
        Player.transform.position = Vector3.MoveTowards(m_CurrentOrigin.position, endPosition2, Mathf.Infinity);
    }

    private void ProcessIndexTriggerDown()
    {
        //Fade in every time you click
        fade.fadeOnStart = true;

        //If I don't point any object just teleport 10 Units forward
        if (!m_CurrentObject)
        {
            Vector3 endPosition = m_CurrentOrigin.position + (m_CurrentOrigin.forward * m_Distance);
            Player.transform.position = Vector3.MoveTowards(m_CurrentOrigin.position, endPosition, Mathf.Infinity);

            return;
        }

        //Interactable interactable = m_CurrentObject.GetComponent<Interactable>();
        //interactable.Pressed();

        //If I click on one object, then teleport to this object
        Vector3 endPosition2 = m_CurrentObject.transform.position - (m_CurrentOrigin.forward * 3);
        Player.transform.position = Vector3.MoveTowards(m_CurrentOrigin.position, endPosition2, m_Distance * 10);       
    }

    private void ProcessTwoGet()
    {
        //Changing the mode 
        if(controlMode == false)
        {
            controlMode = true;
            foreach (GameObject l in line)
            {
                l.SetActive(false);
            }
        }
        else
        {
            controlMode = false;
            foreach (GameObject l in line)
            {
                l.SetActive(true);
            }
        }
    }

    private void gamepadMovement()
    {
        OVRInput.Update();
        movementVector = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
        // movementVector = player.transform.InverseTransformVector(movementVector);
        movementVector.x = movementVector.x * movementSpeed;
        movementVector.z = movementVector.y * movementSpeed;
        movementVector.y = 0;

        //player.transform.position += (player.transform.forward * Time.deltaTime);
        Player.transform.rotation = c.transform.localRotation;
        Player.transform.Translate(movementVector * Time.deltaTime);
    }

    private void gamepadMovementAccel()
    {
        OVRInput.Update();
        inputVector = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
        inputVector.z = inputVector.y;
        inputVector.y = 0;
        if ((inputVector.x > 0.5 || inputVector.x < -0.5 && (inputVector.y > -0.5 && inputVector.z < 0.5)) || (inputVector.z > 0.5 || (inputVector.z < -0.5 && (inputVector.x > -0.5 && inputVector.x < 0.5))))
            movementVector += inputVector;
        if (movementVector.x > 5)
            movementVector.x = 5;
        if (movementVector.x < -5)
            movementVector.x = -5;
        if (movementVector.z > 5)
            movementVector.z = 5;
        if (movementVector.z < -5)
            movementVector.z = -5;
        //movementVector += inputVector
        else
        {
            if (movementVector.x < 1 && movementVector.x > -1)
            {
                if (movementVector.x > 0)
                    movementVector.x -= 0.2f;
                if (movementVector.x < 0)
                    movementVector.x += 0.2f;
            }
            if (movementVector.z < 1 && movementVector.z > -1)
                if (movementVector.z > 0)
                    movementVector.z -= 0.2f;
            if (movementVector.z < 0)
                movementVector.z += 0.2f;
        }
        Player.transform.rotation = c.transform.localRotation;
        Player.transform.Translate(movementVector * movementSpeed * Time.deltaTime);
    }
}
