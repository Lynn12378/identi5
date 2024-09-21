<?php

header("Access-Control-Allow-Origin: *");
header("Access-Control-Allow-Methods: POST, GET, OPTIONS");
header("Access-Control-Allow-Headers: Content-Type");

$Action = $_GET['Action'];
if($Action == "Update")
{
    include 'Connector.php';
    $conn = connectDatabase();

    $PlayerOutputData = json_decode($_POST['PlayerOutputData']);
    $sql = sprintf(
        "UPDATE output_data
        SET
        isFinished = '%s',
        oufitChangedNo = '%s',
        placeholderNo = '%s',
        buildingVisit = '%s',
        failGameNo = '%s',
        deathNo = '%s',
        killNo = '%s',
        organizeNo = '%s',
        fullNo = '%s',
        zombieInShelterNo = '%s',
        surviveTime = '%s',
        contribution = '%s',
        messageSent = '%s',
        teamCreated = '%s',
        quitTeamNo = '%s',
        totalVoiceDetectionDuration = '%s',
        joinTeamNo = '%s',
        giftNo = '%s',
        rankClikedNo = '%s',
        bulletOnLiving = '%s',
        bulletOnPlayer = '%s',
        interactNo = '%s',
        timeInShelter = '%s'
        collisionMapNo = '%s',
        bulletOnCollisions = '%s',
        remainHP = '%s',
        remainBullet = '%s'
        WHERE player_id = %s",

        $PlayerOutputData->isFinished,
        $PlayerOutputData->oufitChangedNo,
        $PlayerOutputData->placeholderNo,
        json_encode($PlayerOutputData->buildingVisit),
        $PlayerOutputData->failGameNo,
        $PlayerOutputData->deathNo,
        $PlayerOutputData->killNo,
        $PlayerOutputData->organizeNo,
        $PlayerOutputData->fullNo,
        $PlayerOutputData->zombieInShelterNo,
        $PlayerOutputData->surviveTime,
        $PlayerOutputData->contribution,
        $PlayerOutputData->messageSent,
        $PlayerOutputData->teamCreated,
        $PlayerOutputData->quitTeamNo,
        $PlayerOutputData->totalVoiceDetectionDuration,
        $PlayerOutputData->joinTeamNo,
        $PlayerOutputData->giftNo,
        $PlayerOutputData->rankClikedNo,
        $PlayerOutputData->bulletOnLiving,
        $PlayerOutputData->bulletOnPlayer,
        $PlayerOutputData->interactNo,
        $PlayerOutputData->timeInShelter,
        $PlayerOutputData->collisionMapNo,
        $PlayerOutputData->bulletOnCollisions,
        json_encode($PlayerOutputData->remainHP),
        json_encode($PlayerOutputData->remainBullet),
        $PlayerOutputData->playerId
    );
    echo $sql;
    $result = $conn->query($sql);
    
}
if($Action == "GetOD")
{
    include 'Connector.php';
    $conn = connectDatabase();

    $PlayerOutputData = json_decode($_POST['PlayerOutputData']);
    $response = array();
    $response['PlayerOutputData'] = GetOutputData($conn, $PlayerOutputData);
    header('Content-Type: application/json');
    echo json_encode($response);
}

function GetOutputData($conn, $PlayerOutputData)
{
    $sql = sprintf(
        "SELECT * FROM output_data WHERE Player_id= '%s' Limit 1",
        $PlayerOutputData->Player_id
    );

    $result = $conn->query($sql);
    if ($result)
    {
        $row = $result->fetch_assoc();
        $PlayerOutputData->outfitTime = $row['outfitTime'];
        $PlayerOutputData->manualTime = $row['manualTime'];
        $PlayerOutputData->failGameNo = $row['failGameNo'];
    }
    return $PlayerOutputData;
}

function UpdateColumn($conn, $column, $Player_id, $value)
{
    $sql = sprintf(
        "UPDATE output_data
        SET $column = '%s'
        WHERE player_id = %s",
        $value,
        $Player_id
    );
    $result = $conn->query($sql);
}

?>