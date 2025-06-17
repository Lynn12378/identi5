<?php

header("Access-Control-Allow-Origin: *");
header("Access-Control-Allow-Methods: POST, GET, OPTIONS");
header("Access-Control-Allow-Headers: Content-Type");

include 'Connector.php';

$conn = connectDatabase();

if (!$conn) {
    echo "Database connection failed.";
    exit;
}

// 處理搜索條件
$search_query = "";
if (isset($_GET['search'])) {
    $search_query = $conn->real_escape_string($_GET['search']);
}

// 處理篩選條件
$filter_start_date = "";
$filter_end_date = "";

if (isset($_GET['filter_start_date'])) {
    $filter_start_date = $conn->real_escape_string($_GET['filter_start_date']);
}

if (isset($_GET['filter_end_date'])) {
    $filter_end_date = $conn->real_escape_string($_GET['filter_end_date']);
}

// 設置時間為 23:59:59
// 創建新變量用於儲存包含時間的日期（否則html會無法顯示
$filter_end_datetime = $filter_end_date;
if (!empty($filter_end_datetime)) {
    $filter_end_datetime .= ' 23:59:59';
}

// 處理排序條件
$sort_column = "Player_name";
$sort_order = "ASC";

if (isset($_GET['sort_by'])) {
    switch ($_GET['sort_by']) {
        case 'Player_name':
            $sort_column = "player.Player_name";
            break;
        case 'playTime':
            $sort_column = "output_data.playTime";
            break;
        case 'openness':
            $sort_column = "bfi_result.openness";
            break;
        case 'conscientiousness':
            $sort_column = "bfi_result.conscientiousness";
            break;
        case 'extraversion':
            $sort_column = "bfi_result.extraversion";
            break;
        case 'agreeableness':
            $sort_column = "bfi_result.agreeableness";
            break;
        case 'neuroticism':
            $sort_column = "bfi_result.neuroticism";
            break;
        default:
            $sort_column = "Player_name";
    }
}

if (isset($_GET['order'])) {
    $sort_order = $conn->real_escape_string($_GET['order']);
}

// 查詢資料庫，使用 JOIN 結合 `player`, `output_data` 和 `bfi_result` 表
$sql = "SELECT  player.Player_id,
                player.Player_name,
                output_data.id AS output_data_id,
                bfi_result.id AS bfi_result_id,
                output_data.*, bfi_result.*
        FROM player
        JOIN output_data ON player.Player_id = output_data.playerId
        JOIN bfi_result ON player.Player_id = bfi_result.player_id
        WHERE player.Player_name LIKE '%$search_query%'";

if (!empty($filter_start_date) && !empty($filter_end_datetime)) {
    $sql .= " AND output_data.playTime BETWEEN '$filter_start_date' AND '$filter_end_datetime'";
}

// 加入排序
$sql .= " ORDER BY $sort_column $sort_order";

$result = $conn->query($sql);
?>


<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Identi5 分析報告列表</title>
    <style>
        body {
            font-family: "STKaiTi", serif;
            padding: 20px;
            background-color: #e5e5d5;
            color: #333;
        }
        .header {
            position: sticky;
            top: 0;
            background-color: #e5e5d5;
            padding: 10px 0;
            border-bottom: 2px solid #ddd;
            padding-bottom: 40px;
            z-index: 1000;
            display: flex;
            justify-content: flex-start;
            align-items: center;
            flex-wrap: wrap;
        }
        .header form {
            display: flex;
            align-items: center;
            width: 100%;
            gap: 10px;
        }
        .search-bar, .filter-bar, .sort-bar {
            font-family: "STKaiTi", serif;
            padding: 10px;
            border: 1px solid #ccc;
            border-radius: 4px;
            font-size: 18px;
            box-sizing: border-box;
        }
        .search-bar {
            flex: 2;
            min-width: 200px;
        }

        .filter-bar, .sort-bar {
            flex: 1;
            min-width: 100px;
        }
        button {
            font-family: "STKaiTi", serif;
            padding: 10px 20px;
            border: none;
            border-radius: 4px;
            background-color: #28a745;
            color: white;
            font-size: 18px;
            cursor: pointer;
            transition: background-color 0.3s;
        }
        button:hover {
            background-color: #218838;
        }
        .player-list {
            margin-top: 50px;
            border-collapse: collapse;
            width: 100%;
            border-radius: 4px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
            background-color: #ffffff;
        }
        .player-list th, .player-list td {
            border: 1px solid #ddd;
            padding: 12px;
            text-align: center;
            font-size: 18px;
        }
        .player-list th {
            background-color: #f2f2f2;
        }
        .player-list a {
            color: #007bff;
            text-decoration: none;
        }
        .player-list a:hover {
            text-decoration: underline;
        }
    </style>
