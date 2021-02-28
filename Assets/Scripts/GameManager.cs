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
    bool drawGame = false;

    [Header("Game Assets")]
    public Transform throwPivot;
    public GameObject piece;
    public List<RowManager> rowList;
    public List<GameObject> piecesList;
    public List<GameObject> usedPieces;

    List<GameObject> checkedPiecesList;

    public Sprite playerPieceTexture;
    public Sprite computerPieceTexture;
    public GameObject restartButton;

    [Header("Game Screens")]
    public Text turnText;
    public Text winScreenText;
    public RectTransform winScreen;
    public CanvasGroup fadeScreen;
    public CanvasGroup whiteFadeScreen;

    void Start(){
        //SoundManager.instance.PlayMusic("puzzle_game_02",0.5f);
        InitializeRows();
        RestartGame();
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
            SoundManager.instance.Play("dragUnit");
            restartButton.SetActive(false);
        }else{
            SoundManager.instance.Play("switch");
        }

        GameObject piece = GetPiece();
        piece.transform.SetParent(transform);
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
        piece.transform.DOMoveY(throwPivot.position.y,0.5f).From().OnComplete(CheckWin).SetEase(Ease.InCubic);
    }

    void SetTurn(bool active){
        isPlayerTurn = active;
        if(active){
            turnText.text = "Player's turn";
            restartButton.SetActive(true);
        }else{
            turnText.text = "Computer's turn";
        }

        CheckRows();
    }

    void CheckRows(){
        bool hasSpace = true;
        foreach(RowManager row in rowList){
            hasSpace = row.piecesList.Count < piecesLimit;
        }
        if(!hasSpace){
            gameActive = false;
            drawGame = true;
            StartCoroutine(WonGame());
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
                Invoke("SetComputerTurn",0.3f);
            else  
                gameActive = true;
        }
    }

    void SetComputerTurn(){

        //Vertical Checking
        int computerIndex = -1;
        foreach(RowManager row in rowList){
            if(row.piecesList.Count < piecesLimit && row.piecesList.Count >= 3){
                string[] tags = {"ComputerPiece","PlayerPiece"};
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

        //Horizontal Checking
        if(computerIndex == -1){
            for(int i = 0; i < rowList.Count; i++){
                for(int index = 0; index < 5;index++){
                    string[] tags = {"ComputerPiece","PlayerPiece"};
                    foreach(string tag in tags){
                        bool hasPiece = true;
                        for(var u = 0; u <3;u++){
                            if(hasPiece)
                                hasPiece = CheckPieceTag(rowList[index + u].piecesList,i,tag);
                        }
                        if(hasPiece){
                            if(index < 4 && rowList[index + 3].piecesList.Count == i){
                                computerIndex = index + 3;
                            }else if(index > 0 && rowList[index - 1].piecesList.Count == i){
                                computerIndex = index - 1;
                            }
                        }
                    }
                }
            }
        }

        //Random Picking if the player is not going to win
        if(computerIndex == -1){
            computerIndex = playerRowIndex + 1;
            if(UnityEngine.Random.Range(0,10) > 5)
                computerIndex = playerRowIndex - 1;
            if(computerIndex == -1)
                computerIndex = 0;
            if(computerIndex > rowList.Count - 1)
                computerIndex = rowList.Count - 1;
        }
        if(rowList[computerIndex].piecesList.Count == piecesLimit){
            if(CheckPlayerAround())
                AddPiece(rowList[GetAvailableRowIndex()]);
            else
                SetComputerTurn();
        }else{
            AddPiece(rowList[computerIndex]);
        }
    }

    bool CheckPieceTag(List<GameObject> piece, int index, string tag){
        return piece.Count > index && piece[index].tag == tag;
    }

    bool CheckRowFull(int index){
        return rowList[index].piecesList.Count == piecesLimit;
    }

    bool CheckPlayerAround(){
        if(playerRowIndex == 0){
            return CheckRowFull(1);
        }else if(playerRowIndex == piecesLimit){
            return CheckRowFull(piecesLimit-1);
        }else{
            return CheckRowFull(playerRowIndex - 1) && CheckRowFull(playerRowIndex) && CheckRowFull(playerRowIndex+1);
        }
    }

    int GetAvailableRowIndex(){
        int index = 0;
        foreach(RowManager row in rowList){
            if(row.piecesList.Count < piecesLimit && index == 0)
                index = row.rowIndex;
        }
        return index;
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

        restartButton.SetActive(false);
        if(drawGame){
            winScreenText.text = "draw";
            SoundManager.instance.Play("level_failed");
        }else{
            if(isPlayerTurn){
                winScreenText.text = "Player Wins";
                SoundManager.instance.Play("winGame");
            }else{
                winScreenText.text = "Computer Wins";
                SoundManager.instance.Play("level_failed");
            }
        }
        

        yield return new WaitForSeconds(1);
        
        UIAnimation.ShowVertical(winScreen);
        UIAnimation.FadeInScreen(fadeScreen,0.5f);
        //RestartGame();
    }

    public void HideAndRestart(){
        UIAnimation.HideVertical(winScreen);
        UIAnimation.FadeOutScreen(fadeScreen,0.5f);
        Invoke("RestartGame",1f);
    }

    public void RestartGame(){
        drawGame = false;
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
        SoundManager.instance.Play("winGame");

        whiteFadeScreen.alpha = 1;
        whiteFadeScreen.gameObject.SetActive(true);
        UIAnimation.FadeOutScreen(whiteFadeScreen,0.5f);
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
