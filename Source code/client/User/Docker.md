# Marketplace User UI

## Docker

### Local


docker build -f Dockerfile.local -t marketplace/local/user/ui:latest .

docker run --rm --name marketplace_user_ui -p 3001:3000 -v "$PWD:/react-app" marketplace/local/user/ui

### Dev

docker build -f Dockerfile.dev -t marketplace/dev/user/ui:latest .

docker run --rm -d --name marketplace_dev_user_ui -p 3001:80 marketplace/dev/user/ui

### Prod

docker build -t marketplace/prod/user/ui:latest .

docker run --rm -d --name marketplace_prod_user_ui -p 3001:80 marketplace/prod/user/ui