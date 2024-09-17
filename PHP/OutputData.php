<?php

header("Access-Control-Allow-Origin: *");
header("Access-Control-Allow-Methods: POST, GET, OPTIONS");
header("Access-Control-Allow-Headers: Content-Type");

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
        $PlayerOutputData->buildingVisit = json_decode($row['buildingVisit']);
        $PlayerOutputData->remainHP = json_decode($row['remainHP']);
        $PlayerOutputData->remainBullet = json_decode($row['remainBullet']);
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
