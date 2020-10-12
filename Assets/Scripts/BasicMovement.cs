using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    [SerializeField]
    new Animator animation = default;

    void Update()
    {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f);

        animation.SetFloat("Horizontal", movement.x);
        animation.SetFloat("Vertical", movement.y);
        animation.SetFloat("Magnitude", movement.magnitude);

        //In order for the correct idle animation to play, the previous direction needs to be checked
        SetPreviousAnimationDirection(movement);

        transform.position += movement * Time.deltaTime;
    }

    //I am open to sugestions on how to avoid this mess!
    void SetPreviousAnimationDirection(Vector3 movement)
    {
        if (movement.magnitude > 0.001f)
        {
            if (movement.x > 0.001f && movement.y > 0.001f)
            {
                if(movement.x > movement.y)
                {
                    animation.SetBool("WasFront", false);
                    animation.SetBool("WasBack", false);
                    animation.SetBool("WasRight", true);
                    animation.SetBool("WasLeft", false);
                }
                else
                {
                    animation.SetBool("WasFront", true);
                    animation.SetBool("WasBack", false);
                    animation.SetBool("WasRight", false);
                    animation.SetBool("WasLeft", false);
                }
            }
            else if (movement.x < -0.001f && movement.y < -0.001f)
            {
                if (movement.x < movement.y)
                {
                    animation.SetBool("WasFront", false);
                    animation.SetBool("WasBack", false);
                    animation.SetBool("WasRight", false);
                    animation.SetBool("WasLeft", true);
                }
                else
                {
                    animation.SetBool("WasFront", false);
                    animation.SetBool("WasBack", true);
                    animation.SetBool("WasRight", false);
                    animation.SetBool("WasLeft", false);
                }
            }
            else if (movement.x > 0.001f && movement.y < -0.001f)
            {
                if (movement.x > -1*movement.y)
                {
                    animation.SetBool("WasFront", false);
                    animation.SetBool("WasBack", false);
                    animation.SetBool("WasRight", true);
                    animation.SetBool("WasLeft", false);
                }
                else
                {
                    animation.SetBool("WasFront", false);
                    animation.SetBool("WasBack", true);
                    animation.SetBool("WasRight", false);
                    animation.SetBool("WasLeft", false);
                }
            }
            else if (movement.x < -0.001f && movement.y > 0.001f)
            {
                if (-1*movement.x > movement.y)
                {
                    animation.SetBool("WasFront", false);
                    animation.SetBool("WasBack", false);
                    animation.SetBool("WasRight", false);
                    animation.SetBool("WasLeft", true);
                }
                else
                {
                    animation.SetBool("WasFront", true);
                    animation.SetBool("WasBack", false);
                    animation.SetBool("WasRight", false);
                    animation.SetBool("WasLeft", false);
                }
            }
            else if (movement.y > 0.001f)
            {
                animation.SetBool("WasFront", true);
                animation.SetBool("WasBack", false);
                animation.SetBool("WasRight", false);
                animation.SetBool("WasLeft", false);
            }
            else if (movement.y < -0.001f)
            {
                animation.SetBool("WasFront", false);
                animation.SetBool("WasBack", true);
                animation.SetBool("WasRight", false);
                animation.SetBool("WasLeft", false);
            }
            else if (movement.x > 0.001f)
            {
                animation.SetBool("WasFront", false);
                animation.SetBool("WasBack", false);
                animation.SetBool("WasRight", true);
                animation.SetBool("WasLeft", false);
            }
            else if (movement.x < -0.001f)
            {
                animation.SetBool("WasFront", false);
                animation.SetBool("WasBack", false);
                animation.SetBool("WasRight", false);
                animation.SetBool("WasLeft", true);
            }
        }
    }
}
