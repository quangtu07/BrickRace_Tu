using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Floor : MonoBehaviour
{
    [SerializeField] private Brick brickFloor; 
    [SerializeField] private Transform brickPrefabHolder;
    public List<Brick> brickInFloor = new List<Brick>();

    [SerializeField] private Transform brickSpawnPosition;

    public List<int> colorNumber = new List<int>();

    private int[,] BrickCoordinate = new int[8, 8];

    private void Start()
    {
        LoadMap();
        InvokeRepeating(nameof(RestoreMap), 0f, 3f);
    }

    // sinh brick khi start chua co mau mac dinh
    private void LoadMap()
    {
        for (int i = 0; i < BrickCoordinate.GetLength(0); i++)
        {
            for (int j = 0; j < BrickCoordinate.GetLength(1); j++)
            {
                brickFloor.ChangeColor(ColorType.None);
                InstantiateBrick(i , j);
            }
        }
    }

    // sau 3s doi mau vien gach dua tren, nhung mau co trong colorNumber
    public void RestoreMap()
    {
        if (brickInFloor.Count > 0)
        {
            for (int i = 0; i < brickInFloor.Count; i++)
            {
                int rand = Random.Range(1, 5);

                if (CheckColorBrickInStage(rand))
                {
                    if (rand == 1)
                    {
                        brickInFloor[i].ChangeColor(ColorType.Red);
                    }

                    if (rand == 2)
                    {

                        brickInFloor[i].ChangeColor(ColorType.Blue);
                    }

                    if (rand == 3)
                    {
                        brickInFloor[i].ChangeColor(ColorType.Green);
                    }

                    if (rand == 4)
                    {
                        brickInFloor[i].ChangeColor(ColorType.Orange);
                    }
                }

            }
        }
    }

    public bool CheckColorBrickInStage(int rand)
    {
        if (colorNumber.Count > 0)
        {
            for (int i = 0; i < colorNumber.Count; i++)
            {
                if (rand == colorNumber[i])
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void InstantiateBrick(int i, int j)
    {

        Brick brickPrefab = Instantiate(brickFloor, brickSpawnPosition.position + new Vector3(i, 0.1f, -j), Quaternion.identity);
        brickPrefab.transform.parent = this.brickPrefabHolder;
        brickInFloor.Add(brickPrefab);
    }

    public void ClearBrickInStage()
    {
        if (brickInFloor.Count > 0)
        {
            for (int i = 0; i < brickInFloor.Count; i++)
            {
                Destroy(brickInFloor[i].gameObject);
            }
            brickInFloor.Clear();
        }
    }

    public Vector3 SeekBrick(ColorType color)
    {
        if (brickInFloor.Count > 0)
        {
   
            Transform brickNearest = brickInFloor[0].transform;

            if (brickNearest != null)
            {
                float dis;
                float disMin = Vector3.Distance(transform.position, brickNearest.position);
                for (int i = 0; i < brickInFloor.Count; i++)
                {
                    if (brickInFloor[i] == null) continue;
                    if (color == brickInFloor[i].color)
                    {
                        dis = Vector3.Distance(transform.position, brickInFloor[i].transform.position);
                        if (dis < disMin)
                        {
                            disMin = dis;
                        }
                    }

                }

                for (int i = 0; i < brickInFloor.Count; i++)
                {
                    if (brickInFloor[i] == null) continue;
                    if (color == brickInFloor[i].color)
                    {
                        dis = Vector3.Distance(transform.position, brickInFloor[i].transform.position);
                        if (Mathf.Abs(dis - disMin) <= 0.1f)
                        {
                            return brickInFloor[i].transform.position;
                        }
                    }

                }
            }

            
        }

        return Vector3.zero;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag is TagName.PLAYER_TAG or TagName.ENEMY_TAG)
        {

            if (other.gameObject.GetComponent<Character>().color == ColorType.Red)
            {
                colorNumber.Add(1);
            }

            if (other.gameObject.GetComponent<Character>().color == ColorType.Blue)
            {

                colorNumber.Add(2);
            }

            if (other.gameObject.GetComponent<Character>().color == ColorType.Green)
            {

                colorNumber.Add(3);
            }

            if (other.gameObject.GetComponent<Character>().color == ColorType.Orange)
            {

                colorNumber.Add(4);
            }

        }

    }


}
