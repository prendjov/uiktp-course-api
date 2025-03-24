# Course Education API

## Overview

This repository contains the Course Education API solution, a .NET-based microservices architecture composed of several services, including an API and supporting infrastructure components such as MariaDB.
## Solution Structure

The solution is organized as follows:

- **src**: Contains the source code for various services.
  - **Api**: Main API service.
  - **Application**: Contains application-specific logic and services.
  - **DTO**: Data Transfer Objects used across the solution.
  - **Domain**: Contains domain models and business logic.
  - **Infrastructure**: Contains infrastructure-specific code, such as repositories.
  
- **docker-compose.yml**: Docker Compose configuration file to orchestrate the deployment of the services.

- **docker-compose.override.yml**: Override configuration for Docker Compose, typically used in development.

- **CourseEducation.sln**: Solution file for Visual Studio.

## Prerequisites

Before deploying the solution, ensure the following prerequisites are met:

- **Docker**: Ensure Docker is installed on your machine.
- **Docker Compose**: Ensure Docker Compose is installed.
- **.NET SDK**: Required for building the solution (if not building via Docker).

## Environment Configuration

The environment variables required by the services are already set in the Docker Compose file. However, you can customize them according to your requirements:

- **MariaDB**:
  - `MARIADB_ROOT_PASSWORD`: Root password for MariaDB.
  - `MARIADB_USER`: Username for accessing the database.
  - `MARIADB_PASSWORD`: Password for the specified user.
  - `MARIADB_DATABASE`: Database name to be created.
 
  - **WE STRONGLY RECOMMEND TO NOT USE THE **

- **ASP.NET Core**:
  - `ASPNETCORE_ENVIRONMENT`: Environment setting for the ASP.NET Core API.
    - `Development` - For development purposes
    - `Staging` - For testing
    - `Production`

### **Media Configuration**

The `Media` section in the `appsettings.Production.json` file defines how file uploads are handled across services. The **Base API URL** (`HostUrl`) should always match the actual API base URL. This needs to be updated in:

- `src/Api/appSettings.Production.json`
- `src/EmailService/appSettings.Production.json`
- `src/NotificationService/appSettings.Production.json`
- `src/Worker/appSettings.Production.json`

#### **Example Configuration:**

```json
"Media": {
  "AllowedExtensions": [],  
  "MaxFileSizeInKb": 51200,  
  "FileSystem": {  
    "HostUrl": "<API_BASE_URL>",  
    "UploadMediaPath": "media",  
    "UploadDocumentPath": "documents",  
    "CacheStaticFileInHours": 48  
  }  
}
```

#### **Explanation of Each Field**
- **AllowedExtensions**: Defines the allowed file types for uploads (currently empty, meaning all file types are accepted).
- **MaxFileSizeInKb**: Maximum file size allowed for uploads (default is `51200 KB`, which is **50MB**).
- **FileSystem**:
  - **HostUrl**: Base URL where media files will be uploaded and accessed.  
    - This should always be **the same as the API base URL** to avoid inconsistencies.  
    - Example: `http://localhost:8082` (if running locally).  
  - **UploadMediaPath**: The relative directory where media files (such as images and videos) are stored.
  - **UploadDocumentPath**: The relative directory for document uploads (such as PDFs and Word files).
  - **CacheStaticFileInHours**: Defines how long static files (like images and documents) should be cached in the system.

## Build and Deployment

### Build the Solution

If you wish to build the solution locally, use the following command:

```
dotnet build CourseEducation.sln
```

## Build and Deploy Services

Use Docker Compose to build and deploy the services:

Open the .env file and make sure that this is the content inside

```
ASPNETCORE_ENVIRONMENT=Production
```

Clear the old containers and images
```
docker-compose down --rmi all
```

Build the new images and start the containers
```
docker-compose -p courseeducation -f docker-compose.yml up -d --build
```

## Access the Services

- **API**: Accessible at [http://localhost:8090/swagger](http://localhost:8090).

## Stopping the Services

To stop all running services, use the following command:

```
docker-compose down
```

## Troubleshooting

- **Port Conflicts**: Ensure that the ports defined in the `docker-compose.yml` file are not in use by other applications.
- **Logs**: Check logs for each service by using the following command:

```
docker-compose logs -f <service_name>
```


## NGINX Reverse proxy setup

1. Install Nginx if not installed
```
sudo apt update
sudo apt install nginx
```

2. Install Certbot and Nginx plugin
```
sudo apt install certbot python3-certbot-nginx
```

3. Create Nginx Configuration File
```
sudo nano /etc/nginx/sites-available/yourdomain.com
```
Add this configuration
```
server {
    listen 80;
    server_name yourdomain.com www.yourdomain.com;

    location / {
        proxy_pass http://127.0.0.1:8090;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        client_max_body_size 50M;
        proxy_read_timeout 300s;            # Time to wait for a response from FastAPI
        proxy_connect_timeout 300s;         # Time to wait for the initial connection to FastAPI
        proxy_send_timeout 300s;            # Time to wait for sending data to FastAPI
    }
}
```

4. Enable the Configuration
```
sudo ln -s /etc/nginx/sites-available/yourdomain.com /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl restart nginx
```

5. Obtain SSL Certificate
```
sudo certbot --nginx -d yourdomain.com -d www.yourdomain.com
```

6. Verify Auto-renewal
```
sudo certbot renew --dry-run
```

7. Restart Nginx
```
sudo systemctl restart nginx
```
The API should now be accessible via https://yourdomain.com with a valid SSL certificate that will automatically renew.
