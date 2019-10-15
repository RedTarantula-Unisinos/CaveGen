using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetriNet : MonoBehaviour
{
    private List<PetriSlot> slotsList;
    private List<PetriConnection> connectionsList;
    private List<PetriTransition> transitionsList;

    void Start() // Constructor
    {
        slotsList = new List<PetriSlot>();
        connectionsList = new List<PetriConnection>();
        transitionsList = new List<PetriTransition>();
    }

    public void RunPetri()
    {
        foreach (PetriTransition transition in transitionsList) // For each transition that there is
        {
            bool enabled = true;
            foreach (PetriConnection inputConnection in transition.inputs) // Check for input connections 
            {
                PetriSlot slot = slotsList[inputConnection.s];
                if ((slot.tokens < inputConnection.weight && inputConnection.type != ConnectionType.Inhibitor) || (slot.tokens > 0 && inputConnection.type == ConnectionType.Inhibitor))
                {
                    enabled = false; // If conditions are satisfied, enable the transition
                    break;
                }
            }

            if (enabled) // If the transition's enabled
            {
                foreach (PetriConnection inputConnection in transition.inputs) // Get each input
                {

                    PetriSlot slot = slotsList[inputConnection.s]; // Reach for their slot
                    if (inputConnection.type != ConnectionType.Inhibitor)
                    {
                        RemoveTokensFromSlot(inputConnection.s,inputConnection.weight); // In case it's a normal or reset connection, remove some tokens from the slot
                    }
                    else if (inputConnection.type == ConnectionType.Reset)
                    {
                        RemoveTokensFromSlot(inputConnection.s,slotsList[inputConnection.s].tokens); // If it's a reset connection, remove all tokens
                    }
                }

                foreach (PetriConnection outputConnection in transition.outputs) // Get each output
                {
                    PetriSlot slot = slotsList[outputConnection.s]; // Reach for their slot
                    AddTokensToSlot(outputConnection.s,outputConnection.weight); // Give tokens to it
                }
            }
        }
    }


    public void AddTokensToSlot(int slotID,int tokensAmount)
    {
        slotsList[slotID].AddTokens(tokensAmount);
    }
    public void RemoveTokensFromSlot(int slotID,int tokensAmount)
    {
        slotsList[slotID].RemoveTokens(tokensAmount);
    }
}


public enum ConnectionType
{
    Normal,
    Inhibitor, // If there's a token, the transition's not available
    Reset // Clears all tokens when activated
}

class PetriConnection
{

    public int id;
    public int s;
    public int t;
    public int weight;
    public ConnectionType type;
    public bool output;

    public PetriConnection(int slotID,int slot,int transition,bool isOutput,int connectionWeight = 1,ConnectionType connectionType = ConnectionType.Normal)
    {
        id = slotID;
        s = slot;
        t = transition;
        weight = connectionWeight;
        type = connectionType;
        output = isOutput;
    }

}

class PetriSlot
{
    public int id;
    public string name;
    public int tokens;

    public List<PetriConnection> inputs;
    public List<PetriConnection> outputs;

    public PetriSlot(int slotID,string slotName,int slotTokens = 0)
    {
        id = slotID;
        name = slotName;
        tokens = slotTokens;
        inputs = new List<PetriConnection>();
        outputs = new List<PetriConnection>();
    }

    public void AddTokens(int amount)
    {
        tokens += amount;
    }
    public void RemoveTokens(int amount)
    {
        tokens -= amount;
    }


}

class PetriTransition
{
    public int id;
    public string name;
    public List<PetriConnection> inputs;
    public List<PetriConnection> outputs;
    public delegate void CallBack();
    CallBack cb;

    public PetriTransition(int transId,string transName, CallBack callback = null)
    {
        id = transId;
        name = transName;
        inputs = new List<PetriConnection>();
        outputs = new List<PetriConnection>();
        cb = callback;
    }

    public void ActivateCallBack()
    {
        cb();
    }

}