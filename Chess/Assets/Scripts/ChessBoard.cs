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

    private float totalTime = 0;
    private const float COMPUTER_TIME = 0.1f;

    void Start ()
    {
        chessLogic.Init();
        chessLogic.drawSelectHandle += ShowSelect;
        chessLogic.movePieceHandle += MovePiece;
        InitView();
    }

	void Update ()
    {
        if(SystemInfo.deviceType == DeviceType.Desktop)
        {
            if (Input.GetMouseButtonUp(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                SelectPiece(hit.point);
            }
        }
        else
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position), Vector2.zero);
                SelectPiece(hit.point);
            }
        }

        if (chessLogic.Computer)
        {
            totalTime += Time.deltaTime;
            if(totalTime >= COMPUTER_TIME)
            {
                chessLogic.ResponseMove();
                totalTime = 0;
            }
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
    }

    /// <summary>
    /// 显示隐藏选中标志
    /// </summary>
    /// <param name="visible"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    void ShowSelect(bool visible, int x = 0, int y = 0)
    {
        selectSprite.SetActive(visible);
        if (visible)
        {
            selectSprite.transform.position = new Vector2(xLocation[x - ChessLogic.COLUMN_LEFT], yLocation[y - ChessLogic.ROW_TOP]);
        }
    }

    /// <summary>
    /// 画整幅棋子
    /// </summary>
    /// <param name="board"></param>
    void DrawBoard(sbyte[] board)
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

    /// <summary>
    /// 获取点击位置
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
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

        return new Vector2(selectX, selectY);
    }

    /// <summary>
    /// 根据点击位置判断选中棋子
    /// </summary>
    /// <param name="point"></param>
    void SelectPiece(Vector2 point)
    {
        Vector2 pos = GetPostionByHit(point);
        if (pos != new Vector2(-1, -1))
        {
            int x = (int)pos.x + ChessLogic.COLUMN_LEFT;
            int y = (int)pos.y + ChessLogic.ROW_TOP;

            chessLogic.ClickSquare(x, y);
        }
    }

    /// <summary>
    /// 设置棋子位置
    /// </summary>
    /// <param name="piece"></param>
    /// <param name="boardPosition"></param>
    void SetPiecePosition(GameObject piece, Vector2 boardPosition)
    {
        piece.transform.position = new Vector2(xLocation[(int)boardPosition.x - ChessLogic.COLUMN_LEFT], yLocation[(int)boardPosition.y - ChessLogic.ROW_TOP]);
    }

    /// <summary>
    /// 棋子的值对应棋子预设
    /// </summary>
    /// <param name="pc"></param>
    /// <returns></returns>
    int ChangePcToPieceId(int pc)
    {
        if(chessLogic.Red(pc))
        {
            return pc % 8;
        }

        return pc % 8 + 7;
    }

    /// <summary>
    /// 添加一个棋子
    /// </summary>
    /// <param name="pc"></param>
    /// <param name="position"></param>
    void AddPiece(int pc, Vector2 position)
    {
        GameObject piece = Instantiate(pieceGameObject[ChangePcToPieceId(pc)]);
        piece.transform.parent = transform;
        SetPiecePosition(piece, position);
        pieceDict.Add(position, piece);
    }

    /// <summary>
    /// 移动一个棋子，如果目标位置有棋子就吃子
    /// </summary>
    /// <param name="srcPostion"></param>
    /// <param name="dstPostion"></param>
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

    /// <summary>
    /// 删掉一个棋子
    /// </summary>
    /// <param name="position"></param>
    void EatPiece(Vector2 position)
    {
        if (pieceDict.ContainsKey(position))
        {
            Destroy(pieceDict[position]);
            pieceDict.Remove(position);
        }
    }

    public void OnStartupClick()
    {
        chessLogic.MySituation.Startup();
        DrawBoard(chessLogic.MySituation.CurrentBoard);
    }
}
