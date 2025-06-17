<?php

header("Access-Control-Allow-Origin: *");
header("Access-Control-Allow-Methods: POST, GET, OPTIONS");
header("Access-Control-Allow-Headers: Content-Type");

include 'Connector.php';

$conn = connectDatabase();

if (!$conn) 
{
    echo "Database connection failed.";
    return;
}

$sql = "SELECT Player_id, Player_name FROM player";
$result = $conn->query($sql);

$data = array();
while ($row = $result->fetch_assoc()) {
    $data[] = $row;
}

header('Content-Type: application/json');
echo json_encode($data);

$conn->close();
?>
