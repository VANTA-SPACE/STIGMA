using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Core.Level;
using Serialization;

public class LevelSelect : MonoBehaviour
{
    static LevelSelect _;
    public static LevelSelect instance => _ ??= FindObjectOfType<LevelSelect>();
    ScrollRect rect;
    public List<RectTransform> objs = new List<RectTransform>();
    public GameObject TestObj;
    public List<LevelData> datas = new List<LevelData>();
    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<ScrollRect>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Init()
    {
        foreach (TextAsset ass in Resources.LoadAll<TextAsset>("Levels/"))
        {
            datas.Add(new LevelData((Dictionary<string, object>)Json.Deserialize(ass.text)));
        }
        for (int i = 0; i < datas.Count; i++)
        {
            
        }
    }
    float space = 20;
    public void Add(GameObject obj)
    {
        var @new = Instantiate(obj, rect.content).GetComponent<RectTransform>();
        objs.Add(@new);
        float s = 0;
        for (int i = 0; i < objs.Count; i++)
        {
            objs[i].anchoredPosition = new Vector2(10f, -s);
            s += objs[i].sizeDelta.y + space;
        }
        rect.content.sizeDelta = new Vector2(rect.content.sizeDelta.x, s);
    }
}
