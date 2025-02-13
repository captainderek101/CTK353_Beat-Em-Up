using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractPrompt : MonoBehaviour
{
    [SerializeField] private GameObject uiObject;
    private PlayerInputs.PlayerActions playerActions;
    public UnityEvent interactEvent;

    private void Start()
    {
        //playerActions = PlayerInputController.Instance.inputActions.Player;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            uiObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            uiObject.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(PlayerInputController.Instance.player.actions.FindAction("Interact").inProgress && other.tag == "Player" && interactEvent != null)
        {
            interactEvent.Invoke();
        }
    }
}
