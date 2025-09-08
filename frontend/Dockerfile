# Use an official Node.js runtime as the base image
FROM docker.io/node:slim AS builder

# Set the working directory in the container
WORKDIR /app

# Copy package.json and package-lock.json to the working directory
COPY package*.json ./

# Install dependencies
RUN npm install

# Copy the rest of the application code to the working directory
COPY . .

# Build the application
RUN npm run build

# Use an official Nginx runtime as the base image for serving the application
FROM docker.io/nginx:stable-alpine

# Copy the built application from the builder stage to the Nginx default directory
COPY --from=builder /app/dist /usr/share/nginx/html

# Expose port 8081 for HTTP traffic
EXPOSE 8081

# Start Nginx
CMD ["nginx", "-g", "daemon off;"]
