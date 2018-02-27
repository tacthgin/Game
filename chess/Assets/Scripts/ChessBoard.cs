using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessBoard : MonoBehaviour
{
    private float[] xLocation = { -2.488f, -1.866f, -1.244f, -0.622f, 0.0f, 0.622f, 1.244f, 1.866f, 2.488f };
    private float[] yLocation = { 2.8f, 2.178f, 1.556f, 0.934f, 0.312f, -0.31f, -0.932f, -1.554f, -2.176f, -2.8f};
    private float squareDelta = 0.622f;
    private Dictionary<Vector2, GameObject> pieceDict = new Dictionary<Vector2, GameObject>();
    private GameObject selectSprite;

    [SerializeField]
    private GameObject[] pieceGameObject;
    [SerializeField]
    private GameObject selectGameObject;

    private ChessLogic chessLogic = new ChessLogic();

    void Start ()
    {
        chessLogic.Init();
        chessLogic.drawSelectHandle += ShowSelect;
        chessLogic.movePieceHandle += MovePiece;
        InitView();
    }

	void Update ()
    {
        if (Input.GetMouseButtonUp(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            Debug.Log(hit.point);
            SelectPiece(hit.point);
        }
	}

    void InitView()
    {
        if(selectSprite == null)
        {
            selectSprite = Instantiate(selectGameObject);
            selectSprite.transform.parent = transform;
            selectSprite.SetActive(false);
        }
        DrawBoard(chessLogic.MySituation.CurrentBoard);
    }

    void ShowSelect(bool visible, int x = 0, int y = 0)
    {
        selectSprite.SetActive(visible);
        if (visible)
        {
            selectSprite.transform.position = new Vector2(xLocation[x - ChessLogic.COLUMN_LEFT], yLocation[y - ChessLogic.ROW_TOP]);
        }
    }

    void DrawBoard(byte[] board)
    {
        for(int i = ChessLogic.COLUMN_LEFT; i <= ChessLogic.COLUMN_RIGHT; ++i)
        {
            for(int j = ChessLogic.ROW_TOP; j <= ChessLogic.ROW_BOTTOM; ++j)
            {
                int pc = board[chessLogic.CoordXY(i, j)];
                if(pc != 0)
                {
                    AddPiece(pc, new Vector2(i, j));
                }
            }
        }
    }

    Vector2 GetPostionByHit(Vector2 point)
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

    void SelectPiece(Vector2 point)
    {
        Vector2 pos = GetPostionByHit(point);
        if (pos != Vector2.zero)
        {
            int x = (int)pos.x + ChessLogic.COLUMN_LEFT;
            int y = (int)pos.y + ChessLogic.ROW_TOP;

            chessLogic.ClickSquare(x, y);
        }
    }

    void SetPiecePosition(GameObject piece, Vector2 boardPosition)
    {
        piece.transform.position = new Vector2(xLocation[(int)boardPosition.x - ChessLogic.COLUMN_LEFT], yLocation[(int)boardPosition.y - ChessLogic.ROW_TOP]);
    }

    int ChangePcToPieceId(int pc)
    {
        if(chessLogic.Red(pc))
        {
            return pc % 8;
        }

        return pc % 8 + 7;
    }

    void AddPiece(int pc, Vector2 position)
    {
        GameObject piece = Instantiate(pieceGameObject[ChangePcToPieceId(pc)]);
        piece.transform.parent = transform;
        SetPiecePosition(piece, position);
        pieceDict.Add(position, piece);
    }

    void MovePiece(Vector2 srcPostion, Vector2 dstPostion)
    {
        if(pieceDict.ContainsKey(srcPostion))
        {
            GameObject piece = pieceDict[srcPostion];
            pieceDict.Remove(srcPostion);

            if(pieceDict.ContainsKey(dstPostion))
            {
                EatPiece(dstPostion);
            }
            pieceDict[dstPostion] = piece;
            SetPiecePosition(piece, dstPostion);
        }
    }

    void EatPiece(Vector2 position)
    {
        if (pieceDict.ContainsKey(position))
        {
            Destroy(pieceDict[position]);
            pieceDict.Remove(position);
        }
    }
}
