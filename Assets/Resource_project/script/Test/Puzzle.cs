using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Puzzle : MonoBehaviour
{
    public GridLayoutGroup puzzleGrid; // 拼圖的網格佈局組件
    public List<Button> puzzlePieces;  // 拼圖塊的列表，包括空白塊
    public Button emptySlot;           // 空白塊的按鈕
    public Sprite newImage;

    private int gridSize = 4;          // 定義拼圖的行列數（4x4）
    private List<int> puzzleState;     // 存儲拼圖塊的當前狀態
    private int emptySlotIndex;        // 空白塊的當前索引

    void Start()
    {
        InitializePuzzle(); // 初始化拼圖
    }

    void InitializePuzzle()
    {
        // 初始化拼圖，設置初始位置
        puzzleState = new List<int>();

        for (int i = 0; i < puzzlePieces.Count; i++)
        {
            puzzleState.Add(i); // 將拼圖塊的索引加入列表
            int index = i; // 捕捉當前索引以供回調使用
            puzzlePieces[i].onClick.AddListener(() => OnPieceClicked(index)); // 為每個拼圖塊按鈕添加點擊事件監聽器
        }

        emptySlotIndex = puzzlePieces.Count - 1; // 將空白塊放在最後一個位置
        //ShufflePuzzle(); // 隨機打亂拼圖塊的位置
        UpdatePuzzleDisplay(); // 更新拼圖顯示
    }

    void ShufflePuzzle()
    {
        do
        {
            // 白癡洗牌演算法
            for (int i = puzzleState.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                int temp = puzzleState[i];
                puzzleState[i] = puzzleState[randomIndex];
                puzzleState[randomIndex] = temp;
            }

            // 更新空白塊的位置
            emptySlotIndex = puzzleState.IndexOf(puzzlePieces.Count - 1);
        }
        while (!IsSolvable()); // 確保拼圖狀態是可解的
    }

    bool IsSolvable()
    {
        int inversions = 0;

        // 計算逆序數
        for (int i = 0; i < puzzleState.Count - 1; i++)
        {
            for (int j = i + 1; j < puzzleState.Count; j++)
            {
                // 排除空白塊
                if (puzzleState[i] != emptySlotIndex && puzzleState[j] != emptySlotIndex)
                {
                    if (puzzleState[i] > puzzleState[j])
                    {
                        inversions++;
                    }
                }
            }
        }

        // 如果是奇數行數且空白塊在偶數行，逆序數應為奇數
        if (gridSize % 2 == 1)
        {
            return inversions % 2 == 0;
        }
        else
        {
            int emptyRowFromBottom = gridSize - (emptySlotIndex / gridSize);
            if (emptyRowFromBottom % 2 == 0)
            {
                return inversions % 2 == 1;
            }
            else
            {
                return inversions % 2 == 0;
            }
        }
    }

    public void OnPieceClicked(int pieceIndex)
    {
        int currentPosition = puzzleState.IndexOf(pieceIndex); // 獲取當前點擊的拼圖塊在狀態中的位置
        if (IsAdjacent(currentPosition)) // 檢查拼圖塊是否與空白塊相鄰
        {
            SwapPieces(currentPosition); // 交換拼圖塊與空白塊的位置
            UpdatePuzzleDisplay(); // 更新拼圖顯示
            CheckIfSolved(); // 檢查拼圖是否已經完成
        }
    }

    bool IsAdjacent(int position)
    {
        // 檢查點擊的拼圖塊是否與空白塊相鄰（左右或上下）
        int row = position / gridSize;
        int col = position % gridSize;

        int emptyRow = emptySlotIndex / gridSize;
        int emptyCol = emptySlotIndex % gridSize;

        return (row == emptyRow && Mathf.Abs(col - emptyCol) == 1) ||
               (col == emptyCol && Mathf.Abs(row - emptyRow) == 1);
    }

    void SwapPieces(int piecePosition)
    {
        // 交換拼圖塊與空白塊在狀態列表中的位置
        int temp = puzzleState[piecePosition];
        puzzleState[piecePosition] = puzzleState[emptySlotIndex];
        puzzleState[emptySlotIndex] = temp;

        // 更新空白塊的位置索引
        emptySlotIndex = piecePosition;
    }

    void UpdatePuzzleDisplay()
    {
        // 根據狀態列表中的順序更新拼圖塊的顯示順序
        for (int i = 0; i < puzzleState.Count; i++)
        {
            puzzlePieces[puzzleState[i]].transform.SetSiblingIndex(i);
        }
    }

    void CheckIfSolved()
    {
        // 檢查所有拼圖塊是否按正確順序排列
        for (int i = 0; i < puzzleState.Count - 1; i++)
        {
            if (puzzleState[i] != i)
            {
                return;
            }
        }
        ChangePuzzle();
        Debug.Log("Puzzle Solved!");
    }

    public void ChangePuzzle()
    {
        
        emptySlot.onClick.RemoveAllListeners();
        emptySlot.onClick.AddListener(() => NewEmptySlotAction("小塊拼圖"));
    }

    public void NewEmptySlotAction(string puzzle)
    {
        Image emptySlotImage = emptySlot.GetComponent<Image>();
        string dragItemName = FindObjectOfType<DragAndDrop>().item.itemName;
        if (dragItemName == puzzle && FindObjectOfType<InventorySystem>().isDragging)
        {
            emptySlotImage.sprite = newImage;
            emptySlot.GetComponent<Item>().enabled = true;
            Item item = emptySlot.GetComponent<Item>();
            item.PickUpItem();
        }
    }
}