</head>
<body>

<div class="header">
    <form method="GET" action="index.php">
        <input class="search-bar" type="text" name="search" placeholder="搜尋玩家名稱..." value="<?php echo htmlspecialchars($search_query); ?>">
        
        <label for="filter_start_date">篩選遊玩日期區間:</label>
        <input class="filter-bar" type="date" id="filter_start_date" name="filter_start_date" value="<?php echo htmlspecialchars($filter_start_date); ?>">

        <label for="filter_end_date">～</label>
        <input class="filter-bar" type="date" id="filter_end_date" name="filter_end_date" value="<?php echo htmlspecialchars($filter_end_date); ?>">
        
        <select class="sort-bar" name="sort_by">
            <option value="Player_name" <?php if ($sort_column == "player.Player_name") echo 'selected'; ?>>排序依據: 玩家名稱</option>
            <option value="playTime" <?php if ($sort_column == "output_data.playTime") echo 'selected'; ?>>排序依據: 遊玩時間</option>
            <option value="openness" <?php if ($sort_column == "bfi_result.openness") echo 'selected'; ?>>排序依據: 開放性</option>
            <option value="conscientiousness" <?php if ($sort_column == "bfi_result.conscientiousness") echo 'selected'; ?>>排序依據: 盡責性</option>
            <option value="extraversion" <?php if ($sort_column == "bfi_result.extraversion") echo 'selected'; ?>>排序依據: 外向性</option>
            <option value="agreeableness" <?php if ($sort_column == "bfi_result.agreeableness") echo 'selected'; ?>>排序依據: 親和性</option>
            <option value="neuroticism" <?php if ($sort_column == "bfi_result.neuroticism") echo 'selected'; ?>>排序依據: 情緒不穩定性</option>
        </select>
        
        <select class="sort-bar" name="order">
            <option value="ASC" <?php if ($sort_order == "ASC") echo 'selected'; ?>>升冪 (A-Z)</option>
            <option value="DESC" <?php if ($sort_order == "DESC") echo 'selected'; ?>>降冪 (Z-A)</option>
        </select>
        <button type="submit">搜尋</button>
    </form>
</div>

<table class="player-list">
    <thead>
        <tr>
            <th>玩家名稱</th>
            <th>遊玩時間</th>
            <th>開放性</th>
            <th>盡責性</th>
            <th>外向性</th>
            <th>親和性</th>
            <th>情緒不穩定性</th>
            <th>個人報告</th>
        </tr>
    </thead>
    <tbody>
        <?php
        if ($result->num_rows > 0) {
            while($row = $result->fetch_assoc()) {
                $player_id = htmlspecialchars($row['Player_id']);
                $report_url = "startNotebook.php?player_id=" . urlencode($player_id);

                echo "<tr>";
                echo "<td>" . htmlspecialchars($row['Player_name']) . "</td>";
                echo "<td>" . htmlspecialchars($row['playTime']) . "</td>";
                echo "<td>" . htmlspecialchars($row['openness']) . "</td>";
                echo "<td>" . htmlspecialchars($row['conscientiousness']) . "</td>";
                echo "<td>" . htmlspecialchars($row['extraversion']) . "</td>";
                echo "<td>" . htmlspecialchars($row['agreeableness']) . "</td>";
                echo "<td>" . htmlspecialchars($row['neuroticism']) . "</td>";
                echo "<td><a href='" . htmlspecialchars($report_url) . "' target='_blank'>查看報告</a></td>";
                echo "</tr>";
            }
        } else {
            echo "<tr><td colspan='8'>無符合條件的玩家</td></tr>";
        }
        ?>
    </tbody>
</table>

<?php $conn->close(); ?>

</body>
</html>