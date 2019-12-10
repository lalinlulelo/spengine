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
    public GameObject reticle;

    /// <summary>
    /// 
    /// </summary>
    public OVRScreenFade fade;

    //For the button Two/Back
    private enum ControlMode { Teleport, Fly, Accelerate };
    private ControlMode controlMode = ControlMode.Teleport;

    //GamepadMovement
    private Vector3 inputVector;
    private Vector3 movementVector;
    private Vector3 accelDirVector;
    private float AccelSpeed { get; set; } = 0f;
    private const float AccelRate = 5f;
    private float MovementSpeed { get; } = 10.0f;
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
        controlMode = ControlMode.Teleport;
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
        switch (controlMode)
        {
            case ControlMode.Teleport:
                TeleportMovement();
                break;
            case ControlMode.Fly:         
                GamepadMovement();
                break;
            case ControlMode.Accelerate:
                GamepadMovementAccel();
                break;
        }
    }

    private void TeleportMovement()
    {
        Vector3 hitPoint = UpdateLine();
        m_CurrentObject = UpdatePointerStatus();
        OnPointerUpdate?.Invoke(hitPoint, m_CurrentObject);
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
        if (controller == OVRInput.Controller.Touchpad || controlMode != ControlMode.Teleport)
        {
            m_LineRenderer.enabled = false;
            reticle.SetActive(false);
        }
        else
        {
            m_LineRenderer.enabled = true;
            reticle.SetActive(true);
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
        if (controlMode == ControlMode.Teleport)
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
    }

    private void ProcessIndexTriggerDown()
    {
        if (controlMode == ControlMode.Teleport)
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
            Player.transform.position = m_CurrentObject.transform.position;
            
        } else if (controlMode == ControlMode.Accelerate)
        {
            movementVector = new Vector3(0, 0, 0);
            AccelSpeed = 0;
            accelDirVector = new Vector3(0, 0, 0);
        } else if (controlMode == ControlMode.Fly)
        {
            movementVector = new Vector3(0, 0, 0);
            AccelSpeed = 0;
            accelDirVector = new Vector3(0, 0, 0);
        }   
    }

    private void ProcessTwoGet()
    {
        //Changing the mode 
        if(controlMode == ControlMode.Teleport)
        {
            controlMode = ControlMode.Fly;
            movementVector = new Vector3(0, 0, 0);
            AccelSpeed = 0;
            accelDirVector = new Vector3(0, 0, 0);
            reticle.SetActive(false);
        } else if (controlMode == ControlMode.Fly)
        {
            controlMode = ControlMode.Accelerate;
            movementVector = new Vector3(0, 0, 0);
            AccelSpeed = 0;
            accelDirVector = new Vector3(0, 0, 0);
        }
        else if (controlMode == ControlMode.Accelerate)
        {
            controlMode = ControlMode.Teleport;
            
        }
    }

    private void GamepadMovement()
    {
        //OVRInput.Update();
        Vector2 moveVector = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
        if (moveVector.Equals(new Vector2(0, 0)))
        {
            return;
        }
        Vector3 dir = Vector3.Scale(Camera.main.transform.forward, new Vector3(moveVector.normalized.x, moveVector.normalized.y, 1));
        Player.transform.position += dir * Time.deltaTime * MovementSpeed;
    }

    private void GamepadMovementAccel()
    {
        //OVRInput.Update();
        Vector2 moveVector = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
        if (!moveVector.Equals(new Vector2(0, 0)))
        {
            accelDirVector = Vector3.Scale(Camera.main.transform.forward, new Vector3(moveVector.normalized.x, moveVector.normalized.y, 1));
            AccelSpeed += AccelRate * Time.deltaTime;
        }
        Player.transform.position += accelDirVector * Time.deltaTime * AccelSpeed;    
    }
}
