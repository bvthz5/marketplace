# Stage 1: Fetching the latest node image on Alpine Linux
FROM node:alpine AS builder

# Set the working directory
WORKDIR /app

# Copy package.json and package-lock.json (if exists) to install dependencies
COPY package*.json ./

# Install dependencies
RUN npm ci --production

# Copy the rest of the source code
COPY . .

# Build the application
RUN npm run build:prod

# Stage 2: Fetching the latest nginx image
FROM nginx

# Copy the built assets from the builder stage to the nginx HTML directory
COPY --from=builder /app/build /usr/share/nginx/html

# Copy the custom nginx.conf
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Expose port 80
EXPOSE 80
