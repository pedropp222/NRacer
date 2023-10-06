using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GestorInputsJogo : MonoBehaviour, JogoInputs.IConduzirActions
{
    private JogoInputs inputs;

    void Awake()
    {
        if (inputs == null)
        {
            inputs = new JogoInputs();

            inputs.Conduzir.SetCallbacks(this);
        }
    }

    public event Action<float> GuiarEvent;
    public event Action<float> AccTravarEvent;
    public event Action<InputActionPhase> PausaEvent;

    public void AtivarConduzir()
    {
        inputs.GUI.Disable();
        inputs.Conduzir.Enable();      
    }

    public void AtivarGUI()
    {
        inputs.Conduzir.Disable();
        inputs.GUI.Enable();
    }

    public void OnACC_TRA(InputAction.CallbackContext context)
    {
        AccTravarEvent?.Invoke(context.ReadValue<float>());
    }

    public void OnGuiar(InputAction.CallbackContext context)
    {
        GuiarEvent?.Invoke(context.ReadValue<float>());
    }

    public void OnPausa(InputAction.CallbackContext context)
    {
        PausaEvent?.Invoke(context.phase);
    }
}
