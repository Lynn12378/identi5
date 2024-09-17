<?php
header("Access-Control-Allow-Origin: *");
header("Access-Control-Allow-Methods: POST, GET, OPTIONS");
header("Access-Control-Allow-Headers: Content-Type");

include 'Connector.php';
include 'OutputData.php';

$Action = $_GET['Action'];
$conn = connectDatabase();
if (!$conn) {
    echo "Database connection failed.";
    return;
}

switch ($Action) {
    case "getAll":
        $sql = "SELECT * FROM Player";
        $result = $conn->query($sql);

        if ($result->num_rows > 0) {
            $rows = array();
            while($row = $result->fetch_assoc()) {
                $response[] = $row;
            }
            echo json_encode($response);
        } else {
            echo json_encode(array("status" => "error", "message" => "0 results"));
        }
        break;
//登入
    case "login":
        $PlayerInfo = json_decode($_POST['PlayerInfo']);
        $PlayerOutputData = json_decode($_POST['PlayerOutputData']);
        $Player_name = $PlayerInfo->Player_name;
        $Player_password = $PlayerInfo->Player_password;
        $response = array();
        $result = Check($conn, $Player_name);
        if ($result->num_rows > 0)
        {
            while($row = $result->fetch_assoc())
            {
                if ($row['Player_password'] == $Player_password)
                {
                    $response["status"] = "Success"; 
                    $response['message'] = "登入成功";
                }
                else
                {
                    $response['status'] = "Failure";
                    $response['message'] = "密碼錯誤，請再試一次";
                }
                $Player_id = $row['Player_id'];
                $PlayerInfo->Player_id = $Player_id; 
                $PlayerOutputData->Player_id = $Player_id;
                $PlayerInfo = GetOutfits($conn, $PlayerInfo);

                $date = date('Y-m-d H:i:s', time());
                UpdateColumn($conn, "playTime", $Player_id, $date);
                UpdateColumn($conn, "manualTime", $Player_id, $PlayerOutputData->manualTime);
                $PlayerOutputData = GetOutputData($conn, $PlayerOutputData);

                $response['PlayerInfo'] = $PlayerInfo;
                $response['PlayerOutputData'] = $PlayerOutputData;
            }
        }else{
            $response['status'] = "Failure";
            $response['message'] = "該帳戶名稱不存在";
        }
        header('Content-Type: application/json');
        echo json_encode($response);
        break;
//註冊
    case "signUp":
        #region player
        $PlayerInfo = json_decode($_POST['PlayerInfo']);
        $Player_name = $PlayerInfo->Player_name;
        $Player_password = $PlayerInfo->Player_password;
        $Player_colors = $PlayerInfo->colorList;
        $Player_outfits = $PlayerInfo->outfits;

        $response = array();
        $result = Check($conn, $Player_name);
        if ($result->num_rows > 0)
        {
            $response['status'] = "Failure";
            $response['message'] = "註冊失敗，該帳戶名稱已被註冊";
        }else{
            $sql = sprintf(
                "INSERT INTO player (Player_name, Player_password) VALUES ('%s', '%s')",
                $Player_name,
                $Player_password
            );
        
            $result = $conn->query($sql);
            $result = Check($conn, $Player_name);

            while($row = $result->fetch_assoc())
            {
                $Player_id = $row['Player_id'];
                $response['Player_id'] = $Player_id;
                $response['Player_name'] = $row['Player_name'];
            }
            $response["status"] = "Success";
            $response['message'] = "註冊成功";
            #endregion
            #region outfit
            $sql = sprintf(
                "INSERT INTO outfits (Player_id, Player_colors, Player_outfits) VALUES ('%s', '%s', '%s')",
                $Player_id,
                json_encode($Player_colors),
                json_encode($Player_outfits)
            );
            $result = $conn->query($sql);
            #endregion
            #region output_data
            $date = date('Y-m-d H:i:s', time());
            $sql = sprintf(
                "INSERT INTO output_data (player_id, signUptime) VALUES ('%s', '%s')",
                $Player_id,
                $date
            );
            $result = $conn->query($sql);
            #endregion
        }
        header('Content-Type: application/json');
        echo json_encode($response);
        break;
//創建
    case "create":
        $PlayerInfo = json_decode($_POST['PlayerInfo']);
        $PlayerOutputData = json_decode($_POST['PlayerOutputData']);
        $Player_id = $PlayerInfo->Player_id;
        $response = array();

        $sql = sprintf(
            "UPDATE outfits
            SET Player_colors = '%s', Player_outfits = '%s'
            WHERE player_id = %s",
            json_encode($PlayerInfo->colorList),
            json_encode($PlayerInfo->outfits),
            $Player_id
        );
        $result = $conn->query($sql); 
        $date = date('Y-m-d H:i:s', time());
        UpdateColumn($conn, "playTime", $Player_id, $date);
        UpdateColumn($conn, "outfitTime", $Player_id, $PlayerOutputData->outfitTime);        
        UpdateColumn($conn, "manualTime", $Player_id, $PlayerOutputData->manualTime);
        $response["status"] = "Success";
        $response['message'] = "角色創建成功";
        header('Content-Type: application/json');
        echo json_encode($response);
        break;
    default:
        echo json_encode(array("status" => "error", "message" => "Unknown action."));
        break;
}

$conn->close();

function Check($conn, $Player_name)
{
    $sql = sprintf(
        "SELECT * FROM player WHERE Player_name= '%s' Limit 1",
        $Player_name
    );

    $result = $conn->query($sql);
    return $result;
}

function GetOutfits($conn, $PlayerInfo)
{
    $id = $PlayerInfo->Player_id;
    $sql = sprintf(
        "SELECT * FROM outfits WHERE Player_id= '%s' Limit 1",
        $id
    );

    $result = $conn->query($sql);
    if ($result)
    {
        $row = $result->fetch_assoc();
        $PlayerInfo->colorList = json_decode($row['Player_colors']);
        $PlayerInfo->outfits = json_decode($row['Player_outfits']);
    }
    return $PlayerInfo;
}
?>