using System;
using Locale;
using TMPro;
using UnityEngine;
using Utils;

namespace UI {
    public class VersionText : MonoBehaviour {
        public enum VersionType {
            PreAlpha,
            Alpha,
            Beta,
            ReleaseCandidate,
            Release
        }

        public VersionType versionType;
        public int majorVer;
        public int minorVer;
        public int patchVer;
        public string metadata;
        public int releaseNum;

        private void Start() {
            StartCoroutine(StigmaUtils.SetUntil(() => {
                UpdateText();
                Events.OnLanguageChange.AddListener(UpdateText);
            }, () => StigmaStartup.StartedUp));
        }

        private void UpdateText() {
            if (patchVer == 0) {
                
                switch (versionType) {
                    case VersionType.PreAlpha:
                    case VersionType.Alpha:
                    case VersionType.Beta:
                        var min = minorVer.ToString();
                        var min2 = min.Substring(0, min.Length - 1);
                        var min3 = min[min.Length - 1].ToString();
                        GetComponent<TMP_Text>().text =
                            $"{Translate.Get(versionType.ToString())}<mspace=0.5em> {majorVer}.{min2}</mspace>"
                            + $"{min3}{versionType.ToString().ToLower()[0]}{metadata}<mspace=0.5em> (r{releaseNum})</mspace>";
                        break;
                    case VersionType.ReleaseCandidate:
                        GetComponent<TMP_Text>().text =
                            $"<mspace=0.5em>{majorVer}.{minorVer}</mspace>"
                            + $"{metadata} RC<mspace=0.5em>{releaseNum}</mspace>";
                        break;
                    case VersionType.Release:
                        #if UNITY_EDITOR
                        GetComponent<TMP_Text>().text =
                            $"{Translate.Get(versionType.ToString())}<mspace=0.5em> {majorVer}.{minorVer}</mspace>{metadata}";
                        #endif
                        break;
                }
                return;
            }
            switch (versionType) {
                case VersionType.PreAlpha:
                case VersionType.Alpha:
                case VersionType.Beta:
                    var pat = patchVer.ToString();
                    var pat2 = pat.Substring(0, pat.Length - 1);
                    var pat3 = pat[pat.Length - 1].ToString();
                    GetComponent<TMP_Text>().text =
                        $"{Translate.Get(versionType.ToString())}<mspace=0.5em> {majorVer}.{minorVer}.{pat2}</mspace>"
                        + $"{pat3}{versionType.ToString().ToLower()[0]}{metadata}<mspace=0.5em> (r{releaseNum})</mspace>";
                    break;
                case VersionType.ReleaseCandidate:
                    GetComponent<TMP_Text>().text =
                        $"<mspace=0.5em>{majorVer}.{minorVer}.{patchVer}</mspace>"
                        + $"{metadata} RC<mspace=0.5em>{releaseNum}</mspace>";
                    break;
                case VersionType.Release:
                    #if UNITY_EDITOR
                    GetComponent<TMP_Text>().text =
                        $"{Translate.Get(versionType.ToString())}<mspace=0.5em> {majorVer}.{minorVer}.{patchVer}</mspace>{metadata}";
                    #endif
                    break;
            }
        }
    }
}