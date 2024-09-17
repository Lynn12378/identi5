<?php
if (isset($_GET['player_id'])) {
    $player_id = intval($_GET['player_id']); // 取得 player_id

    $notebookPath = 'C:/Users/hohyi/Desktop/畢業專題/Identi5.ipynb';
    $paramFilePath = 'C:/xampp/htdocs/DEMO/temp/player_id.txt';
    file_put_contents($paramFilePath, $player_id);

    $outputDir = 'C:/xampp/htdocs/DEMO/reports/';
    $outputPdfPath = $outputDir . "analysis_report_$player_id.pdf";

    $command = "jupyter nbconvert --execute $notebookPath --to notebook --ExecutePreprocessor.kernel_name=python3 --ExecutePreprocessor.allow_errors=True --ExecutePreprocessor.timeout=-1";

    // 執行啟動Notebook
    exec($command, $output, $return_var);

    if ($return_var === 0) {
        // 檢查PDF文件是否存在
        if (file_exists($outputPdfPath)) {
            // 設置頁面標題
            echo '<!DOCTYPE html>
            <html lang="zh">
            <head>
                <meta charset="UTF-8">
                <title>個人分析報告</title>
            </head>
            <body>';

            // 設置PDF直接在網頁顯示
            header('Content-Type: application/pdf');
            header('Content-Disposition: inline; filename="analysis_report_' . $player_id . '.pdf"');
            readfile($outputPdfPath);
            exit;
        } else {
            echo "找不到指定的 PDF 文件。";
        }
    } else {
        echo "執行Notebook或生成PDF時發生錯誤。<br>";
        echo "命令: " . htmlspecialchars($command) . "<br>";
        echo "錯誤碼: " . $return_var . "<br>";
        echo "輸出: <pre>" . implode("\n", $output) . "</pre>";
    }
} else {
    echo "未提供玩家ID。";
}
?>
