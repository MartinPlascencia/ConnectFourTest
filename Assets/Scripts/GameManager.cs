using System.Collections;
using System.Collections.Generic;
using System;
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
    string lastPieceConnection;
    int playerRowIndex;

    [Header("Game Assets")]
    public Transform throwPivot;
    public GameObject piece;
    public List<RowManager> rowList;
    public List<GameObject> piecesList;
    public List<GameObject> usedPieces;

    List<GameObject> checkedPiecesList;

    public Sprite playerPieceTexture;
    public Sprite computerPieceTexture;

    [Header("Game Screens")]
    public Text turnText;
    public Text winScreenText;
    public RectTransform winScreen;
    public CanvasGroup fadeScreen;

    void Start(){
        SetTurn(true);
        InitializeRows();
    }

    void InitializeRows(){
        int index = 0;
        foreach(RowManager row in rowList){
            row.rowIndex = index;
            index++;
        }
    }

    public void AddPiece(RowManager row){

        if(isPlayerTurn){
            gameActive = false;
            playerRowIndex = row.rowIndex;
        }
        GameObject piece = GetPiece();
        piece.transform.parent = transform;
        piece.transform.position = row.startPosition.position + new Vector3(0,rowOffset * row.piecesList.Count,0);
        piece.transform.SetAsFirstSibling();
        row.piecesList.Add(piece);
        if(isPlayerTurn){
            piece.GetComponent<Image>().sprite = playerPieceTexture;
            piece.tag = "PlayerPiece";
        }else{
            piece.GetComponent<Image>().sprite = computerPieceTexture;
            piece.tag = "ComputerPiece";
        }
        usedPieces.Add(piece);
        
        piece.GetComponent<PieceBehaviour>().isChecked = false;
        piece.transform.DOMoveY(throwPivot.position.y,1f).From().OnComplete(CheckWin);
    }

    void SetTurn(bool active){
        isPlayerTurn = active;
        if(active){
            turnText.text = "Player's turn";
        }else{
            turnText.text = "Computer's turn";
        }
    }

    void CheckWin(){

        
        foreach(GameObject piece in usedPieces){

            foreach(GameObject gamePiece in usedPieces){
                SetPiece(gamePiece,false);
            }
            checkedPiecesList = new List<GameObject>();
            SetPiece(piece,true);
            if(checkedPiecesList.Count >= 4){
                StartCoroutine(WonGame());
                gameActive = false;
                break;
            }
        }

        if(checkedPiecesList.Count < 4){
            SetTurn(!isPlayerTurn);
            if(!isPlayerTurn)
                SetComputerTurn();
            else  
                gameActive = true;
        }
    }

    void SetComputerTurn(){
        int computerIndex = -1;
        foreach(RowManager row in rowList){
            if(row.piecesList.Count < piecesLimit && row.piecesList.Count >= 3){
                string[] tags = {"PlayerPiece","ComputerPiece"};
                foreach(string tag in tags){
                    int numberSamePieces = 0;
                    for(int i = row.piecesList.Count - 1; i > row.piecesList.Count - 4;i--){
                        string tagUsed = row.piecesList[i].tag;
                        if(tagUsed == tag)
                            numberSamePieces++;
                    }
                    if(numberSamePieces == 3)
                        computerIndex = row.rowIndex;
                }
            }
        }

        if(computerIndex == -1){
            computerIndex = playerRowIndex + 1;
            if(UnityEngine.Random.Range(0,10) > 5)
                computerIndex = playerRowIndex - 1;
            if(computerIndex == -1)
                computerIndex = 0;
            if(computerIndex > rowList.Count - 1)
                computerIndex = rowList.Count - 1;
        }

        AddPiece(rowList[computerIndex]);
    }

    void CheckPieces(GameObject piece){
        float pieceDistance = rowOffset * 1.8f;
        float minimumDistance = rowOffset * 0.1f;
        foreach(GameObject gamePiece in usedPieces){
            if(!gamePiece.GetComponent<PieceBehaviour>().isChecked){
                float distance = Vector3.Distance(piece.transform.position,gamePiece.transform.position);
                if(distance < pieceDistance && piece.tag == gamePiece.tag){
                    string connection = null;
                    float pieceDistanceY = Mathf.Abs(piece.transform.position.y - gamePiece.transform.position.y);
                    float pieceDistanceX = Mathf.Abs(piece.transform.position.x - gamePiece.transform.position.x);
                    if(pieceDistanceX < minimumDistance){
                        connection = "vertical";
                    }else if(pieceDistanceY < minimumDistance){
                        connection = "horizontal";
                    }else{
                        string addedText = "Up";
                        if(piece.transform.position.y > gamePiece.transform.position.y)
                            addedText = "Down";
                        if(piece.transform.position.x > gamePiece.transform.position.x){
                            connection = "rightDiagonal";
                        }else{
                            connection = "leftDiagonal";
                        }
                        connection+= addedText;
                    }
                    if(checkedPiecesList.Count == 1){
                        lastPieceConnection = connection;
                        SetPiece(gamePiece,true);
                    }else{
                        if(connection == lastPieceConnection)
                            SetPiece(gamePiece,true);   
                    }
                }
            }
        }
    }

    IEnumerator WonGame(){
        foreach(GameObject piece in checkedPiecesList){
            UIAnimation.SquishObject(piece.transform);
        }

        yield return new WaitForSeconds(1);
        if(isPlayerTurn){
            winScreenText.text = "Player Wins";
        }else{
            winScreenText.text = "Computer Wins";
        }
        UIAnimation.ShowVertical(winScreen);
        UIAnimation.FadeInScreen(fadeScreen,0.5f);
        //RestartGame();
    }

    public void HideAndRestart(){
        UIAnimation.HideVertical(winScreen);
        UIAnimation.FadeOutScreen(fadeScreen,0.5f);
        RestartGame();
    }

    public void RestartGame(){
        foreach(RowManager row in rowList){
            row.piecesList = new List<GameObject>();
        }
        usedPieces = new List<GameObject>();
        foreach(GameObject piece in piecesList){
            piece.SetActive(false);
            piece.transform.position = Vector3.zero;
        }
        gameActive = true;
        SetTurn(true);
    }

    void SetPiece(GameObject piece,bool active){
        piece.GetComponent<PieceBehaviour>().isChecked = active;
        if(active){
            checkedPiecesList.Add(piece);
            CheckPieces(piece);
            //Debug.Log(checkedPiecesList.Count + " pieces number " + lastPieceConnection + " going up " +  piece.tag);
        }
    }

    public void PlayerMove(RowManager row){
        if(gameActive && row.piecesList.Count < piecesLimit){
            AddPiece(row);
        }
    }

    GameObject GetPiece(){
        GameObject usedPiece = null;
        foreach(GameObject piece in piecesList){
            if(!piece.activeSelf && usedPiece == null){
                usedPiece = piece;
                usedPiece.SetActive(true);
            }
        }
        if(usedPiece == null){
            usedPiece = Instantiate(piece,transform.position,transform.rotation);
        }
        piecesList.Add(usedPiece);
        return usedPiece;
    }
}
