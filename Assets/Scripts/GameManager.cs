using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [Header("Game Stats")]

    public bool gameActive = true;
    public float rowOffset = 2;
    int piecesLimit = 6;

    public bool isPlayerTurn = true;

    [Header("Game Assets")]
    public Transform throwPivot;
    public GameObject piece;
    public List<GameObject> piecesList;
    public List<GameObject> usedPieces;

    public Sprite playerPieceTexture;
    public Sprite computerPieceTexture;

    public void AddPiece(RowManager row){

        if(!gameActive || row.piecesNumber >= piecesLimit)
            return;
        //gameActive = false;
        GameObject piece = GetPiece();
        piece.transform.parent = transform;
        piece.transform.position = row.startPosition.position + new Vector3(0,rowOffset * row.piecesNumber,0);
        piece.transform.SetAsFirstSibling();
        row.piecesNumber++;
        if(isPlayerTurn){
            piece.GetComponent<Image>().sprite = playerPieceTexture;
            piece.tag = "PlayerPiece";
        }else{
            piece.GetComponent<Image>().sprite = computerPieceTexture;
            piece.tag = "ComputerPiece";
        }
        usedPieces.Add(piece);
        isPlayerTurn = !isPlayerTurn;

        piece.transform.DOMoveY(throwPivot.position.y,1f).From();
    }

    GameObject GetPiece(){
        GameObject usedPiece = null;
        foreach(GameObject piece in piecesList){
            if(!piece.activeSelf)
                usedPiece = piece;
        }
        if(usedPiece == null){
            usedPiece = Instantiate(piece,transform.position,transform.rotation);
        }
        return usedPiece;
    }
}
