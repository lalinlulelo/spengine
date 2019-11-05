using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private string selectableTag = "Planet";

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                float step = Time.deltaTime; // calculate distance to move
                var selection = hit.transform;
                if (selection.CompareTag(selectableTag))
                {
                    transform.position = Vector3.MoveTowards(transform.position, selection.position, step);
                }
            }
        }       
    }
}
