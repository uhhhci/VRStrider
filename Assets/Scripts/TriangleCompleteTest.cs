using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TriangleCompleteTest : MonoBehaviour
{
    public int _participantID = 0;
    private int _playerPrefsParticipantID = 0;
    public bool _separateMethodExport = true;
    public Transform _startPoint;
    public Transform _firstPoint;
    public Transform _secondPoint;

    public Transform _playerTransform;
    public CharacterController _playerController;

    public float _distance;
    public float _angle;
    private float _time;

    public List<TriangleCompleteTestCondition> _sortedConditions;
    public List<TriangleCompleteTestCondition> _randomizedConditions;
    public int _currentConditionIndex;

    private int _repititions = 1;
    private float[] _distanceLevels1 = { 10 };
    private float[] _distanceLevels2 = { 5, 10 };
    private float[] _angleLevels = { 60, 90, 120 };
    public string[] _methodLevels = { "Joystick", "Bike_Feedback_Off", "Bike_Feedback_On", "Teleport" };

    private bool _experimentRunning = false;
    public AudioSource _audioInput;
    public AudioSource _audioFinish;
    private string _lastMethodLevelName = "";
    private float _trialStartTime;
    public bool _debug = false;
    public Transform _environment;

    // Start is called before the first frame update
    void Start()
    {
        _participantID = PlayerPrefs.GetInt("participantNumberTriangleComplete");
        _playerPrefsParticipantID = PlayerPrefs.GetInt("participantNumberTriangleComplete");
        StartExperiment();
    }

    private void StartExperiment()
    {
        print("Starting Triangle Completion Experiment. Last used participant ID #" + PlayerPrefs.GetInt("participantNumberTriangleComplete"));
        _experimentRunning = true;
        GenerateConditionList();
        StartTrial();
        StartCoroutine(InitiateBike());
    }

    // Update is called once per frame
    void Update()
    {
        if (_participantID != _playerPrefsParticipantID)
        {
            print("Participant ID # changed from " + _playerPrefsParticipantID + " to " + _participantID);
            PlayerPrefs.SetInt("participantNumberTriangleComplete", _participantID);
            _playerPrefsParticipantID = _participantID;
        }

        if ((_firstPoint.transform.position - _playerTransform.transform.position).magnitude < 0.75f)
        {
            HideFirstPoint();
        }
        if ((_secondPoint.transform.position - _playerTransform.transform.position).magnitude < 0.75f)
        {
            HideSecondPoint();
        }


        CalculateDistanceFromPlayerToEndPoint();
        CalculateAngleFromPlayerToEndPoint();


        if (Input.GetKeyDown(KeyCode.Return))
        {
            UserInput();
        }
        if (Input.GetButtonDown("Controller0") || Input.GetButtonDown("Controller1") || Input.GetButtonDown("Controller2") || Input.GetButtonDown("Controller3"))
        {
            UserInput();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            ExportTable(_separateMethodExport);
        }
    }

    public void UserInput()
    {
        //if both vertices have been visited
        if (_debug || (!_firstPoint.gameObject.activeInHierarchy && !_secondPoint.gameObject.activeInHierarchy))
        {
            //if side button is pushed end trial
            if (_experimentRunning)
            {
                EndTrial();
            }
        }
    }

    private void EndTrial()
    {
        _time = Time.time - _trialStartTime;
        _firstPoint.gameObject.SetActive(false);
        _secondPoint.gameObject.SetActive(false);
        SaveValuesForTrial();
        _currentConditionIndex++;

        if (_currentConditionIndex < _randomizedConditions.Count)
        {
            _audioInput.Play();
            StartTrial();
        }
        else if (_currentConditionIndex == _randomizedConditions.Count)
        {
            _audioFinish.Play();
            _experimentRunning = false;
            ExportTable(_separateMethodExport);
            _playerController.ShowTextPrompt("Done!");
        }
    }

    private void StartTrial()
    {
        _environment.position = new Vector3(_playerController.transform.position.x, 0, _playerController.transform.position.z);

        _firstPoint.gameObject.SetActive(true);
        _secondPoint.gameObject.SetActive(true);

        _secondPoint.SetParent(_firstPoint);

        _startPoint.position = _playerTransform.position;
        _startPoint.rotation = _playerTransform.rotation;

        float distancePoint1 = _randomizedConditions[_currentConditionIndex]._distanceToPoint1;
        float distancePoint2 = _randomizedConditions[_currentConditionIndex]._distanceToPoint2;
        float angle = _randomizedConditions[_currentConditionIndex]._angleToPoint2;

        //randomize left or right turn
        float direction = Random.Range(0, 1f);
        if (direction < 0.5f)
        {
            angle *= -1;
        }

        _firstPoint.localPosition = new Vector3(0, 0, distancePoint1);
        _firstPoint.localEulerAngles = new Vector3(0, angle, 0);
        _secondPoint.localPosition = new Vector3(0, 0, distancePoint2);

        _secondPoint.SetParent(_startPoint);

        _trialStartTime = Time.time;

        _playerController._bikeControlEnabled = false;
        _playerController._joystickControlEnabled = false;
        _playerController._teleportControlEnabled = false;

        activateControlMethod();

        if (_randomizedConditions[_currentConditionIndex]._method == "Bike_Feedback_On" || _randomizedConditions[_currentConditionIndex]._method == "Bike_Feedback_Off")
        {
            _playerController.ShowAvatar(true);
        }
        else
        {
            _playerController.ShowAvatar(false);
        }

        if (_randomizedConditions[_currentConditionIndex]._method != _lastMethodLevelName)
        {
            //_playerController.ShowTextPrompt("Input Method:\n" + _randomizedConditions[_currentConditionIndex]._method);
            print("New Input Method: " + _randomizedConditions[_currentConditionIndex]._method);
            _lastMethodLevelName = _randomizedConditions[_currentConditionIndex]._method;
        }
    }

    private void activateControlMethod()
    {
        _playerController._usePressureTurning = false;
        _playerController._useTouchpadTurning = false;

        if (_randomizedConditions[_currentConditionIndex]._method == "Bike_Feedback_On")
        {
            _playerController._bikeControlEnabled = true;
            _playerController._vibroFeedbackEnabled = true;
            _playerController._useIK = true;
        }
        else if (_randomizedConditions[_currentConditionIndex]._method == "Bike_Feedback_Off")
        {
            _playerController._bikeControlEnabled = true;
            _playerController._vibroFeedbackEnabled = false;
            _playerController._useIK = true;
        }
        else if (_randomizedConditions[_currentConditionIndex]._method == "Joystick")
        {
            _playerController._joystickControlEnabled = true;
            _playerController._vibroFeedbackEnabled = false;
            _playerController._useIK = false;
        }
        else if (_randomizedConditions[_currentConditionIndex]._method == "Teleport")
        {
            _playerController._teleportControlEnabled = true;
            _playerController._vibroFeedbackEnabled = false;
            _playerController._useIK = false;
        }
    }

    private IEnumerator InitiateBike()
    {
        _playerController._bikeControlEnabled = true;
        yield return new WaitForSeconds(0.1f);
        _playerController._bikeControlEnabled = false;
        activateControlMethod();

    }

    private void HideFirstPoint()
    {
        _firstPoint.gameObject.SetActive(false);
    }
    private void HideSecondPoint()
    {
        _secondPoint.gameObject.SetActive(false);
    }

    private void CalculateDistanceFromPlayerToEndPoint()
    {
        float correctDistance = (_secondPoint.transform.position - _startPoint.transform.position).magnitude;
        float currentDistance = (_secondPoint.transform.position - _playerTransform.transform.position).magnitude;

        _distance = correctDistance - currentDistance;
    }

    private void CalculateAngleFromPlayerToEndPoint()
    {
        Vector3 target = _startPoint.position - _secondPoint.position;
        target.y = 0;
        Vector3 player = _playerTransform.position - _secondPoint.position;
        player.y = 0;

        _angle = Vector3.SignedAngle(target, player, Vector3.up);
    }

    private void SaveValuesForTrial()
    {
        _randomizedConditions[_currentConditionIndex]._recordedAngle = _angle;
        _randomizedConditions[_currentConditionIndex]._recordedDistance = _distance;
        _randomizedConditions[_currentConditionIndex]._recordedTime = _time;
        print("Recorded values for " + _randomizedConditions[_currentConditionIndex]._method + " trial " + (_currentConditionIndex + 1) + " of overall " + _randomizedConditions.Count + ". Distance: " + _distance + " / Angle: " + _angle + " / Time: " + _time);
    }

    public class TriangleCompleteTestCondition
    {
        public string _method;
        public float _distanceToPoint1;
        public float _distanceToPoint2;
        public float _angleToPoint2;
        public float _recordedDistance;
        public float _recordedAngle;
        public float _recordedTime;

        public TriangleCompleteTestCondition(string method, float distanceToPoint1, float distanceToPoint2, float angleToPoint2)
        {
            _method = method;
            _distanceToPoint1 = distanceToPoint1;
            _distanceToPoint2 = distanceToPoint2;
            _angleToPoint2 = angleToPoint2;
        }
    }

    //Export Table as .txt, Input, Accuracy, Order
    private void ExportTable(bool separateMethods)
    {
        int participantNumber = PlayerPrefs.GetInt("participantNumberTriangleComplete");
        //PlayerPrefs.SetInt("participantNumberTriangleComplete", participantNumber + 1);

        if (separateMethods)
        {
            for (int m = 0; m < _methodLevels.Length; m++)
            {
                int sectionLength = _distanceLevels1.Length * _distanceLevels2.Length * _angleLevels.Length * _repititions;

                string printText = "";

                for (int e = 0; e < sectionLength; e++)
                {
                    int i = m * sectionLength + e;

                    printText += participantNumber + ";" + _sortedConditions[i]._method + ";" + _sortedConditions[i]._distanceToPoint1 + ";" + _sortedConditions[i]._distanceToPoint2 + ";" + _sortedConditions[i]._angleToPoint2 + ";" + _sortedConditions[i]._recordedDistance + ";" + _sortedConditions[i]._recordedAngle + ";" + _sortedConditions[i]._recordedTime + "\n";

                }


                File.WriteAllText("ExperimentData/TriangleComplete/" + participantNumber + "_" + _methodLevels[m] + ".txt", printText);

                print("Table exported at " + "ExperimentData/TriangleComplete/" + participantNumber + "_" + _methodLevels[m] + ".txt");
            }
        }
        else
        {
            string printText = "";

            for (int i = 0; i < _sortedConditions.Count; i++)
            {
                printText += participantNumber + ";" + _sortedConditions[i]._method + ";" + _sortedConditions[i]._distanceToPoint1 + ";" + _sortedConditions[i]._distanceToPoint2 + ";" + _sortedConditions[i]._angleToPoint2 + ";" + _sortedConditions[i]._recordedDistance + ";" + _sortedConditions[i]._recordedAngle + ";" + _sortedConditions[i]._recordedTime + "\n";
            }


            File.WriteAllText("ExperimentData/TriangleComplete/" + participantNumber + ".txt", printText);

            print("Experiment finished, table exported at " + "ExperimentData/TriangleComplete/" + participantNumber + ".txt");
        }

    }

    private void GenerateConditionList()
    {
        _sortedConditions = new List<TriangleCompleteTestCondition>();
        _randomizedConditions = new List<TriangleCompleteTestCondition>();
        for (int m = 0; m < _methodLevels.Length; m++)
        {
            for (int i = 0; i < _distanceLevels1.Length; i++)
            {
                for (int j = 0; j < _distanceLevels2.Length; j++)
                {
                    for (int k = 0; k < _angleLevels.Length; k++)
                    {
                        for (int r = 0; r < _repititions; r++)
                        {

                            string method = _methodLevels[m];
                            float distanceLevel1 = _distanceLevels1[i];
                            float distanceLevel2 = _distanceLevels2[j];
                            float angle = _angleLevels[k];

                            _sortedConditions.Add(new TriangleCompleteTestCondition(method, distanceLevel1, distanceLevel2, angle));
                        }
                    }
                }
            }
        }

        ShuffleConditions();
    }

    private void ShuffleConditions()
    {
        for (int i = 0; i < _sortedConditions.Count; i++)
        {
            _randomizedConditions.Add(_sortedConditions[i]);
        }

        int sectionLength = _distanceLevels1.Length * _distanceLevels2.Length * _angleLevels.Length * _repititions;

        //shuffle elements in sections
        for (int i = 0; i < _methodLevels.Length; i++)
        {
            for (int t = i * sectionLength; t < (i + 1) * sectionLength; t++)
            {
                TriangleCompleteTestCondition tmp = _randomizedConditions[t];
                int r = Random.Range(t, (i + 1) * sectionLength);
                _randomizedConditions[t] = _randomizedConditions[r];
                _randomizedConditions[r] = tmp;
            }
        }

        //Shuffle sections
        for (int m = 0; m < _methodLevels.Length; m++)
        {
            int r = Random.Range(0, _methodLevels.Length);

            for (int e = 0; e < sectionLength; e++)
            {
                int index1 = m * sectionLength + e;
                int index2 = r * sectionLength + e;

                TriangleCompleteTestCondition tmp = _randomizedConditions[index1];
                _randomizedConditions[index1] = _randomizedConditions[index2];
                _randomizedConditions[index2] = tmp;
            }
        }
    }
}
