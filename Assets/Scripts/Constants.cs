// ReSharper disable InconsistentNaming

using Core;
using UnityEngine;
using Utils;

public static class Constants {
    // 각 노트별 판정 (60프레임)
    public const double NOTEJUDGMENT_PERFECT = 2.5;
    public const double NOTEJUDGMENT_NORMAL = 8;
    public const double NOTEJUDGMENT_BAD = 15;
    public const double NOTEJUDGMENT_ELOFFSET = 2;
    
    //노트 너비
    public const float NOTE_WIDTH = 3;
    
    //노트 속도
    public const float NOTE_SPEED_MODIFIER = 4;
    
    public const string TitleEvent = "event:/Title";

    public static readonly ConstDict<Judgment, Color> JudgmentColors = new ConstDict<Judgment, Color>(
        (Judgment.PerfectEarly, new Color(1f, 1f, 0.6f)),
        (Judgment.PerfectLate, new Color(1f, 1f, 0.6f)),
        (Judgment.Good, new Color(0.4f, 0.8f, 1f)),
        (Judgment.GoodEarly, new Color(0.5f, 0.6f, 0.7f)),
        (Judgment.GoodLate, new Color(0.5f, 0.6f, 0.7f)),
        (Judgment.Bad, new Color(0.8f, 0.2f, 0.1f)),
        (Judgment.Miss, new Color(0.6f, 0.6f, 0.6f))
    );
}