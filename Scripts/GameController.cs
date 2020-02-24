using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    // 10*10のint型２次元配列を定義
    private int[,] squares = new int[10, 10];

    //EMPTY=0,WHITE=1,BLACK=-1で定義
    private const int EMPTY = 0;
    private const int WHITE = 1;
    private const int BLACK = -1;

    //現在のプレイヤー（初期プレイヤーは白）
    private int currentPlayer = WHITE;

    //カメラ情報
    private Camera camera_object;
    private RaycastHit hit;

    //prefabs
    public GameObject whiteStone;
    public GameObject blackStone;

    //テキスト
    public Text turn;
    public Text score;
    public Text final_socre;

    //白石と黒石の数
    public int white_score = 0;
    public int black_score = 0;

    // ターン表示の定型文
    private const string whiteTurn = "白のターン";
    private const string blackTurn = "黒のターン";

    // Start is called before the first frame update
    void Start()
    {
        //ターン表示テキストの初期値を代入
        turn.text = whiteTurn;


        //カメラ情報を取得
        camera_object = GameObject.Find("Main Camera").GetComponent<Camera>();

        //配列を初期化
        InitializeArray();

        //デバッグ用のメソッド
        DebugArray();
    }

    // Update is called once per frame
    void Update()
    {
        //石が置けなかったら勝敗を決める
        if (currentPlayer == WHITE)
        {
            if(!Check_put_stone(WHITE, BLACK))
            {

                if(Check_put_stone(BLACK, WHITE))
                {
                    currentPlayer = BLACK;
                    //turnを更新
                    turn.text = blackTurn;
                    return;
                }
                else
                {
                    which_win();
                    return;
                }
            }
        }
        else if (currentPlayer == BLACK)
        {
            if (!Check_put_stone(BLACK, WHITE))
            {

                if (Check_put_stone(WHITE, BLACK))
                {
                    currentPlayer = WHITE;
                    //turnを更新
                    turn.text = whiteTurn;
                    return;
                }
                else
                {
                    which_win();
                    return;
                }
            }
        }


        //マウスがクリックされたとき
        if (Input.GetMouseButtonDown(0))
        {
            //マウスのポジションを取得してRayに代入
            Ray ray = camera_object.ScreenPointToRay(Input.mousePosition);

            //マウスのポジションからRayを投げて何かに当たったらhitにいれる
            if (Physics.Raycast(ray, out hit))
            {
                //x,zの値を取得
                int x = (int)hit.collider.gameObject.transform.position.x;
                int z = (int)hit.collider.gameObject.transform.position.z;

                //マスが空のとき
                if (squares[z,x] == EMPTY)
                {
                    //白のターンのとき
                    if (currentPlayer == WHITE)
                    {
                        //裏返せる石があるかチェック
                        if (!Check(x, z, WHITE, BLACK)) return;

                        //Squaresの値を更新
                        squares[z, x] = WHITE;

                        //Stoneを出力
                        GameObject stone = Instantiate(whiteStone);
                        stone.name = z.ToString() + x.ToString();
                        stone.transform.position = hit.collider.gameObject.transform.position;

                        //挟んだ石をひっくり返す
                        reversi_white(x, z, WHITE, BLACK, whiteStone);

                        //Playerを交代
                        currentPlayer = BLACK;

                        //turnを更新
                        turn.text = blackTurn;

                        //scoreを更新
                        Score();
                        score.text = white_score.ToString() + "対" + black_score.ToString();
                    }

                    //黒のターンのとき
                    else 
                    {
                        //裏返せる石があるかチェック
                        if (!Check(x, z, BLACK, WHITE)) return;

                        //Squaresの値を更新
                        squares[z, x] = BLACK;

                        //Stoneを出力
                        GameObject stone = Instantiate(blackStone);
                        stone.name = z.ToString() + x.ToString();
                        stone.transform.position = hit.collider.gameObject.transform.position;

                        //挟んだ石をひっくり返す
                        reversi_white(x, z, BLACK, WHITE, blackStone);

                        //Playerを交代
                        currentPlayer = WHITE;

                        //turnを更新
                        turn.text = whiteTurn;

                        //scoreを更新
                        Score();
                        score.text = white_score.ToString() + "対" + black_score.ToString();
                    }

                }
            }
        }
        
    }

    //配列情報を初期化する
    private void InitializeArray()
    {
        //for文を利用して配列にアクセスする
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                //配列を空（値を0）にする
                squares[i, j] = EMPTY;
            }
        }

        squares[4,4] = 1;
        squares[5,5] = 1;
        squares[4,5] = -1;
        squares[5,4] = -1;
    }

    private void DebugArray()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                Debug.Log("(i.j) = (" + i + "," + j + ") = " + squares[i, j]);
            }
        }
    }

    //石を置ける場所があるかどうかの判定
    private bool Check_put_stone(int my_color, int enemy_color)
    {
        for (int i = 1; i < 9; i++)
        {
            for (int j = 1; j < 9; j++)
            {
                if(Check(i, j, my_color, enemy_color))
                {
                    return true;
                }

            }
        }
        return false;
    }

    //クリックした箇所に石がおけるかどうか判定する
    private bool Check(int x, int z, int my_color, int enemy_color)
    {
        while(squares[z,x] == 0)
        {
            //右横方向
            if (squares[z, x + 1] == enemy_color)
            {
                int k = x + 2;
                while (squares[z, k] == enemy_color)
                {
                    k++;
                }

                if (squares[z, k] == my_color)
                {
                    return true;
                }
            }

            //左横方向
            if (squares[z, x - 1] == enemy_color)
            {
                int k = x - 2;
                while (squares[z, k] == enemy_color)
                {
                    k--;
                }

                if (squares[z, k] == my_color)
                {
                    return true;
                }
            }

            //上方向
            if (squares[z + 1, x] == enemy_color)
            {
                int k = z + 2;
                while (squares[k, x] == enemy_color)
                {
                    k++;
                }

                if (squares[k, x] == my_color)
                {
                    return true;
                }
            }

            //下方向
            if (squares[z - 1, x] == enemy_color)
            {
                int k = z - 2;
                while (squares[k, x] == enemy_color)
                {
                    k--;
                }

                if (squares[k, x] == my_color)
                {
                    return true;
                }
            }

            //斜め（右上）方向
            if (squares[z + 1, x + 1] == enemy_color)
            {
                int k = 2;
                while (squares[z + k, x + k] == enemy_color)
                {
                    k++;
                }

                if (squares[z + k, x + k] == my_color)
                {
                    return true;
                }
            }

            //斜め（左下）方向
            if (squares[z - 1, x - 1] == enemy_color)
            {
                int k = 2;
                while (squares[z - k, x - k] == enemy_color)
                {
                    k++;
                }

                if (squares[z - k, x - k] == my_color)
                {
                    return true;
                }
            }

            //斜め（左上）方向
            if (squares[z + 1, x - 1] == enemy_color)
            {
                int k = 2;
                while (squares[z + k, x - k] == enemy_color)
                {
                    k++;
                }

                if (squares[z + k, x - k] == my_color)
                {
                    return true;
                }
            }

            //斜め（右下）方向
            if (squares[z - 1, x + 1] == enemy_color)
            {
                int k = 2;
                while (squares[z - k, x + k] == enemy_color)
                {
                    k++;
                }

                if (squares[z - k, x + k] == my_color)
                {
                    return true;
                }
            }

            return false;
        }
        return false;
    }

    //挟んだ相手の石をひっくり返す処理
    private void reversi_white(int x, int z, int my_color,int enemy_color, GameObject my_stone)
    {

        //右横方向
        if (squares[z, x + 1] == enemy_color)
        {
            //どこまで黒が続くか判定
            int k = x + 2;
            while (squares[z, k] == enemy_color)
            {
                k++;
            }

            //その後が白か確認
            if (squares[z, k] == my_color)
            {
                //白石で挟んだ黒石を白石に変える
                for (int i = x+1; i < k; i++)
                {
                    //元の石を削除
                    GameObject remove = GameObject.Find(z.ToString() + i.ToString());
                    Destroy(remove);
                    //新しい石を生成
                    GameObject stone = Instantiate(my_stone);
                    stone.name = z.ToString() + i.ToString();
                    stone.transform.position = new Vector3(i, 0, z);
                    //Squaresの値を更新
                    squares[z, i] = my_color;
                }        
            }
        }

        //左横方向
        if (x > 0)
        {
            if (squares[z, x - 1] == enemy_color)
            {
                //どこまで黒が続くか判定
                int k = x - 2;
                while (squares[z, k] == enemy_color)
                {
                    k--;
                }

                //その後が白か確認
                if (squares[z, k] == my_color)
                {
                    //白石で挟んだ黒石を白石に変える
                    for (int i = x - 1; i > k; i--)
                    {
                        //元の石を削除
                        GameObject remove = GameObject.Find(z.ToString() + i.ToString());
                        Destroy(remove);
                        //新しい石を生成
                        GameObject stone = Instantiate(my_stone);
                        stone.name = z.ToString() + i.ToString();
                        stone.transform.position = new Vector3(i, 0, z);
                        //Squaresの値を更新
                        squares[z, i] = my_color;
                    }
                }
            }
        }

        //上方向
        if (squares[z+1, x] == enemy_color)
        {
            //どこまで黒が続くか判定
            int k = z + 2;
            while (squares[k, x] == enemy_color)
            {
                k++;
            }

            //その後が白か確認
            if (squares[k, x] == my_color)
            {
                //白石で挟んだ黒石を白石に変える
                for (int i = z + 1; i < k; i++)
                {
                    //元の石を削除
                    GameObject remove = GameObject.Find(i.ToString() + x.ToString());
                    Destroy(remove);
                    //新しい石を生成
                    GameObject stone = Instantiate(my_stone);
                    stone.name = i.ToString() + x.ToString();
                    stone.transform.position = new Vector3(x, 0, i);
                    //Squaresの値を更新
                    squares[i, x] = my_color;
                }
            }
        }

        //下方向
        if (squares[z - 1, x] == enemy_color)
        {
            //どこまで黒が続くか判定
            int k = z - 2;
            while (squares[k, x] == enemy_color)
            {
                k--;
            }

            //その後が白か確認
            if (squares[k, x] == my_color)
            {
                //白石で挟んだ黒石を白石に変える
                for (int i = z - 1; i > k; i--)
                {
                    //元の石を削除
                    GameObject remove = GameObject.Find(i.ToString() + x.ToString());
                    Destroy(remove);
                    //新しい石を生成
                    GameObject stone = Instantiate(my_stone);
                    stone.name = i.ToString() + x.ToString();
                    stone.transform.position = new Vector3(x, 0, i);
                    //Squaresの値を更新
                    squares[i, x] = my_color;
                }
            }
        }

        //斜め(右上)
        if (squares[z + 1, x + 1] == enemy_color)
        {
            //どこまで黒が続くか判定
            int count = 2;
            while (squares[z+count, x+count] == enemy_color)
            {
                count++;
            }

            //その後が白か確認
            if (squares[z + count, x + count] == my_color)
            {
                //白石で挟んだ黒石を白石に変える
                for (int i = 1; i < count; i++)
                {
                    //元の石を削除
                    GameObject remove = GameObject.Find((z+i).ToString() + (x+i).ToString());
                    Destroy(remove);
                    //新しい石を生成
                    GameObject stone = Instantiate(my_stone);
                    stone.name = (z + i).ToString() + (x + i).ToString();
                    stone.transform.position = new Vector3(x + i, 0, z + i);
                    //Squaresの値を更新
                    squares[z+i, x+i] = my_color;
                }
            }
        }

        //斜め(左下)
        if (squares[z - 1, x - 1] == enemy_color)
        {
            //どこまで黒が続くか判定
            int count = 2;
            while (squares[z - count, x - count] == enemy_color)
            {
                count++;
            }

            //その後が白か確認
            if (squares[z - count, x - count] == my_color)
            {
                //白石で挟んだ黒石を白石に変える
                for (int i = 1; i < count; i++)
                {
                    //元の石を削除
                    GameObject remove = GameObject.Find((z - i).ToString() + (x - i).ToString());
                    Destroy(remove);
                    //新しい石を生成
                    GameObject stone = Instantiate(my_stone);
                    stone.name = (z - i).ToString() + (x - i).ToString();
                    stone.transform.position = new Vector3(x - i, 0, z - i);
                    //Squaresの値を更新
                    squares[z - i, x - i] = my_color;
                }
            }
        }

        //斜め(右下)
        if (squares[z - 1, x + 1] == enemy_color)
        {
            //どこまで黒が続くか判定
            int count = 2;
            while (squares[z - count, x + count] == enemy_color)
            {
                count++;
            }

            //その後が白か確認
            if (squares[z - count, x + count] == my_color)
            {
                //白石で挟んだ黒石を白石に変える
                for (int i = 1; i < count; i++)
                {
                    //元の石を削除
                    GameObject remove = GameObject.Find((z - i).ToString() + (x + i).ToString());
                    Destroy(remove);
                    //新しい石を生成
                    GameObject stone = Instantiate(my_stone);
                    stone.name = (z - i).ToString() + (x + i).ToString();
                    stone.transform.position = new Vector3(x + i, 0, z - i);
                    //Squaresの値を更新
                    squares[z - i, x + i] = my_color;
                }
            }
        }

        //斜め(左上)
        if (squares[z + 1, x - 1] == enemy_color)
        {
            //どこまで黒が続くか判定
            int count = 2;
            while (squares[z + count, x - count] == enemy_color)
            {
                count++;
            }

            //その後が白か確認
            if (squares[z + count, x - count] == my_color)
            {
                //白石で挟んだ黒石を白石に変える
                for (int i = 1; i < count; i++)
                {
                    //元の石を削除
                    GameObject remove = GameObject.Find((z + i).ToString() + (x - i).ToString());
                    Destroy(remove);
                    //新しい石を生成
                    GameObject stone = Instantiate(my_stone);
                    stone.name = (z + i).ToString() + (x - i).ToString();
                    stone.transform.position = new Vector3(x - i, 0, z + i);
                    //Squaresの値を更新
                    squares[z + i, x - i] = my_color;
                }
            }
        }
    }
    

    //どっちが勝ったか返す
    private void which_win()
    {
        int count_white = 0;
        int count_black = 0;

        for (int i = 1; i < 9; i++)
        {
            for (int j = 1; j < 9; j++)
            {
                if (squares[i, j] == WHITE)
                {
                    count_white += 1;
                }
                else if (squares[i, j] == BLACK)
                {
                    count_black += 1;
                }
            }
        }

        if (count_white > count_black)
        {
            Debug.Log(count_white + "対" + count_black + "で白の勝ち");
            final_socre.text = white_score.ToString() + "対" + black_score.ToString() + "で白の勝ち";
        }
        else if (count_black > count_white)
        {
            Debug.Log(count_black + "対" + count_white + "で黒の勝ち");
            final_socre.text = white_score.ToString() + "対" + black_score.ToString() + "で黒の勝ち";
        }
        else
        {
            Debug.Log(count_black + "対" + count_white + "で同点");
            final_socre.text = white_score.ToString() + "対" + black_score.ToString() + "で同点";
        }
    }

    //どっちが勝ったか返す
    private void Score()
    {
        white_score = 0;
        black_score = 0;

        for (int i = 1; i < 9; i++)
        {
            for (int j = 1; j < 9; j++)
            {
                if (squares[i, j] == WHITE)
                {
                    white_score += 1;
                }
                else if (squares[i, j] == BLACK)
                {
                    black_score += 1;
                }
            }
        }
    }

}
