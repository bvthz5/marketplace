# Marketplace Admin UI






## Docker

### Local


docker build -f Dockerfile.local -t marketplace/local/admin/ui:latest .

docker run --rm --name marketplace_admin_ui -p 3000:3000 -v "$PWD:/react-app" marketplace/local/admin/ui

### Dev

docker build -f Dockerfile.dev -t marketplace/dev/admin/ui:latest .

docker run --rm -d --name marketplace_dev_admin_ui -p 3000:80 marketplace/dev/admin/ui

### Prod

docker build -t marketplace/prod/admin/ui:latest .

docker run --rm -d --name marketplace_prod_admin_ui -p 3001:80 marketplace/prod/admin/ui