# 使用 PHP 8.1 的 Apache 映像
FROM php:8.1-apache

# 安裝 MySQL 客戶端和其他必要擴展
RUN apt-get update && apt-get install -y \
    libpng-dev \
    libjpeg-dev \
    libfreetype6-dev \
    libzip-dev \
    git \
    unzip \
    && rm -rf /var/lib/apt/lists/* \
    && docker-php-ext-configure gd --with-freetype --with-jpeg \
    && docker-php-ext-install gd zip pdo pdo_mysql mysqli

# 【新】複製我們自訂的 ports.conf 設定檔到容器中，覆蓋 Apache 的預設設定
COPY ports.conf /etc/apache2/ports.conf

# 設置工作目錄為 /var/www/html
WORKDIR /var/www/html

# 複製當前目錄下的 PHP 檔案到容器內的工作目錄
COPY . /var/www/html/

# 設置適當的權限
RUN chown -R www-data:www-data /var/www/html

# EXPOSE 8080 (可選，僅為文件目的，Cloud Run 會忽略)
EXPOSE 8080

# Apache 會自動啟動並根據我們的設定檔監聽 $PORT
CMD ["apache2-foreground"]