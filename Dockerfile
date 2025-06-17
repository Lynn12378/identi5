# 使用 PHP 8.1 的 Apache 映像
FROM php:8.1-apache

# 安裝 MySQL 客戶端和其他必要擴展
RUN apt-get update && apt-get install -y libpng-dev libjpeg-dev libfreetype6-dev && \
    apt-get install -y libzip-dev && \
    apt-get install -y git unzip && \
    docker-php-ext-configure gd --with-freetype --with-jpeg && \
    docker-php-ext-install gd zip pdo pdo_mysql mysqli  # 加上 mysqli 擴展

# 設置工作目錄為 /var/www/html
WORKDIR /var/www/html

# 複製當前目錄下的 PHP 檔案到容器內的工作目錄
COPY . /var/www/html/

# 設置適當的權限
RUN chown -R www-data:www-data /var/www/html

# 暴露容器的 80 埠（Apache 預設端口）
EXPOSE 80

# Apache 會自動啟動並監聽 80 端口
CMD ["apache2-foreground"]
