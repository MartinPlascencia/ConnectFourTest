using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowManager : MonoBehaviour
{
    [Header("Row Stats")]
    public int piecesNumber = 0;
    public Transform startPosition;
    public GameObject lastPiece;
    public List<GameObject> piecesList;
    [System.NonSerialized]
    public int rowIndex;
}
