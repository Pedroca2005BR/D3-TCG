using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Pedroca2005BR.Utilities
{
    public class Stopwatch : MonoBehaviour
    {
        [Header("Reference to TextUI")]
        [SerializeField] private TextMeshProUGUI _textMeshProUGUI;

        [Header("Basic Settings")]
        [SerializeField] private bool _isCountdown;
        [SerializeField] protected float _startTime;

        [Header("Limit Settings")]
        [SerializeField] private bool _hasLimit;
        [SerializeField] protected float _stopTime;
        [SerializeField] Action eventToShoot;
        private bool _hasEventBeenShot = false;

        [Header("Display Settings")]
        [SerializeField] private TimerFormat _timerFormat;

        protected float _currentTime = 0f;
        private int _hours;
        private int _minutes;
        private int _seconds;


        #region Singleton

        public static Stopwatch instance;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this);
            }
            else
            {
                instance = this;
            }
        }

        #endregion


        private void Start()
        {
            _currentTime = _startTime;
        }

        private void Update()
        {
            if (_isCountdown)
            {
                CountDownUpdate();
            }
            else
            {
                CountUpUpdate();
            }
        }

        //CountDown and CountUp ticks the timer 
        private void CountDownUpdate()
        {
            _currentTime -= Time.deltaTime;

            if (_hasLimit && _currentTime <= _stopTime)
            {
                _currentTime = _stopTime;

                TryShootEvent();
            }

            SetTimerText();
        }
        private void CountUpUpdate()
        {
            _currentTime += Time.deltaTime;

            if (_hasLimit && _currentTime >= _stopTime)
            {
                _currentTime = _stopTime;

                TryShootEvent();
            }

            SetTimerText();
        }

        //SetTimerText displays the timer according to the TimerFormat
        private void SetTimerText()
        {
            switch (_timerFormat)
            {
                case TimerFormat.FullSeconds:
                    _textMeshProUGUI.text = _currentTime.ToString("0"); break;
                case TimerFormat.DecimalSeconds:
                    _textMeshProUGUI.text = _currentTime.ToString("0.0"); break;
                case TimerFormat.HundredthSeconds:
                    _textMeshProUGUI.text = _currentTime.ToString("0.00"); break;
                case TimerFormat.MinutesSeconds:
                    _minutes = (int)_currentTime / 60;
                    _seconds = (int)_currentTime % 60;
                    _textMeshProUGUI.text = _minutes.ToString("00") + ":" + _seconds.ToString("00"); break;
                case TimerFormat.MinutesHundrethSeconds:
                    _minutes = (int)_currentTime / 60;
                    _textMeshProUGUI.text = _minutes.ToString("00") + ":" + _currentTime.ToString("00.00"); break;
                case TimerFormat.HoursSeconds:
                    _seconds = (int)_currentTime % 60;
                    _minutes = (int)_currentTime / 60 - _hours * 60;
                    _hours = (int)_currentTime / 3600;
                    _textMeshProUGUI.text = _hours.ToString("00") + ":" + _minutes.ToString("00") + ":" + _seconds.ToString("00"); break;
                default:
                    Debug.LogError("Stopwatch can't find a format to display!");
                    break;
            }
        }

        //Resets the timer, but does not make it stop ticking
        public void ResetTimer()
        {
            _currentTime = _startTime;
            _hasEventBeenShot = false;
        }

        //WaitAtZero makes the timer tick stop at 0 and wait for the ForceContinue method to be called
        public void WaitAtZero()
        {
            ResetTimer();
            ForceStopTimer();
        }

        //ForceStop stops the timer tick where it is at the moment of calling
        public void ForceStopTimer()
        {
            _hasLimit = true;
            _stopTime = _currentTime;
        }

        //ForceContinueTimer is used to make the timer tick again, if stopped
        public void ForceContinueTimer()
        {
            _hasLimit = false;
        }

        //ForceContinueTimerUntil can be used as a ForceContinueTimer, but creates a limit point as well
        public void ForceContinueTimerUntil(float stopAt)
        {
            _hasLimit = true;
            _stopTime = stopAt;
        }

        //Method used for turning a count down timer into a count up timer and vice versa
        public void ChangeTimerDirection(float? newLimit = null)
        {
            if (newLimit == null)
            {
                _hasLimit = false;
            }
            else
            {
                _hasLimit = true;
                _stopTime = newLimit.Value;
            }


            if (_isCountdown)
            {
                _isCountdown = false;
            }
            else
            {
                _isCountdown = true;
            }
        }

        private void TryShootEvent()
        {
            if (eventToShoot != null && !_hasEventBeenShot)
            {
                eventToShoot.Invoke();
                _hasEventBeenShot = true;
            }
        }

        public void SetEvent(Action newEvent)
        {
            eventToShoot = newEvent;
            _hasEventBeenShot = false;
        }

        public void SetCountDown()
        {
            _isCountdown = true;
        }

        public int GetTime()
        {
            return (int)_currentTime;
        }
    }

    //Enum used for setting the display type of the timer
    public enum TimerFormat
    {
        FullSeconds,
        DecimalSeconds,
        HundredthSeconds,
        MinutesSeconds,
        MinutesHundrethSeconds,
        HoursSeconds
    }
}
