# Database

docker run `
    -d `
    -e "ACCEPT_EULA=Y" `
    -e "MSSQL_SA_PASSWORD=Admin@1234" `
    -p 1444:1433 `
    --name mssql_2019 `
    --hostname mssql_2019 `
    mcr.microsoft.com/mssql/server:2019-latest

-----------------------------------------------------------------------------------------------------------

# Api

cd "./Source code/server"

## Dev

### Admin

docker build --rm -f "./Admin/MarketplaceAdmin.Api/Dockerfile" -t marketplace/dev/admin/api:latest .

docker run --rm `
    -d `
    -p 8000:443 -p 8001:80 `
    -e ASPNETCORE_HTTP_PORT=https://+:443 `
    -e ASPNETCORE_URLS=http://+:80 `
    -e ASPNETCORE_ENVIRONMENT=DockerDevelopment `
    -e ConnectionStrings__Default="Server=10.10.25.160,1444;Database=Marketplace_Phase3;User Id=sa;password=Admin@1234;Trusted_Connection=False;MultipleActiveResultSets=true;TrustServerCertificate=true" `
    --name marketplace_dev_admin_api `
    marketplace/dev/admin/api


### User

docker build --rm -f "./User/MarketplaceUser.Api/Dockerfile" -t marketplace/dev/user/api:latest .

docker run --rm `
    -d `
    -p 8005:443 -p 8006:80 `
    -e ASPNETCORE_HTTP_PORT=https://+:443 `
    -e ASPNETCORE_URLS=http://+:80 `
    -e ASPNETCORE_ENVIRONMENT=DockerDevelopment `
    -e ConnectionStrings__Default="Server=10.10.25.160,1444;Database=Marketplace_Phase3;User Id=sa;password=Admin@1234;Trusted_Connection=False;MultipleActiveResultSets=true;TrustServerCertificate=true" `
    --name marketplace_dev_user_api `
    marketplace/dev/user/api




## Prod 

### Admin

docker build --rm -f "./Source code/server/Admin/MarketplaceAdmin.Api/Dockerfile" -t marketplace/dev/admin/api:latest .

docker run --rm `
    -d `
    -p 8010:443 -p 8011:80 `
    -e ASPNETCORE_HTTP_PORT=https://+:443 `
    -e ASPNETCORE_URLS=http://+:80 `
    -e ASPNETCORE_ENVIRONMENT=DockerProduction `
    -e ConnectionStrings__Default="Server=10.10.25.160,1444;Database=Marketplace_Phase3;User Id=sa;password=Admin@1234;Trusted_Connection=False;MultipleActiveResultSets=true;TrustServerCertificate=true" `
    --name marketplace_dev_admin_api `
    marketplace/dev/admin/api


### User

docker build --rm -f "./Source code/server/User/MarketplaceUser.Api/Dockerfile" -t marketplace/dev/user/api:latest .

docker run --rm `
    -d `
    -p 8015:443 -p 8016:80 `
    -e ASPNETCORE_HTTP_PORT=https://+:443 `
    -e ASPNETCORE_URLS=http://+:80 `
    -e ASPNETCORE_ENVIRONMENT=DockerProduction `
    -e ConnectionStrings__Default="Server=10.10.25.160,1444;Database=Marketplace_Phase3;User Id=sa;password=Admin@1234;Trusted_Connection=False;MultipleActiveResultSets=true; TrustServerCertificate=true" `
    --name marketplace_dev_user_api `
    marketplace/dev/user/api


-----------------------------------------------------------------------------------------------------------