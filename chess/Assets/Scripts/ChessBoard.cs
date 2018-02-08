using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessBoard : MonoBehaviour
{
    private float[] xLocation = { -2.488f, -1.866f, -1.244f, -0.622f, 0.0f, 0.622f, 1.244f, 1.866f, 2.488f };
    private float[] yLocation = { 2.8f, 2.178f, 1.556f, 0.934f, 0.312f, -0.31f, -0.932f, -1.554f, -2.176f, -2.8f};
    private float squareDelta = 0.622f;
    private Dictionary<int, GameObject> pieceDict = new Dictionary<int, GameObject>();
    private GameObject selectSprite;

    [SerializeField]
    private GameObject[] pieceGameObject;
    [SerializeField]
    private GameObject selectGameObject;

    private ChessLogic chessLogic = new ChessLogic();

    void Start ()
    {
        createSelect();
        chessLogic.init();
        drawBoard(chessLogic.CurrentChessBoard);
	}

	void Update ()
    {
        if (Input.GetMouseButtonUp(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            Debug.Log(hit.point);
            selectPiece(hit.point);
        }
	}

    void createSelect()
    {
        if(selectSprite == null)
        {
            selectSprite = Instantiate(selectGameObject);
            selectSprite.transform.parent = transform;
            selectSprite.SetActive(false);
        }
    }

    void showSelect(bool visible, int x = 0, int y = 0)
    {
        selectSprite.SetActive(visible);
        if (visible)
        {
            selectSprite.transform.position = new Vector2(xLocation[x], yLocation[y]);
        }
    }

    void drawBoard(int[] board)
    {
        for(int i = ChessLogic.COLUMN_LEFT; i <= ChessLogic.COLUMN_RIGHT; ++i)
        {
            for(int j = ChessLogic.ROW_TOP; j <= ChessLogic.ROW_BOTTOM; ++j)
            {
                int pc = board[chessLogic.CoordXY(i, j)];
                if(pc != 0)
                {
                    int pieceId = chessLogic.PieceTag(pc);
                    addPiece(pieceId, i, j);
                }
            }
        }
    }

    Vector2 getPostionByHit(Vector2 point)
    {
        int selectX = -1;
        int selectY = -1;
        float halfDelta = squareDelta / 2;
        for (int i = 0; i < xLocation.Length; i++)
        {
            float location = xLocation[i];
            if (point.x < location + halfDelta && point.x > location - halfDelta)
            {
                selectX = i;
                break;
            }
        }

        for(int i = 0; i < yLocation.Length; i++)
        {
            float location = yLocation[i];
            if (point.y < location + halfDelta && point.y > location - halfDelta)
            {
                selectY = i;
                break;
            }
        }

        if(selectX != -1 && selectY != -1)
        {
            return new Vector2(selectX, selectY);
        }

        return Vector2.zero;
    }

    void selectPiece(Vector2 point)
    {
        Vector2 pos = getPostionByHit(point);
        if (pos != Vector2.zero)
        {
            int x = (int)pos.x;
            int y = (int)pos.y;
            if (pieceDict.ContainsKey(chessLogic.CoordXY(x + ChessLogic.COLUMN_LEFT, y + ChessLogic.ROW_TOP)))
            {
                showSelect(true, x, y);
            }  
        }
    }

    void setPiecePosition(GameObject piece, int x, int y)
    {
        piece.transform.position = new Vector2(xLocation[x - ChessLogic.COLUMN_LEFT], yLocation[y - ChessLogic.ROW_TOP]);
    }

    void addPiece(int pieceId, int x, int y)
    {
        GameObject piece = Instantiate(pieceGameObject[pieceId]);
        piece.transform.parent = transform;
        setPiecePosition(piece, x, y);
        pieceDict.Add(chessLogic.CoordXY(x, y), piece);
    }

    void movePiece(int srcX, int srcY, int dstX, int dstY)
    {
        int srcKey = chessLogic.CoordXY(srcX, srcY);
        if(pieceDict.ContainsKey(srcKey))
        {
            GameObject piece = pieceDict[srcKey];
            pieceDict.Remove(srcKey);

            int dstKey = chessLogic.CoordXY(dstX, dstY);
            pieceDict[dstKey] = piece;
            setPiecePosition(piece, dstX, dstY);
        }
    }

    void eatPiece(int x, int y)
    {
        int key = chessLogic.CoordXY(x, y);
        if (pieceDict.ContainsKey(key))
        {
            GameObject piece = pieceDict[key];
            Destroy(piece);
            pieceDict.Remove(key);
        }
    }
}
