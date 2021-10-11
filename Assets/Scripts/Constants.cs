// ReSharper disable InconsistentNaming

using System.IO;
using Core;
using UnityEngine;
using Utils;

public static class Constants {
    //씬
    public const string STARTUP_SCENE = "SceneStartup";
    public const string INTRO_SCENE = "SceneIntro";
    public const string PLAY_SCENE = "PlayScene";
    public const string LEVEL_SELECT_SCENE = "LevelSelection";
    
    //이벤트
    public const string TitleEvent = "event:/Title";
    
    // 각 노트별 판정 (60프레임 기준)
    // 기본 설정 : 8 / 16 / 24 / 6
    public const double NOTEJUDGMENT_PERFECT = 2.5;
    public const double NOTEJUDGMENT_NORMAL = 6;
    public const double NOTEJUDGMENT_BAD = 10;
    public const double NOTEJUDGMENT_ELOFFSET = 1;

    public const float PERFECT_TOTALGAUGE = 50f;
    public const float PERFECTEL_TOTALGAUGE = 30f;
    public const float BAD_TOTALGAUGE = -80f;
    public const float MISS_TOTALGAUGE = -150f;
    
    public const float GOOD_FIXEDGAUGE = 0.5f;

    public const float GAUGE_RESULT_F = 40f;
    
    //노트 너비
    public const float NOTE_WIDTH = 1.6f;
    
    //노트 속도
    public const float NOTE_SPEED_MODIFIER = 2f;
    

    public static readonly ConstDict<Judgment, Color> JudgmentColors = new ConstDict<Judgment, Color>(
        (Judgment.PerfectEarly, new Color(1f, 1f, 0.6f)),
        (Judgment.PerfectLate, new Color(1f, 1f, 0.6f)),
        (Judgment.Good, new Color(0.4f, 0.8f, 1f)),
        (Judgment.GoodEarly, new Color(0.5f, 0.6f, 0.7f)),
        (Judgment.GoodLate, new Color(0.5f, 0.6f, 0.7f)),
        (Judgment.Bad, new Color(0.8f, 0.2f, 0.1f)),
        (Judgment.Miss, new Color(0.6f, 0.6f, 0.6f))
    );

    
    public static string DataPath => Application.persistentDataPath;
    public static string ResourcePath => Path.Combine(Application.dataPath, "Resources");
}