<?php

header("Access-Control-Allow-Origin: *");
header("Access-Control-Allow-Methods: POST, GET, OPTIONS");
header("Access-Control-Allow-Headers: Content-Type");

function connectDatabase()
{
    // 連接mysql
    $conn = new mysqli("localhost", "Admin", "/yWV1AIhVHWSKn7l", "identi5db");

    // 檢查連線
    if ($conn->connect_error)
    {
        error_log("Database connection failed: " . $conn->connect_error);
        return null;
    }

    return $conn;
}
?>