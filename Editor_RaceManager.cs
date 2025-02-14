using UnityEngine;
using System.Collections;
using UnityEditor;
using RGSK;

[CustomEditor(typeof(RaceManager))]
public class Editor_RaceManager : Editor
{
    RaceManager _target;
    string[] toolbarTabs = { "Race Settings", "Player Settings", "AI Settings" };
    GUIStyle centerLabelStyle;
    int editorTab;

    //Race Settings - Enums
    SerializedProperty raceType;
    SerializedProperty startMode;
    SerializedProperty playerGridPositioningMode;
    SerializedProperty aiDifficultyLevel;
    SerializedProperty raceEndTimerLogic;
    SerializedProperty speedUnit;

    //Player Settings
    SerializedProperty playerVehiclePrefab;
    SerializedProperty playerStartPosition;
    SerializedProperty playerName;
    SerializedProperty playerNationality;

    //AI Settings
    SerializedProperty aiVehiclePrefabs;
    SerializedProperty aiDetails;
    SerializedProperty easyAiDifficulty;
    SerializedProperty mediumAiDifficulty;
    SerializedProperty hardAiDifficulty;

    //Minimap / Racer Names
    SerializedProperty playerMinimapIcon;
    SerializedProperty opponentMinimapIcon;
    SerializedProperty racerName;

    //Race Settings - Values
    SerializedProperty loadRaceSettings;
    SerializedProperty lapCount;
    SerializedProperty opponentCount;
    SerializedProperty checkpointTimeStart;
    SerializedProperty eliminationTimeStart;
    SerializedProperty enduranceTimeStart;
    SerializedProperty driftTimeStart;
    SerializedProperty rollingStartSpeed;
    SerializedProperty useTimeLimit;
    SerializedProperty postRaceSpeedMultiplier;
    SerializedProperty autoStartRace;
    SerializedProperty nonCollisionRace;
    SerializedProperty postRaceCollisions;
    SerializedProperty enableRaceEndTimer;
    SerializedProperty raceEndTimerStart;
    //SerializedProperty infiniteLaps;
    SerializedProperty autoDriveTimeTrial;
    SerializedProperty flyingStart;
    SerializedProperty flyingStartSpeed;
    SerializedProperty finishEnduranceImmediately;
    SerializedProperty endRaceDelay;
    SerializedProperty enableCinematicCameraAfterFinish;
    SerializedProperty autoStartReplay;
    SerializedProperty timeTrialStartPoint;

    //Race Settings - Countdown
    SerializedProperty countdownFrom;
    SerializedProperty countdownDelay;
    SerializedProperty countdownAudio;

    //Race Settings - Catchup
    SerializedProperty enableCatchup;
    SerializedProperty catchupStrength;
    SerializedProperty minCatchupRange;
    SerializedProperty maxCatchupRange;

    //Penalty Settings
    SerializedProperty enableOfftrackPenalty;
    SerializedProperty minWheelCountForOfftrack;
    SerializedProperty forceWrongwayRespawn;
    SerializedProperty wrongwayRespawnTime;

    //Respawn Settings
    SerializedProperty respawnSettings;

    //Slipstream Settings
    SerializedProperty slipstreamSettings;

    //Drift Race Settings
    SerializedProperty driftRaceSettings;

    //Ghost Settings
    SerializedProperty enableGhostVehicle;
    SerializedProperty ghostVehicleMaterial;
    SerializedProperty ghostVehicleShader;

    //Target Time / Score
    SerializedProperty targetTimeGold;
    SerializedProperty targetTimeSilver;
    SerializedProperty targetTimeBronze;
    SerializedProperty targetScoreGold;
    SerializedProperty targetScoreSilver;
    SerializedProperty targetScoreBronze;

    //Music Settings
    SerializedProperty startMusicAfterCountdown;
    SerializedProperty postRaceMusic;
    SerializedProperty loopPostRaceMusic;

