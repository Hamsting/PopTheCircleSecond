using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.Game
{
    /// <summary>
    /// 게임의 박자 계산을 담당한다.
    /// </summary>
    public class BeatManager : Singleton<BeatManager>
    {
        /// <summary>
        /// 한 박자를 의미하는 단위이다.
        /// </summary>
        [SerializeField, InspectorReadOnly]
        private int bar = 0;
        /// <summary>
        /// Bar 박자 내 세부 박자를 의미하는 단위이다.
        /// </summary>
        [SerializeField, InspectorReadOnly]
        private float beat = 0.0f;
        /// <summary>
        /// 현재 경과 시간(초 단위)이다.
        /// </summary>
        [SerializeField, InspectorReadOnly]
        private float gameTime = 0.0f;
        /// <summary>
        /// 현재 박자의 공간적 위치 값이다.
        /// </summary>
        [SerializeField, InspectorReadOnly]
        private double position = 0.0f;
        /// <summary>
        /// position 값에 게임 스피드를 적용한 값이다.
        /// </summary>
        [SerializeField, InspectorReadOnly]
        private double positionWithSpeed = 0.0f;
        /// <summary>
        /// 화면에서 라인 끝의 위치에 해당하는 박자 위치이다.
        /// </summary>
        [SerializeField, InspectorReadOnly]
        private float railEndBarBeat = 0.0f;
        /// <summary>
        /// 분당 출력되는 박자 수 값이다.
        /// </summary>
        [SerializeField, InspectorReadOnly]
        private float bpm = 60.0f;
        /// <summary>
        /// 시작 시 속도 계산에서 기준으로 잡을 BPM 값이다.
        /// </summary>
        [SerializeField, InspectorReadOnly]
        private float standardBPM = 60.0f;
        /// <summary>
        /// 초당 beat 단위의 출력 수이다.
        /// </summary>
        [SerializeField, InspectorReadOnly]
        private float beatPerSecond = 60.0f / 60.0f * GlobalDefines.BeatPerBar;
        /// <summary>
        /// 게임의 속도 값이다. 높아질수록 노트의 이동 속도가 빨라진다.
        /// </summary>
        [SerializeField, Range(0.5f, 10.0f)]
        private float gameSpeed = 2.0f;
        /// <summary>
        /// BPM 정보들을 담는 리스트이다.
        /// </summary>
        [SerializeField, InspectorReadOnly]
        private List<BPMInfo> bpmInfos;
        /// <summary>
        /// 마지막으로 읽은 BPM 정보의 인덱스 값이다.
        /// </summary>
        [SerializeField, InspectorReadOnly]
        private int bpmInfoLastIndex = -1;
        /// <summary>
        /// 한 박자를 공간적 길이로 환산한 값이다.
        /// </summary>
        [SerializeField, InspectorReadOnly]
        private float barToRailLength = GlobalDefines.RailLength / GlobalDefines.DefaultBarCount * (60.0f / 60.0f) * 2.0f;
        /// <summary>
        /// 현재 정지 이펙트 적용의 여부를 나타낸다.
        /// </summary>
        private bool isStopEffect = false;
        /// <summary>
        /// 현재 정지 이펙트 적용의 여부를 나타낸다.
        /// </summary>
        private bool isSpeedRelatedToBPM = false;

        /// <summary>
        /// 한 박자를 의미하는 단위이다.
        /// </summary>
        public int Bar
        {
            get
            {
                return bar;
            }
        }
        /// <summary>
        /// Bar 박자 내 세부 박자를 의미하는 단위이다.
        /// </summary>
        public float Beat
        {
            get
            {
                return beat;
            }
        }
        /// <summary>
        /// 현재 박자의 공간적 위치 값이다.
        /// </summary>
        public double Position
        {
            get
            {
                return position;
            }
        }
        /// <summary>
        /// 현재 적용된 BPM 값이다.
        /// </summary>
        public float CurrentBPM
        {
            get
            {
                return bpm;
            }
            set
            {
                bpm = value;
                OnBPMChanged();
            }
        }
        /// <summary>
        ///  시작 시 속도 계산에서 기준으로 잡을 BPM 값이다.
        /// </summary>
        public float StandardBPM
        {
            get
            {
                return standardBPM;
            }
            set
            {
                standardBPM = value;
                if (standardBPM <= 0.0f)
                {
                    if (bpmInfos != null && bpmInfos.Count > 0)
                        standardBPM = Mathf.Abs(bpmInfos[0].bpm);
                    else
                        standardBPM = 60.0f;
                }

                if (!IsSpeedRelatedToBPM)
                {
                    float standardSpeed = UserSettings.gameSpeed * 60.0f;
                    GameSpeed = UserSettings.gameSpeed * (60.0f / standardBPM);
                }
            }
        }
        /// <summary>
        /// 게임의 속도 값이다. 높아질수록 노트의 이동 속도가 빨라진다.
        /// </summary>
        public float GameSpeed
        {
            get
            {
                return gameSpeed;
            }
            set
            {
                gameSpeed = value;
            }
        }
        /// <summary>
        /// 현재 경과 시간(초 단위)이다.
        /// </summary>
        public float GameTime
        {
            get
            {
                return gameTime;
            }
            set
            {
                GotoTime(value);
            }
        }
        /// <summary>
        /// 화면에서 라인 끝의 위치에 해당하는 박자 위치이다.
        /// </summary>
        public float RailEndBarBeat
        {
            get
            {
                return railEndBarBeat;
            }
        }
        /// <summary>
        /// 화면에서 라인 끝의 위치에 해당하는 박자 위치이다.
        /// </summary>
        public bool IsSpeedRelatedToBPM
        {
            get
            {
                return isSpeedRelatedToBPM;
            }
            set
            {
                isSpeedRelatedToBPM = value;
            }
        }



        /// <summary>
        /// Awake
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            bpmInfos = new List<BPMInfo>();

            GameSpeed = UserSettings.gameSpeed;
        }

        /// <summary>
        /// Start
        /// </summary>
        private void Start()
        {

        }

        /// <summary>
        /// Update
        /// </summary>
        private void Update()
        {
            UpdateBarAndBeat();
            UpdateBPM();
        }

        /// <summary>
        /// Bar와 Beat 변수를 업데이트한다.
        /// </summary>
        private void UpdateBarAndBeat()
        {
            gameTime += Time.deltaTime;
            float increasedBeat = beatPerSecond * Time.deltaTime;
            beat += increasedBeat;
            position += (increasedBeat / GlobalDefines.BeatPerBar) * barToRailLength;
            positionWithSpeed = position * gameSpeed;
            railEndBarBeat = PositionToBarBeat(position + (GlobalDefines.RailLength / gameSpeed));

            if (beat >= GlobalDefines.BeatPerBar)
            {
                bar += (int)beat / GlobalDefines.BeatPerBar;
                beat = beat % (float)GlobalDefines.BeatPerBar;
            }
        }

        /// <summary>
        /// BPM 정보의 변화를 체크하고 적용한다.
        /// </summary>
        private void UpdateBPM()
        {
            if (bpmInfos.Count - 1 > bpmInfoLastIndex)
            {
                BPMInfo nextInfo = bpmInfos[bpmInfoLastIndex + 1];
                float barDiff = GetBarDifference(bar, beat, nextInfo.bar, nextInfo.beat);
                if (barDiff >= 0.0f)
                {
                    float bpmDiffRatio = nextInfo.bpm / bpm;
                    float fixedBarDiff = barDiff * bpmDiffRatio;

                    bar = nextInfo.bar;
                    beat = 0.0f;
                    if (nextInfo.beat > 0.0f)
                        ++bar;

                    beat += fixedBarDiff * GlobalDefines.BeatPerBar;
                    if (beat >= GlobalDefines.BeatPerBar)
                    {
                        bar += (int)beat / GlobalDefines.BeatPerBar;
                        beat = beat % (float)GlobalDefines.BeatPerBar;
                    }

                    isStopEffect = nextInfo.stopEffect;
                    CurrentBPM = nextInfo.bpm;
                    gameTime = nextInfo.time;
                    position = nextInfo.position + fixedBarDiff * barToRailLength;
                    ++bpmInfoLastIndex;

                    Debug.Log("Actual Time : " + gameTime + ", BPMInfo Time : " + nextInfo.time + ", Diff : " + (gameTime - nextInfo.time));
                }
            }
        }

        /// <summary>
        /// BPM 값이 변경되었을 시 호출된다.
        /// </summary>
        private void OnBPMChanged()
        {
            beatPerSecond = Mathf.Abs(bpm) / 60.0f * GlobalDefines.BeatPerBar;

            if (isStopEffect)
                barToRailLength = 0.0f;
            else
                barToRailLength = GlobalDefines.RailLength / GlobalDefines.DefaultBarCount * (bpm / 60.0f);

            Debug.Log("BPM Changed : " + bpm + 
                      ", Position : " + position + 
                      ", Bar/Beat : " + bar + "/" + beat + 
                      ", Stop : " + isStopEffect);
        }

        /// <summary>
        /// 새로운 BPM 정보를 입력한다.
        /// </summary>
        public void AddNewBPMInfo(int _bar, float _beat, float _bpm, bool _stopEffect = false)
        {
            BPMInfo info = new BPMInfo()
            {
                bar = _bar,
                beat = _beat,
                bpm = _bpm,
                stopEffect = _stopEffect
            };

            BPMInfo lastInfo = null;
            for (int i = 0; i <= bpmInfos.Count; ++i)
            {
                if (i == bpmInfos.Count)
                {
                    bpmInfos.Add(info);
                    break;
                }
                else
                {
                    lastInfo = bpmInfos[i];
                    if (GetBarDifference(_bar, _beat, lastInfo.bar, lastInfo.beat) < 0.0f)
                    {
                        bpmInfos.Insert(i, info);
                        break;
                    }
                }
            }

            if (bpmInfos.Count > 1 && lastInfo != null)
            {
                double pivotPos = lastInfo.position;
                float pivotTime = lastInfo.time;
                
                int lastInfoStartBar = (lastInfo.beat == 0.0f) ? lastInfo.bar : lastInfo.bar + 1;
                float barDiff = GetBarDifference(_bar, _beat, lastInfoStartBar, 0.0f);

                if (lastInfo.stopEffect)
                    info.position = pivotPos;
                else
                    info.position = pivotPos + (double)(barDiff *
                            (GlobalDefines.RailLength / GlobalDefines.DefaultBarCount * (lastInfo.bpm / 60.0f)));
                info.time = pivotTime + barDiff / (Mathf.Abs(lastInfo.bpm) / 60.0f);
            }
            else
            {
                info.position = 0.0d;
                info.time = 0.0f;
            }
        }

        /// <summary>
        /// 박자를 공간적 위치의 값으로 변경한다.
        /// </summary>
        public double BarBeatToPosition(int _bar, float _beat = 0.0f)
        {
            for (int i = bpmInfos.Count - 1; i >= 0; --i)
            {
                BPMInfo info = bpmInfos[i];
                int infoStartBar = (info.beat == 0.0f) ? info.bar : info.bar + 1;
                if (infoStartBar <= _bar)
                {
                    if (info.stopEffect)
                        return info.position;
                    else
                        return info.position + GetBarDifference(_bar, _beat, infoStartBar, 0.0f) * 
                            (GlobalDefines.RailLength / GlobalDefines.DefaultBarCount * (info.bpm / 60.0f));
                }
            }

            return 0.0f;
        }

        /// <summary>
        /// 공간적 위치를 박자 위치로 변경한다.
        /// </summary>
        public float PositionToBarBeat(double _position)
        {
            for (int i = bpmInfos.Count - 1; i >= 0; --i)
            {
                BPMInfo info = bpmInfos[i];
                if (info.position <= _position)
                {
                    double positionDiff = _position - info.position;
                    return ToBarBeat(info.bar, info.beat) + 
                        ((float)positionDiff / (GlobalDefines.RailLength / GlobalDefines.DefaultBarCount * (info.bpm / 60.0f)));
                }
            }

            return 0.0f;
        }

        /// <summary>
        /// 두 박자값 간의 차이를 계산한다.
        /// </summary>
        public static float GetBarDifference(int _bar1, float _beat1, int _bar2, float _beat2)
        {
            return ((float)_bar1 + _beat1 / GlobalDefines.BeatPerBar) -
                   ((float)_bar2 + _beat2 / GlobalDefines.BeatPerBar);
        }

        /// <summary>
        /// 특정 박자 위치에서 적용되는 BPM 정보를 가져온다.
        /// </summary>
        public float GetBPMWithBarBeat(int _bar, float _beat = 0.0f)
        {
            for (int i = bpmInfos.Count - 1; i >= 0; --i)
            {
                BPMInfo info = bpmInfos[i];
                int infoStartBar = (info.beat == 0.0f) ? info.bar : info.bar + 1;
                if (infoStartBar <= _bar)
                {
                    return info.bpm; 
                }
            }
            return 60.0f;
        }

        /// <summary>
        /// 박자를 시간값(초 단위)으로 변경한다.
        /// </summary>
        public float BarBeatToTime(int _bar, float _beat)
        {
            for (int i = bpmInfos.Count - 1; i >= 0; --i)
            {
                BPMInfo info = bpmInfos[i];
                int infoStartBar = (info.beat == 0.0f) ? info.bar : info.bar + 1;
                if (infoStartBar <= _bar)
                {
                    return info.time + GetBarDifference(_bar, _beat, infoStartBar, 0.0f) / (info.bpm / 60.0f);
                }
            }

            return 0.0f;
        }
        
        /// <summary>
        /// 박자를 시간값(초 단위)으로 변경한다.
        /// </summary>
        public float BarBeatToTime(float _barBeat)
        {
            float beat = (_barBeat - Mathf.Ceil(_barBeat)) / GlobalDefines.BeatPerBar;
            return BarBeatToTime((int)_barBeat, beat);
        }

        public static float ToBarBeat(int _bar, float _beat = 0.0f)
        {
            return (float)_bar + (_beat / GlobalDefines.BeatPerBar);
        }

        /// <summary>
        /// 특정 박자가 유효한 값의 박자인지 체크한다.
        /// </summary>
        public bool IsPossibleBarBeat(int _bar, float _beat)
        {
            foreach (BPMInfo info in bpmInfos)
            {
                if (info.bar > _bar)
                    return true;
                else if (info.bar != _bar)
                    continue;

                if (info.beat != 0.0f && info.beat <= _beat)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 특정 박자비트가 유효한지 체크하고, 정정한다.
        /// </summary>
        public float CorrectBarBeat(float _barBeat)
        {
            int bar = (int)_barBeat;
            float beat = (_barBeat - bar) * GlobalDefines.BeatPerBar;

            foreach (BPMInfo info in bpmInfos)
            {
                if (info.bar > bar)
                    return _barBeat;
                else if (info.bar != bar)
                    continue;

                if (info.beat != 0.0f && info.beat <= beat)
                {
                    // 임시 작성, 계산식 수정 필요 (bpm 변경에 따른 보정)
                    float modifiedBeat = beat - info.beat;

                    float corrected = (float)bar + 1.0f + (modifiedBeat / GlobalDefines.BeatPerBar);
                    return corrected;
                }
            }
            return _barBeat;
        }

        /// <summary>
        /// 처음 시작 시의 대기 시간으로 이동한다.
        /// </summary>
        public void GotoStartDelayTime(float _time)
        {
            gameTime = _time;

            bpmInfoLastIndex = -1;
            BPMInfo bpmInfo = bpmInfos[0];
            isStopEffect = bpmInfo.stopEffect;
            CurrentBPM = bpmInfo.bpm;

            float timeDiff = _time;
            float barBeatDiff = timeDiff * (bpmInfo.bpm / 60.0f);

            position = bpmInfo.position + barBeatDiff * barToRailLength;
            bar = ((bpmInfo.beat == 0.0f) ? bpmInfo.bar : bpmInfo.bar + 1) + (int)barBeatDiff;
            beat = (barBeatDiff - (int)barBeatDiff) * (float)GlobalDefines.BeatPerBar;
        }
        
        /// <summary>
        /// 현재 시간 위치를 특정 시간으로 이동한다.
        /// </summary>
        public void GotoTime(float _time)
        {
            gameTime = Mathf.Clamp(_time, 0.0f, MusicManager.Instance.MusicLength);

            bpmInfoLastIndex = 0;
            for (int i = bpmInfos.Count - 1; i >= 0; --i)
            {
                if (bpmInfos[i].time <= _time)
                {
                    bpmInfoLastIndex = i;
                    break;
                }
            }
            BPMInfo bpmInfo = bpmInfos[bpmInfoLastIndex];
            isStopEffect = bpmInfo.stopEffect;
            CurrentBPM = bpmInfo.bpm;

            float timeDiff = _time - bpmInfos[bpmInfoLastIndex].time;
            float barBeatDiff = timeDiff * (bpmInfo.bpm / 60.0f);

            position = bpmInfo.position + barBeatDiff * barToRailLength;
            bar = ((bpmInfo.beat == 0.0f) ? bpmInfo.bar : bpmInfo.bar + 1) + (int)barBeatDiff;
            beat = (barBeatDiff - (int)barBeatDiff) * (float)GlobalDefines.BeatPerBar;
        }
    }
}