using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class CSVAssetReader_Test : MonoBehaviour
{
    public Text fildText;
    public Text findFildText;
    public int currentRow;
    public int priorRow = -1;

    [Header("로드할 TextAsset 데이터를 넣어주세요")]
    public TextAsset myTxt;

    public InputField idFild;
    public InputField objectFild;

    public Transform content;
    public GridLayoutGroup table;

    public Transform keyContent;
    public GridLayoutGroup keyTable;
    public Image original;
    public Image original2;
    public Transform line;
    public Transform line2;

    public TextAsset file;
    public Transform code;
    public Image original3;
    public Image originalHead;
    public Image originalInfo;
    public Button prior;
    public Button next;

    public Transform line3;
    public Transform detail;
    public Transform detailName;
    public Transform step;
    public Transform info;
    public GameObject DetailPanel;

    public GameObject valuePanel;
    public Text valueName;
    public Text value;

    int lineInt;
    string ofString;
    string exportData;
    List<string> kl;
    List<string> kl2;
    List<Dictionary<string, object>> data;
    int diffCnt;
    public void ShowValue(int r, string n)
    {
        valuePanel.SetActive(true);
        valueName.text = n;
        value.text = data[r][n].ToString();
    }
    public static void InsertionSort<T>(IList<T> list, Comparison<T> comparison)
    {
        if (list == null)
            throw new ArgumentNullException("list");
        if (comparison == null)
            throw new ArgumentNullException("comparison");

        int count = list.Count;
        for (int j = 1; j < count; j++)
        {
            T key = list[j];

            int i = j - 1;
            for (; i >= 0 && comparison(list[i], key) > 0; i--)
            {
                list[i + 1] = list[i];
            }
            list[i + 1] = key;
        }
    }

    public void DetailTable(string name)
    {
        DetailPanel.SetActive(true);
        detailName.GetChild(0).GetComponent<Text>().text = name;
        string current = data[0][name].ToString();
        Image.Instantiate(original2, line3).transform.GetChild(0).GetComponent<Text>().text = data[0][kl[0]].ToString();
        Image.Instantiate(original, detail).transform.GetChild(0).GetComponent<Text>().text = current;

        for (int i=1; i < data.Count; i++)
        {
            if(current != data[i][name].ToString())
            {
                current = data[i][name].ToString();
                Image.Instantiate(original2, line3).transform.GetChild(0).GetComponent<Text>().text = data[i][kl[0]].ToString();
                Image.Instantiate(original, detail).transform.GetChild(0).GetComponent<Text>().text = current;
            }
        }
    }

    public void ClearDetail()
    {
        foreach(Transform child in line3)
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach(Transform child in detail)
        {
            GameObject.Destroy(child.gameObject);
        }
        DetailPanel.SetActive(false);
    }
    public void OnEnable()
    {
        prior.interactable = false;
        string[] lines = Regex.Split(file.text, @"\n");
        int cnt=0;
        for (int i = 0; i < lines.Length; i++)
        {
            Image.Instantiate(original3, code).transform.GetChild(0).GetComponent<Text>().text = lines[i];
            Image.Instantiate(original2, line2).transform.GetChild(0).GetComponent<Text>().text = (++cnt).ToString();
        }
        TextLoard();
        ColorCurrent();
    }
    private int CompareValueNext(string x, string y)
    {
        if(x == null)
        {
            if(y == null)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        else
        {
            if (y == null)
            {
                return -1;
            }

            if(!data[currentRow][x].Equals(data[currentRow + 1][x]))
            {
                if (!data[currentRow][y].Equals(data[currentRow + 1][y]))
                {
                    return 0;
                }
                else
                {
                    return -1;
                }

            }
            else
            {
                if (!data[currentRow][y].Equals(data[currentRow + 1][y]))
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }

        }
    }
    private int CompareValuePrior(string x, string y)
    {
        if (x == null)
        {
            if (y == null)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        else
        {
            if (y == null)
            {
                return -1;
            }

            if (!data[currentRow][x].Equals(data[currentRow - 1][x]))
            {
                if (!data[currentRow][y].Equals(data[currentRow - 1][y]))
                {
                    return 0;
                }
                else
                {
                    return -1;
                }

            }
            else
            {
                if (!data[currentRow][y].Equals(data[currentRow - 1][y]))
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }

        }
    }
    private void FillTable()
    {
        foreach(Transform t in keyContent)
        {
            GameObject.Destroy(t.gameObject);
        }
        //col head
        for (int j = 0; j < data[0].Count-3; j++)
        {
            Image temp = Image.Instantiate(originalHead, keyContent);
            if(j<diffCnt)
                temp.GetComponent<Image>().color = Color.red;
            temp.transform.GetChild(0).GetComponent<Text>().text = kl2[j].ToString();
            int c = j;
            temp.GetComponent<Button>().onClick.AddListener(delegate { DetailTable(kl2[c]); });

        }

        for (int i = 0; i < data.Count; i++)
        {
            for (int j = 0; j < data[0].Count-3; j++)
            {
                content.GetChild(i*(data[0].Count-3)+j).GetChild(0).GetComponent<Text>().text = data[i][kl2[j]].ToString();
                content.GetChild(i * (data[0].Count - 3) + j).GetComponent<Button>().onClick.RemoveAllListeners();
                int a = i;
                string b = kl2[j];
                content.GetChild(i * (data[0].Count - 3) + j).GetComponent<Button>().onClick.AddListener(delegate { ShowValue(a,b); });
            }
        }
    }
    public void Next()
    {   diffCnt = 0;
        foreach(string s in kl2)
        {
            if(!data[currentRow][s].Equals(data[currentRow + 1][s]))
            {
                diffCnt++;
            }
        }
        InsertionSort(kl2, CompareValueNext);
        FillTable();


        currentRow++;
        prior.interactable = true;
        
        line.parent.parent.gameObject.GetComponent<ScrollRect>().verticalNormalizedPosition = 1 - (float) currentRow / data.Count;
        line2.parent.parent.gameObject.GetComponent<ScrollRect>().verticalNormalizedPosition = 1 - (float)(Int32.Parse(data[currentRow][kl[0]].ToString()) - 1) / data.Count;
        content.parent.parent.gameObject.GetComponent<ScrollRect>().verticalNormalizedPosition = 1 - (float)currentRow / data.Count;
        code.parent.parent.gameObject.GetComponent<ScrollRect>().verticalNormalizedPosition = 1 - (float)(Int32.Parse(data[currentRow][kl[0]].ToString()) - 1) / data.Count;

        

        ColorCurrent();
        if (currentRow+1 >= data.Count)
        {
            next.interactable = false;
            //각 버튼비활성화
            return;
        }
    }
    
    public void Prior()
    {   diffCnt = 0;
        foreach (string s in kl2)
        {
            if (!data[currentRow][s].Equals(data[currentRow - 1][s]))
            {
                diffCnt++;
            }
        }

        InsertionSort(kl2, CompareValuePrior);
        FillTable();


        currentRow--;
        next.interactable = true;
        
        line.parent.parent.gameObject.GetComponent<ScrollRect>().verticalNormalizedPosition = 1 - (float)currentRow / data.Count;
        line2.parent.parent.gameObject.GetComponent<ScrollRect>().verticalNormalizedPosition = 1 - (float)(Int32.Parse(data[currentRow][kl[0]].ToString()) - 1) / data.Count;
        content.parent.parent.gameObject.GetComponent<ScrollRect>().verticalNormalizedPosition = 1 - (float)currentRow / data.Count;
        code.parent.parent.gameObject.GetComponent<ScrollRect>().verticalNormalizedPosition = 1 - (float)(Int32.Parse(data[currentRow][kl[0]].ToString()) - 1) / data.Count;

        

        ColorCurrent();
        if (currentRow-1 < 0)
        {
            prior.interactable = false;
            //각 버튼비활성화
            return;
        }
    }
    public void ColorCurrent()
    {
        for(int i=0; i < diffCnt; i++)
        {
            keyContent.GetChild(i).GetComponent<Image>().color = Color.red;
        }

        if(priorRow != -1)
        {
            for (int j = 0; j < data[0].Count - 3; j++)
            {
                content.GetChild(priorRow * (data[0].Count - 3) + j).GetComponent<Image>().color = Color.white;
            }
            for (int j = 0; j < 2; j++)
            {
                info.GetChild(priorRow * (2) + j).GetComponent<Image>().color = Color.white;
            }
            line.GetChild(priorRow).GetComponent<Image>().color = Color.white;
            line2.GetChild(Int32.Parse(data[priorRow][kl[0]].ToString()) - 1).GetComponent<Image>().color = Color.white;
            code.GetChild(Int32.Parse(data[priorRow][kl[0]].ToString()) - 1).GetComponent<Image>().color = Color.white;
            step.GetChild(priorRow).GetComponent<Image>().color = Color.white;
        }
       

        
        for (int j = 0; j < data[0].Count-3; j++)
        {
            content.GetChild(currentRow*(data[0].Count - 3)+j).GetComponent<Image>().color = Color.red;
        }
        for (int j = 0; j < 2; j++)
        {
            info.GetChild(currentRow * (2) + j).GetComponent<Image>().color = Color.red;
        }
        line.GetChild(currentRow).GetComponent<Image>().color = Color.red;
        line2.GetChild(Int32.Parse(data[currentRow][kl[0]].ToString()) - 1).GetComponent<Image>().color = Color.red;
        code.GetChild(Int32.Parse(data[currentRow][kl[0]].ToString())-1).GetComponent<Image>().color = Color.red;
        step.GetChild(currentRow).GetComponent<Image>().color = Color.red;
        priorRow = currentRow;
    }

    public void TextLoard()
    {
        //ShowText();

        data = CSVAssetReader.Read(myTxt);  // myTxt 에 등록한 TextAsset을 바로 읽게 합니다. Resource 폴더관계 없이. . 
        table.constraintCount = data[0].Count-3;
        keyTable.constraintCount = data[0].Count-3;
        kl = new List<string>(data[0].Keys);
        kl2 = new List<string>(data[0].Keys);
        kl2.RemoveAt(0);
        kl2.RemoveAt(0);
        kl2.RemoveAt(0);
        //col head
        for (int j = 3; j < data[0].Count; j++)
        {
            Image temp = Image.Instantiate(originalHead, keyContent);
            temp.transform.GetChild(0).GetComponent<Text>().text = kl[j].ToString();
            int c = j;
            temp.GetComponent<Button>().onClick.AddListener(delegate { DetailTable(kl[c]); });

        }

        for (int i = 0; i < data.Count; i++)
        {
            for (int j = 3; j < data[0].Count; j++)
            {
                Image temp = Image.Instantiate(original, content);
                temp.transform.GetChild(0).GetComponent<Text>().text = data[i][kl[j]].ToString();
                int a = i;
                string b = kl[j];
                temp.GetComponent<Button>().onClick.AddListener(delegate { ShowValue(a, b); });

            }
        }
        //row head
        for (int i = 0; i < data.Count; i++)
        {
            Image temp = Image.Instantiate(original2, line);
            temp.transform.GetChild(0).GetComponent<Text>().text = data[i][kl[0]].ToString();
        }
        //step
        for (int i = 0; i < data.Count; i++)
        {
            Image temp = Image.Instantiate(original2, step);
            temp.transform.GetChild(0).GetComponent<Text>().text = (i+1).ToString();
        }
        //info
        for (int i = 0; i < data.Count; i++)
        {
            for (int j = 1; j < 3; j++)
            {
                Image temp = Image.Instantiate(originalInfo, info);
                temp.transform.GetChild(0).GetComponent<Text>().text = data[i][kl[j]].ToString();
            }
        }
    }

    public void FindFildData()  // 찾는 데이터의 번호 / 오브젝트로 찾기
    {
        lineInt = Int32.Parse(idFild.text);  // 입력받은 필드를 int형으로 변경
        ofString = objectFild.text;  //입력 받은 필드의 텍스쳐를 사용

        if (lineInt < data.Count)
        {
            findFildText.text = data[lineInt][ofString].ToString();   // _myData = data[2]["Name"].ToString(); 앞의 이 코드를 대체

        }
        else
        {
            findFildText.text = "범위 밖입니다";
        }
    }

    //전체 텍스트 보는 스크립트 (앞의 코드)
    void ShowText()
    {
        if (myTxt != null)
        {
            string currentText = myTxt.text.Substring(0, myTxt.text.Length - 1);
            fildText.text = currentText;
        }
    }

}