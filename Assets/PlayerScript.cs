using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public PetriNet pn;
    public bool createdNet = false;

    void Start()
    {
        if (pn == null)
            pn = gameObject.AddComponent<PetriNet>();
        if(!createdNet)
        CreatePetriNet();
    }

    public void CreatePetriNet()
    {
        if (createdNet)
        {
            DestroyImmediate(pn);
        }

        if (pn == null)
            pn = gameObject.AddComponent<PetriNet>();


        createdNet = true;

        pn.CreateSlot("Tiro Recebido"); // 0
        pn.CreateSlot("Saúde"); // 1
        pn.CreateTransition("Atingido"); // T0
        pn.CreateTransition("Game Over"); // T1
        pn.CreateConnectionST(0,0);
        pn.CreateConnectionST(1,0);
        pn.CreateConnectionST(1,1,1,PetriNet.ConnectionType.Inhibitor);


        pn.CreateSlot("Quadrante Munição"); // 2
        pn.CreateSlot("Input Carregar Munição"); // 3
        pn.CreateSlot("Robo Vizinhança"); // 4
        pn.CreateSlot("Input Atacar"); // 5
        pn.CreateSlot("Munição"); // 6
        pn.CreateSlot("Tiro Enviado"); // 7
        pn.CreateTransition("Carregar Munição"); // T2
        pn.CreateTransition("Atacar Robo"); // T3
        pn.CreateConnectionST(2,2);
        pn.CreateConnectionST(3,2);
        pn.CreateConnectionST(4,3);
        pn.CreateConnectionST(5,3);
        pn.CreateConnectionST(6,3);
        pn.CreateConnectionTS(6,2,10);
        pn.CreateConnectionTS(7,3);

        pn.CreateSlot("Quadrante Colisão"); // 8
        pn.CreateSlot("Input Deslocar"); // 9
        pn.CreateSlot("Combustível"); // 10
        pn.CreateSlot("Input Carregar Combustível"); // 11
        pn.CreateSlot("Quadrante Combustível"); // 12
        pn.CreateTransition("Deslocar"); // T4
        pn.CreateTransition("Carregar Combustível"); // T5
        pn.CreateConnectionST(8,4,1,PetriNet.ConnectionType.Inhibitor);
        pn.CreateConnectionST(9,4);
        pn.CreateConnectionST(10,4);
        pn.CreateConnectionST(11,5);
        pn.CreateConnectionST(12,5);
        pn.CreateConnectionTS(10,5,20);

        pn.AddTokensToSlot(1,100);
        pn.AddTokensToSlot(6,20);
        pn.AddTokensToSlot(10,50);

        pn.transitionsArray[4].SetCallback(Move);
        pn.transitionsArray[1].SetCallback(Die);

        Debug.Log("Created Net");
    }

    public void TestRun()
    {
        pn.RunPetri();
    }

    void Move()
    {
        Debug.Log("Moved Rover");
    }

    void Die()
    {
        Debug.Log("Hover Died");
    }
}
