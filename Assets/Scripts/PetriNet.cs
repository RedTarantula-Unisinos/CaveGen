using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetriNet : MonoBehaviour
{

    public enum ConnectionType
    {
        Normal,
        Inhibitor, // If there's a token, the transition's not available
        Reset // Clears all tokens when activated
    }

    [Serializable]
    public struct PetriConnection
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

    [Serializable]
    public struct PetriSlot
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

    [Serializable]
    public struct PetriTransition
    {
        public int id;
        public string name;
        public List<PetriConnection> inputs;
        public List<PetriConnection> outputs;
        [Serializable]
        public delegate void CallBack();
        [SerializeField]
        public CallBack cb;

        public PetriTransition(int transId,string transName,CallBack callback = null)
        {
            id = transId;
            name = transName;
            inputs = new List<PetriConnection>();
            outputs = new List<PetriConnection>();
            cb = callback;
        }

        public void ActivateCallBack()
        {
            //Debug.Log("Callback for " + name + ": " + cb);
            if(cb != null)
            cb();
            else
            {
                //Debug.Log("No set callback");
            }
        }

        public void SetCallback(CallBack callback)
        {
            //Debug.Log("Setting callback for " + name + ": " + cb);
            cb = callback;
        }

    }


    [SerializeField] [HideInInspector]
    public List<PetriSlot> slotsList = new List<PetriSlot>();
    [SerializeField] [HideInInspector]
    public List<PetriConnection> connectionsList = new List<PetriConnection>();
    [SerializeField] [HideInInspector]
    public List<PetriTransition> transitionsList = new List<PetriTransition>();

    [SerializeField]
    public PetriSlot[] slotsArray;
    [SerializeField]
    public PetriConnection[] connectionsArray;
    [SerializeField]
    public PetriTransition[] transitionsArray;

    public bool debugging = false;

    public void ListsToArrays()
    {
        slotsArray = slotsList.ToArray();
        connectionsArray = connectionsList.ToArray();
        transitionsArray = transitionsList.ToArray();
    }

    public void RunPetri()
    {
        foreach (PetriTransition transition in transitionsArray) // For each transition that there is
        {
            bool enabled = true;
            foreach (PetriConnection inputConnection in transition.inputs) // Check for input connections 
            {
                PetriSlot slot = slotsArray[inputConnection.s];
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

                    PetriSlot slot = slotsArray[inputConnection.s]; // Reach for their slot
                    if (inputConnection.type != ConnectionType.Inhibitor)
                    {
                        RemoveTokensFromSlot(inputConnection.s,inputConnection.weight); // In case it's a normal or reset connection, remove some tokens from the slot
                    }
                    else if (inputConnection.type == ConnectionType.Reset)
                    {
                        RemoveTokensFromSlot(inputConnection.s,slotsArray[inputConnection.s].tokens); // If it's a reset connection, remove all tokens
                    }
                }

                foreach (PetriConnection outputConnection in transition.outputs) // Get each output
                {
                    PetriSlot slot = slotsArray[outputConnection.s]; // Reach for their slot
                    AddTokensToSlot(outputConnection.s,outputConnection.weight); // Give tokens to it
                }

                transition.ActivateCallBack();
            }
            if (debugging)
                Debug.Log(transition.name + " enabled: " + enabled);
        }
    }


    public void AddTokensToSlot(int slotID,int tokensAmount)
    {
        slotsArray[slotID].AddTokens(tokensAmount);
        if (debugging)
            Debug.Log("Adding " + tokensAmount + " tokens to slot " + slotID);
    }
    public void SetTokensOnSlot(int slotID,int tokensAmount)
    {
        slotsArray[slotID].tokens = tokensAmount;
        if (debugging)
            Debug.Log("Setting " + tokensAmount + " tokens on slot " + slotID);
    }
    public void RemoveTokensFromSlot(int slotID,int tokensAmount)
    {
        slotsArray[slotID].RemoveTokens(tokensAmount);
        if (debugging)
            Debug.Log("Removing " + tokensAmount + " tokens from slot " + slotID);
    }

     public void CreateConnectionST(int slot, int transition, int w = 1, ConnectionType ctype = ConnectionType.Normal)
        {
            int slotID = slot;
            int transID = transition;
            int weight = w;
            ConnectionType type = ctype;
            

            PetriConnection c = new PetriConnection(connectionsList.ToArray().Length,slotID,transID,false,weight,type);
            transitionsList[transID].inputs.Add(c);
            slotsList[slotID].outputs.Add(c);
            connectionsList.Add(c);

            
            PetriTransition trans = transitionsList[transID];
            PetriSlot petriSlot = slotsList[slotID];
            string l = "Created a connection from the slot [" + petriSlot.name + "(" + petriSlot.id + ")" + "] to the transition [" + trans.name + "(" + trans.id + ")]" + "Id: " + c.id;
        }
        public void CreateConnectionTS(int slot, int transition, int w = 1)
        {
            int slotID = slot;
            int transID = transition;
            int weight = w;
            ConnectionType type = ConnectionType.Normal;

            PetriConnection c = new PetriConnection(connectionsList.ToArray().Length,slotID,transID,true,weight,type);
            transitionsList[transID].outputs.Add(c);
            slotsList[slotID].inputs.Add(c);
            connectionsList.Add(c);

            PetriTransition trans = transitionsList[transID];
            PetriSlot pslot = slotsList[slotID];
            string l = "Created a connection from the transition [" + trans.name + "(" + trans.id + ")" + "] to the slot [" + pslot.name + "(" + pslot.id + ")]" + "Id: " + c.id;
        }
        public void CreateSlot(string slotName)
        {
            string name = slotName;


            PetriSlot s = new PetriSlot(slotsList.ToArray().Length,name);
            slotsList.Add(s);


            int id = slotsList.ToArray().Length - 1;


            
        }
        public void CreateTransition(string transName)
        {
            string name = transName;
            
            PetriTransition t = new PetriTransition(transitionsList.ToArray().Length,name);
            transitionsList.Add(t);
            
        }
}


