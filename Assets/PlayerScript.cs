using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public PetriNet pn;
    public MapGenerator mg;
    public Vector2 cellPosition;
    public bool createdNet = false;
    public bool run = false;
    

    void Start()
    {
        if (pn == null)
            pn = gameObject.AddComponent<PetriNet>();
        if (mg == null)
            mg = GameObject.Find("Map").GetComponent<MapGenerator>();

        CreatePetriNet();
    }

    public void CreatePetriNet()
    {
        DestroyImmediate(pn);
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
        pn.CreateSlot("Input Deslocar Norte"); // 9
        pn.CreateSlot("Input Deslocar Leste"); // 10
        pn.CreateSlot("Input Deslocar Sul"); // 11
        pn.CreateSlot("Input Deslocar Oeste"); // 12
        pn.CreateSlot("Combustível"); // 13
        pn.CreateSlot("Input Carregar Combustível"); // 14
        pn.CreateSlot("Quadrante Combustível"); // 15
        pn.CreateTransition("Deslocar Norte"); // T4
        pn.CreateTransition("Deslocar Leste"); // T5
        pn.CreateTransition("Deslocar Sul"); // T6
        pn.CreateTransition("Deslocar Oeste"); // T7
        pn.CreateTransition("Carregar Combustível"); // T8

        pn.CreateConnectionST(8,4,1,PetriNet.ConnectionType.Inhibitor);

        pn.CreateConnectionST(9,4);
        pn.CreateConnectionST(10,5);
        pn.CreateConnectionST(11,6);
        pn.CreateConnectionST(12,7);

        pn.CreateConnectionST(13,4);
        pn.CreateConnectionST(13,5);
        pn.CreateConnectionST(13,6);
        pn.CreateConnectionST(13,7);

        pn.CreateConnectionST(14,8);
        pn.CreateConnectionST(15,8);

        pn.CreateConnectionTS(13,8,20);

        pn.ListsToArrays();

        pn.AddTokensToSlot(1,100);
        pn.AddTokensToSlot(6,20);
        pn.AddTokensToSlot(13,50);

        pn.transitionsArray[4].SetCallback(MoveN);
        pn.transitionsArray[5].SetCallback(MoveL);
        pn.transitionsArray[6].SetCallback(MoveS);
        pn.transitionsArray[7].SetCallback(MoveO);
        pn.transitionsArray[1].SetCallback(Die);

        Debug.Log("Created Net");
    }

    void Update()
    {
        MoveInput();

        
    }

    void FixedUpdate()
    {
       if(run) pn.RunPetri();
    }

    private void MoveInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            pn.SetTokensOnSlot(9,1);
            if (mg.cells[(int)cellPosition.x,(int)cellPosition.y+1].wall)
            {
                pn.SetTokensOnSlot(8,1);
            }
            Debug.Log("Input N");
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            pn.SetTokensOnSlot(9,0);
            pn.SetTokensOnSlot(8,0);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            pn.SetTokensOnSlot(10,1);
            if (mg.cells[(int)cellPosition.x+1,(int)cellPosition.y].wall)
            {
                pn.SetTokensOnSlot(8,1);
            }
            Debug.Log("Input L");
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            pn.SetTokensOnSlot(10,0);
            pn.SetTokensOnSlot(8,0);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            pn.SetTokensOnSlot(11,1);
            if (mg.cells[(int)cellPosition.x,(int)cellPosition.y-1].wall)
            {
                pn.SetTokensOnSlot(8,1);
            }
            Debug.Log("Input S");
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            pn.SetTokensOnSlot(11,0);
            pn.SetTokensOnSlot(8,0);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            pn.SetTokensOnSlot(12,1);
            if (mg.cells[(int)cellPosition.x-1,(int)cellPosition.y].wall)
            {
                pn.SetTokensOnSlot(8,1);
            }
            Debug.Log("Input O");
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            pn.SetTokensOnSlot(12,0);
            pn.SetTokensOnSlot(8,0);
        }
    }

    public void TestRun()
    {
        pn.RunPetri();
    }
    
    public void MoveN()
    {
        Debug.Log("Moved Rover N");

        //Debug.Log("Agent before: " + mg.cells[(int)cellPosition.x,(int)cellPosition.y].go.transform.name);
        //Debug.Log("Cells: " + mg.cells);
        //Debug.Log("New Cell: " + mg.cells[(int)cellPosition.x,(int)cellPosition.y - 1]);
        //Debug.Log("Agent: " + mg.cells[(int)cellPosition.x,(int)cellPosition.y - 1].agent);
        //Debug.Log("gameObject: " + gameObject);

        
        mg.cells[(int)cellPosition.x,(int)cellPosition.y].agent = null;
        cellPosition = new Vector2(cellPosition.x,cellPosition.y+1);

        mg.cells[(int)cellPosition.x,(int)cellPosition.y].agent = gameObject;
        Debug.Log("Agent after: " + mg.cells[(int)cellPosition.x,(int)cellPosition.y-1].go.transform.name);
        transform.SetParent(mg.cells[(int)cellPosition.x,(int)cellPosition.y].go.transform,false);

        //transform.localPosition = new Vector3(0,0,-.5f);
    }
    public void MoveL()
    {
        Debug.Log("Moved Rover L");
    }
    public void MoveS()
    {
        Debug.Log("Moved Rover S");
    }
    public void MoveO()
    {
        Debug.Log("Moved Rover O");
    }

    void Die()
    {
        Debug.Log("Hover Died");
    }
}