    //Message Settings
    SerializedProperty showRaceInfoMessages;
    SerializedProperty showWhenRacerFinishes;
    SerializedProperty showSplitTimes;
    SerializedProperty showVehicleAheadAndBehindGap;
    SerializedProperty addPositionToRaceEndMessage;
    SerializedProperty bestTimeInfo;
    SerializedProperty finalLapInfo;
    SerializedProperty invalidLapInfo;
    SerializedProperty raceEndMessage;
    SerializedProperty lapKnockoutDisqualifyInfo;
    SerializedProperty checkpointDisqualifyInfo;
    SerializedProperty eliminationDisqualifyInfo;
    SerializedProperty defaultDisqualifyInfo;


    void OnEnable()
    {
        _target = (RaceManager)target;

        centerLabelStyle = new GUIStyle();
        centerLabelStyle.fontStyle = FontStyle.Normal;
        centerLabelStyle.normal.textColor = Color.white;
        centerLabelStyle.alignment = TextAnchor.MiddleCenter;

        //RaceSettings - Enums
        raceType = serializedObject.FindProperty("raceType");
        startMode = serializedObject.FindProperty("startMode");
        playerGridPositioningMode = serializedObject.FindProperty("playerGridPositioningMode");
        aiDifficultyLevel = serializedObject.FindProperty("aiDifficultyLevel");
        raceEndTimerLogic = serializedObject.FindProperty("raceEndTimerLogic");
        speedUnit = serializedObject.FindProperty("speedUnit");

        //Player Settings
        playerVehiclePrefab = serializedObject.FindProperty("playerVehiclePrefab");
        playerStartPosition = serializedObject.FindProperty("playerStartPosition");
        playerName = serializedObject.FindProperty("playerName");
        playerNationality = serializedObject.FindProperty("playerNationality");

        //AI Settings
        aiVehiclePrefabs = serializedObject.FindProperty("aiVehiclePrefabs");
        aiDetails = serializedObject.FindProperty("aiDetails");
        easyAiDifficulty = serializedObject.FindProperty("easyAiDifficulty");
        mediumAiDifficulty = serializedObject.FindProperty("mediumAiDifficulty");
        hardAiDifficulty = serializedObject.FindProperty("hardAiDifficulty");

        //Minimap / Racer Names
        playerMinimapIcon = serializedObject.FindProperty("playerMinimapIcon");
        opponentMinimapIcon = serializedObject.FindProperty("opponentMinimapIcon");
        racerName = serializedObject.FindProperty("racerName");

        //RaceSettings - Values
        loadRaceSettings = serializedObject.FindProperty("loadRaceSettings");
        lapCount = serializedObject.FindProperty("lapCount");
        opponentCount = serializedObject.FindProperty("opponentCount");
        checkpointTimeStart = serializedObject.FindProperty("checkpointTimeStart");
        eliminationTimeStart = serializedObject.FindProperty("eliminationTimeStart");
        enduranceTimeStart = serializedObject.FindProperty("enduranceTimeStart");
        driftTimeStart = serializedObject.FindProperty("driftTimeStart");
        raceEndTimerStart = serializedObject.FindProperty("raceEndTimerStart");
        rollingStartSpeed = serializedObject.FindProperty("rollingStartSpeed");
        endRaceDelay = serializedObject.FindProperty("endRaceDelay");
        postRaceSpeedMultiplier = serializedObject.FindProperty("postRaceSpeedMultiplier");
        autoStartRace = serializedObject.FindProperty("autoStartRace");
        nonCollisionRace = serializedObject.FindProperty("nonCollisionRace");
        postRaceCollisions = serializedObject.FindProperty("postRaceCollisions");
        enableRaceEndTimer = serializedObject.FindProperty("enableRaceEndTimer");
        //infiniteLaps = serializedObject.FindProperty("infiniteLaps");
        autoDriveTimeTrial = serializedObject.FindProperty("autoDriveTimeTrial");
        flyingStart = serializedObject.FindProperty("flyingStart");
        flyingStartSpeed = serializedObject.FindProperty("flyingStartSpeed");
        useTimeLimit = serializedObject.FindProperty("useTimeLimit");
        finishEnduranceImmediately = serializedObject.FindProperty("finishEnduranceImmediately");
        enableCinematicCameraAfterFinish = serializedObject.FindProperty("enableCinematicCameraAfterFinish");
        autoStartReplay = serializedObject.FindProperty("autoStartReplay");
        timeTrialStartPoint = serializedObject.FindProperty("timeTrialStartPoint");

        //Race Settings - Countdown
        countdownFrom = serializedObject.FindProperty("countdownFrom");
        countdownDelay = serializedObject.FindProperty("countdownDelay");
        countdownAudio = serializedObject.FindProperty("countdownAudio");

        //Race Settings - Countdown
        enableCatchup = serializedObject.FindProperty("enableCatchup");
        catchupStrength = serializedObject.FindProperty("catchupStrength");
        minCatchupRange = serializedObject.FindProperty("minCatchupRange");
        maxCatchupRange = serializedObject.FindProperty("maxCatchupRange");

        //Penalty Settings
        enableOfftrackPenalty = serializedObject.FindProperty("enableOfftrackPenalty");
        minWheelCountForOfftrack = serializedObject.FindProperty("minWheelCountForOfftrack");
        forceWrongwayRespawn = serializedObject.FindProperty("forceWrongwayRespawn");
        wrongwayRespawnTime = serializedObject.FindProperty("wrongwayRespawnTime");

        //Respawn Settings
        respawnSettings = serializedObject.FindProperty("respawnSettings");

        //Slipstream Settings
        slipstreamSettings = serializedObject.FindProperty("slipstreamSettings");

        //Drift Settings
        driftRaceSettings = serializedObject.FindProperty("driftRaceSettings");

        //Ghost Settings
        enableGhostVehicle = serializedObject.FindProperty("enableGhostVehicle");
        ghostVehicleMaterial = serializedObject.FindProperty("ghostVehicleMaterial");
        ghostVehicleShader = serializedObject.FindProperty("ghostVehicleShader");

        //Target Time / Score
        targetTimeGold = serializedObject.FindProperty("targetTimeGold");
        targetTimeSilver = serializedObject.FindProperty("targetTimeSilver");
        targetTimeBronze = serializedObject.FindProperty("targetTimeBronze");
        targetScoreGold = serializedObject.FindProperty("targetScoreGold");
        targetScoreSilver = serializedObject.FindProperty("targetScoreSilver");
        targetScoreBronze = serializedObject.FindProperty("targetScoreBronze");

        //Music
        startMusicAfterCountdown = serializedObject.FindProperty("startMusicAfterCountdown");
        postRaceMusic = serializedObject.FindProperty("postRaceMusic");
        loopPostRaceMusic = serializedObject.FindProperty("loopPostRaceMusic");

        //Message Settings
        showRaceInfoMessages = serializedObject.FindProperty("showRaceInfoMessages");
        showWhenRacerFinishes = serializedObject.FindProperty("showWhenRacerFinishes");
        showSplitTimes = serializedObject.FindProperty("showSplitTimes");
        showVehicleAheadAndBehindGap = serializedObject.FindProperty("showVehicleAheadAndBehindGap");
        addPositionToRaceEndMessage = serializedObject.FindProperty("addPositionToRaceEndMessage");
        bestTimeInfo = serializedObject.FindProperty("bestTimeInfo");
        finalLapInfo = serializedObject.FindProperty("finalLapInfo");
        invalidLapInfo = serializedObject.FindProperty("invalidLapInfo");
        raceEndMessage = serializedObject.FindProperty("raceEndMessage");
        lapKnockoutDisqualifyInfo = serializedObject.FindProperty("lapKnockoutDisqualifyInfo");
        checkpointDisqualifyInfo = serializedObject.FindProperty("checkpointDisqualifyInfo");
        eliminationDisqualifyInfo = serializedObject.FindProperty("eliminationDisqualifyInfo");
        defaultDisqualifyInfo = serializedObject.FindProperty("defaultDisqualifyInfo");
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        editorTab = GUILayout.SelectionGrid(editorTab, toolbarTabs, 3);

        if(EditorGUI.EndChangeCheck())
        {
            GUI.FocusControl(null);
        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        switch(editorTab)
        {
            //RACE SETTINGS
            case 0:

                EditorGUILayout.LabelField("Общие настройки", centerLabelStyle);
                EditorGUILayout.Space();

                //---Enums---
                EditorGUILayout.PropertyField(raceType);
                EditorGUILayout.PropertyField(startMode);
                EditorGUILayout.PropertyField(raceEndTimerLogic);
                EditorGUILayout.PropertyField(speedUnit);

                //---Values---
                if (_target.raceType == RaceType.LapKnockout || _target.raceType == RaceType.Endurance ||
                    _target.raceType == RaceType.Drag || _target.raceType == RaceType.Drift ||
                    _target.raceType == RaceType.TimeTrial || _target.raceType == RaceType.TimeAttack)
                {
                    EditorGUILayout.HelpBox("Этот тип гонки отменяет подсчет кругов и/или количество соперников..", MessageType.Info);
                }

                EditorGUILayout.PropertyField(lapCount);
                EditorGUILayout.PropertyField(opponentCount);

                GUILayout.Space(10);
                EditorGUILayout.PropertyField(checkpointTimeStart);
                EditorGUILayout.PropertyField(eliminationTimeStart);
                EditorGUILayout.PropertyField(enduranceTimeStart);
                EditorGUILayout.PropertyField(driftTimeStart);
                EditorGUILayout.PropertyField(rollingStartSpeed);
                EditorGUILayout.PropertyField(raceEndTimerStart);
                EditorGUILayout.PropertyField(flyingStartSpeed);

                GUILayout.Space(10);
                EditorGUILayout.PropertyField(loadRaceSettings);
                EditorGUILayout.PropertyField(autoStartRace);
                EditorGUILayout.PropertyField(useTimeLimit);
                EditorGUILayout.PropertyField(autoDriveTimeTrial);
                EditorGUILayout.PropertyField(nonCollisionRace);
                EditorGUILayout.PropertyField(finishEnduranceImmediately);
                EditorGUILayout.PropertyField(enableRaceEndTimer);
                EditorGUILayout.PropertyField(flyingStart);

                EditorGUILayout.PropertyField(timeTrialStartPoint);

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                //---Countdown---
                EditorGUILayout.LabelField("Настройки обратного отсчета", centerLabelStyle);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(countdownFrom);
                EditorGUILayout.PropertyField(countdownDelay);
                EditorGUILayout.PropertyField(countdownAudio, true);
                EditorGUILayout.PropertyField(startMusicAfterCountdown);

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                //---Minimap / Racer Name---
                EditorGUILayout.LabelField("Настройки значка на миникарте/имени гонщика", centerLabelStyle);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(playerMinimapIcon);
                EditorGUILayout.PropertyField(opponentMinimapIcon);
                EditorGUILayout.PropertyField(racerName);

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                //---Catchup---
                EditorGUILayout.LabelField("Настройки догонялок", centerLabelStyle);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(enableCatchup);
                EditorGUILayout.PropertyField(catchupStrength);
                EditorGUILayout.PropertyField(minCatchupRange);
                EditorGUILayout.PropertyField(maxCatchupRange);

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                //---Penalty---
                EditorGUILayout.LabelField("Настройки штрафа", centerLabelStyle);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(enableOfftrackPenalty);
                EditorGUILayout.PropertyField(forceWrongwayRespawn);
                EditorGUILayout.PropertyField(minWheelCountForOfftrack);
                EditorGUILayout.PropertyField(wrongwayRespawnTime);

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                //---Respawn Settings---
                EditorGUILayout.LabelField("Параметры респауна", centerLabelStyle);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(respawnSettings, true);

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                //---Slipstream---
                EditorGUILayout.LabelField("Настройки слипстрима", centerLabelStyle);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(slipstreamSettings, true);

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                //---Drift---
                EditorGUILayout.LabelField("Настройки дрифт-гонки", centerLabelStyle);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(driftRaceSettings, true);

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                //---Ghost---
                EditorGUILayout.LabelField("Настройки Призрачного Транспортного Средства", centerLabelStyle);
                EditorGUILayout.Space();

                if (_target.ghostVehicleShader != null && _target.ghostVehicleMaterial != null)
                {
                    EditorGUILayout.HelpBox("Назначены как материал призрачного транспортного средства, так и шейдер. Пожалуйста, назначьте только один.", MessageType.Warning);
                }

                EditorGUILayout.PropertyField(enableGhostVehicle);
                EditorGUILayout.PropertyField(ghostVehicleMaterial);
                EditorGUILayout.PropertyField(ghostVehicleShader);

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                //Target Time/Scores
                EditorGUILayout.LabelField("Настройки целевого времени/счета", centerLabelStyle);
                EditorGUILayout.Space();

                EditorGUILayout.HelpBox("В гонках типа Time Attack используются целевые времена..", MessageType.Info);
                EditorGUILayout.PropertyField(targetTimeGold);
                EditorGUILayout.PropertyField(targetTimeSilver);
                EditorGUILayout.PropertyField(targetTimeBronze);

                EditorGUILayout.HelpBox("В гонках типа «Дрифт» использовались целевые баллы...", MessageType.Info);
                EditorGUILayout.PropertyField(targetScoreGold);
                EditorGUILayout.PropertyField(targetScoreSilver);
                EditorGUILayout.PropertyField(targetScoreBronze);

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                //---Post Race---
                EditorGUILayout.LabelField("Настройки после гонки", centerLabelStyle);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(endRaceDelay);
                EditorGUILayout.PropertyField(postRaceSpeedMultiplier);
                EditorGUILayout.PropertyField(postRaceMusic);
                EditorGUILayout.PropertyField(enableCinematicCameraAfterFinish);
                EditorGUILayout.PropertyField(autoStartReplay);
                EditorGUILayout.PropertyField(postRaceCollisions);
                EditorGUILayout.PropertyField(loopPostRaceMusic);

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                //---Messages---
                EditorGUILayout.LabelField("Сообщения в гонке", centerLabelStyle);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(showRaceInfoMessages);
                EditorGUILayout.PropertyField(showWhenRacerFinishes);
                EditorGUILayout.PropertyField(showSplitTimes);
                EditorGUILayout.PropertyField(showVehicleAheadAndBehindGap);
                EditorGUILayout.PropertyField(addPositionToRaceEndMessage);
                EditorGUILayout.PropertyField(bestTimeInfo, true);
                EditorGUILayout.PropertyField(finalLapInfo, true);
                EditorGUILayout.PropertyField(invalidLapInfo, true);
                EditorGUILayout.PropertyField(lapKnockoutDisqualifyInfo, true);
                EditorGUILayout.PropertyField(checkpointDisqualifyInfo, true);
                EditorGUILayout.PropertyField(eliminationDisqualifyInfo, true);
                EditorGUILayout.PropertyField(defaultDisqualifyInfo, true);
                EditorGUILayout.PropertyField(raceEndMessage);

                break;

                //Player Settings
            case 1:

                //Player
                EditorGUILayout.PropertyField(playerVehiclePrefab);
                EditorGUILayout.PropertyField(playerName);
                EditorGUILayout.PropertyField(playerNationality);
                EditorGUILayout.PropertyField(playerGridPositioningMode);
                if (_target.playerGridPositioningMode == GridPositioningMode.Select)
                {
                    EditorGUILayout.PropertyField(playerStartPosition);
                }

                break;


                //AI Settings
            case 2:

                //AI
                EditorGUILayout.PropertyField(aiVehiclePrefabs, true);
                EditorGUILayout.PropertyField(aiDetails);

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                //Difficulty
                EditorGUILayout.LabelField("AI Difficulty Settings", centerLabelStyle);
                EditorGUILayout.PropertyField(aiDifficultyLevel);
                EditorGUILayout.PropertyField(easyAiDifficulty);
                EditorGUILayout.PropertyField(mediumAiDifficulty);
                EditorGUILayout.PropertyField(hardAiDifficulty);
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
